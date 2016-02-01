﻿
#region Using Directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents a binding of a type that can be used by the dependency injection kernel to resolve that type to a concrete instance.
    /// </summary>
    /// <typeparam name="T">The type that is to be bound.</typeparam>
    internal sealed class Binding<T> : IBinding, IBindingToSyntax<T>, IBindingInScopeSyntax, IBindingWhenInjectedIntoSyntax where T : class
    {
        #region Constructors

        /// <summary>
        /// Intializes a new <see cref="Binding"/> instance. It is private because this class uses the factory pattern to instantiate new instances.
        /// </summary>
        /// <param name="kernel">The kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.</param>
        /// <param name="typeBound">The type that should be bound by the binding.</param>
        private Binding(Kernel kernel, Type typeBound)
        {
            this.kernel = kernel;
            this.typeBound = typeBound;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.
        /// </summary>
        private Kernel kernel;

        /// <summary>
        /// Contains the scope in which the binding is being resolved.
        /// </summary>
        private ResolvingScope scope = ResolvingScope.Transient;

        /// <summary>
        /// Contains the type that is bound by the binding.
        /// </summary>
        private Type typeBound;

        /// <summary>
        /// Contains the type to which the binding should be resolved to.
        /// </summary>
        private Type typeToResolveTo;

        /// <summary>
        /// Contains the factory method, which should be used to instantiate the type.
        /// </summary>
        private Func<T> typeResolveFactory;

        /// <summary>
        /// Contains the type into which the binding should only inject to.
        /// </summary>
        private Type typeInjectedInto;

        /// <summary>
        /// Contains a value that determines whether the binding should inject into the type specified in <see cref="typeInjectedInto"/> and its sub-classes or only into the type specified in <see cref="typeInjectedInto"/>. Only applies if
        /// <see cref="typeInjectedInto"/> is not <c>null</c>.
        /// </summary>
        private bool shouldOnlyInjectExactlyInto;

        /// <summary>
        /// Contains a resolved singleton instance. This is only used if the binding is in singleton scope. A resolved singleton instance is stored separately, because it must not be a weak reference (if it were, there is a chance that the
        /// reference gets lost).
        /// </summary>
        private T resolvedSingletonInstance = null;

        /// <summary>
        /// Contains a list of all the instances that have been resolved by this binding.
        /// </summary>
        private List<WeakReference<T>> resolvedInstances = new List<WeakReference<T>>();

        /// <summary>
        /// Contains a value that determines whether binding is currently being disposed.
        /// </summary>
        private bool isDisposing;
        
        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Creates a new binding for the specified type.
        /// </summary>
        /// <typeparam name="T">The type that should be bound by the binding.</typeparam>
        /// <param name="kernel">The kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.</param>
        /// <exception cref="InvalidOperationException">If the specified type is generic or is not a class or interface, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created binding.</returns>
        public static Binding<T> Create(Kernel kernel)
        {
            // Gets the type information for the type that is to be bound and validates if the type qualifies for binding, if not then an OperationException is thrown
            TypeInfo typeInformation = typeof(T).GetTypeInfo();
            if (typeInformation.ContainsGenericParameters)
                throw new InvalidOperationException("Bound tyype must not be generic.");

            // Creates the new binding and returns it
            return new Binding<T>(kernel, typeof(T));
        }

        #endregion

        #region IBinding Implementation

        /// <summary>
        /// Determines whether the specified type can be resolved by this binding.
        /// </summary>
        /// <param name="typeToResolve">The type that is to be resolved.</param>
        /// <param name="injectionTargetType">The type that the resolved type will be injected into. Can be <c>null</c> if the type is only resolved and not injected into anything.</param>
        /// <returns>Returns a value that determines whether the type can be resolved or not.</returns>
        public bool CanResolve(Type typeToResolve, Type injectionTargetType)
        {
            // Checks if the the binding has a constraint for the types that it may be injected into
            if (this.typeInjectedInto != null)
            {
                // Checks if the type is injected into anything, if not then the type cannot be resolved
                if (injectionTargetType == null)
                    return false;

                // Checks if the type can be injected into
                if (this.shouldOnlyInjectExactlyInto && this.typeInjectedInto != injectionTargetType)
                    return false;
                if (!this.shouldOnlyInjectExactlyInto)
                {
                    TypeInfo typeInformation = injectionTargetType.GetTypeInfo();
                    if (this.typeInjectedInto != injectionTargetType && !typeInformation.IsSubclassOf(this.typeInjectedInto) && !typeInformation.ImplementedInterfaces.Contains(injectionTargetType))
                        return false;
                }
            }

            // Checks if the type that is to be resolved is the type that is bound by this binding
            return this.typeBound == typeToResolve;
        }

        /// <summary>
        /// Resolves the specified type by creating a new instance of it.
        /// </summary>
        /// <exception cref="ResolveException">If the type could not be resolved, an <see cref="ResolveException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the type that is to be resolved.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public object Resolve()
        {
            // Removes all reference that we have already lost the reference for
            this.resolvedInstances.RemoveAll(instance =>
                {
                    T target;
                    return !instance.TryGetTarget(out target);
                });

            // Checks if the resolve scope is singleton and there is already an instance, if so then the existent instance is returned
            if (this.scope == ResolvingScope.Singleton && this.resolvedSingletonInstance != null)
                return this.resolvedSingletonInstance;

            // Checks if the user specified a factory for the resolving, if so, it is invoked and the result is returned
            if (this.typeResolveFactory != null)
            {
                // Creates the instance by invoking the factory method
                T instance = (T)this.typeResolveFactory();

                // Stores the resolved instance (if the binding is in singleton scope, then it must be stored separately so that its reference does not get lost via the weak reference)
                if (this.scope == ResolvingScope.Singleton)
                    this.resolvedSingletonInstance = instance;
                else
                    this.resolvedInstances.Add(new WeakReference<T>(instance));

                // Returns the created instance
                return instance;
            }

            // Gets the information about all the constructors of the type and sorts them by their parameter count (the algorithm is greedy and tries to take the constructor that has the most parameters it is able to resolve)
            TypeInfo typeInformation = this.typeToResolveTo.GetTypeInfo();
            IOrderedEnumerable<ConstructorInfo> constructorInformations = typeInformation.DeclaredConstructors.OrderByDescending(constructorInformation => constructorInformation.GetParameters().Count());
            
            // Cycles over all the constructors and tries them one by one
            foreach (ConstructorInfo constructorInformation in constructorInformations)
            {
                // Gets all the parameters of the constructor
                Dictionary<ParameterInfo, IBinding> parameterInformations = constructorInformation.GetParameters().ToDictionary(parameterInformation => parameterInformation, parameterInformation => this.kernel.FindMatchingBinding(parameterInformation.ParameterType, this.typeToResolveTo));

                // Tries to resolve all the parameters
                List<object> parameterValues = new List<object>();
                try
                {
                    foreach (KeyValuePair<ParameterInfo, IBinding> parameterInformation in parameterInformations)
                    {
                        // Tries to resolve the type of the parameter, if that does not work, then the default value for the parameter is used, if it has no default value, then the next constructor is tried
                        try
                        {
                            // Checks if a binding for the parameter could be found, if not then a resolve exception is thronw, otherwise, it tries to resolve the parameter type
                            if (parameterInformation.Value == null)
                                throw new ResolveException("No binding for constructor parameter found");
                            parameterValues.Add(parameterInformation.Value.Resolve());
                        }
                        catch (ResolveException)
                        {
                            if (parameterInformation.Key.HasDefaultValue)
                                parameterValues.Add(parameterInformation.Key.DefaultValue);
                            else
                                throw;
                        }
                    }
                }
                catch (ResolveException)
                {
                    // Since the parameters could not be resolved, the next constructor is tried
                    continue;
                }

                // Tries to invoke the default constructor and returns the created object
                try
                {
                    // Creates the instance by invoking the constructor
                    T instance = (T)constructorInformation.Invoke(parameterValues.ToArray());

                    // Stores the resolved instance (if the binding is in singleton scope, then it must be stored separately so that its reference does not get lost via the weak reference)
                    if (this.scope == ResolvingScope.Singleton)
                        this.resolvedSingletonInstance = instance;
                    else
                        this.resolvedInstances.Add(new WeakReference<T>(instance));

                    // Returns the created instance
                    return instance;
                }
                catch (NullReferenceException) { }
                catch (MemberAccessException) { }
                catch (ArgumentException) { }
                catch (TargetInvocationException) { }
                catch (TargetParameterCountException) { }
                catch (NotSupportedException) { }
                catch (SecurityException) { }
            }

            // If we have come this far, the type could not be resolved, therefore an exception is thrown
            throw new ResolveException("No valid constructor for resolving found.");
        }

        #endregion

        #region IBindingToSyntax Implementation

        /// <summary>
        /// Specifies the type to which the binding should be resolved.
        /// </summary>
        /// <typeparam name="TResolve">The type to which the binding is bound.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// If the specified type is generic, is abstract, is not a class or struct, or does not inherit or implement the bound type, then an <see cref="InvalidOperationException"/> exception is thrown.
        /// </exception>
        /// <returns>Returns the binding for chaining calls.</returns>
        public IBindingInScopeSyntax ToType<TResolve>() where TResolve : class, T
        {
            // Gets the type information for the type that is to be bound
            Type newTypeToResolveTo = typeof(TResolve);
            TypeInfo typeInformation = newTypeToResolveTo.GetTypeInfo();

            // Validates if the type qualifies for resolving, if not then an OperationException is thrown
            if (typeInformation.IsAbstract)
                throw new InvalidOperationException("Resolving type must not be abstract.");
            if (typeInformation.ContainsGenericParameters)
                throw new InvalidOperationException("Resolving type must not be generic.");

            // Sets the type to which the binding is bound
            this.typeToResolveTo = newTypeToResolveTo;

            // Returns the binding so that calls to it may be chained
            return this;
        }

        /// <summary>
        /// Specifies that the binding should resolve to itself.
        /// </summary>
        /// <returns>Returns the binding for chaining calls.</returns>
        public IBindingInScopeSyntax ToSelf()
        {
            return this.ToType<T>();
        }

        /// <summary>
        /// Specified the factory to which the binding should be resolved.
        /// </summary>
        /// <typeparam name="TResolve">The type to which the binding is bound.</typeparam>
        /// <param name="factory">The factory method, which should be used to instantiate the type.</param>
        /// <returns>Returns the binding for chaining calls.</returns>
        public IBindingInScopeSyntax ToFactory<TResolve>(Func<TResolve> factory) where TResolve : class, T
        {
            // Stores the type to which the binding should resolve and the factory, which is used to instantiate the type
            this.ToType<TResolve>();
            this.typeResolveFactory = factory;

            // Returns the binding so that calls to it may be chained
            return this;
        }

        #endregion

        #region IBindingInScopeSyntax Implementation

        /// <summary>
        /// Binds the binding in transient scope, which means everytime the binding is resolved, a new instance of the bound type is created.
        /// </summary>
        public IBindingWhenInjectedIntoSyntax InTransientScope()
        {
            this.scope = ResolvingScope.Transient;
            return this;
        }

        /// <summary>
        /// Binds the binding in singleton scope, which means that only the first time the binding is resolved, a new instance of the bound type is creaeted. If one already exists, then is returned.
        /// </summary>
        public IBindingWhenInjectedIntoSyntax InSingletonScope()
        {
            this.scope = ResolvingScope.Singleton;
            return this;
        }

        #endregion

        #region IBindingWhenInjectedIntoSyntax Implementation

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected into the specified type or a sub-class of it.
        /// </summary>
        /// <typeparam name="TInjectionTarget">The type the binding should only be injected into.</typeparam>
        public void WhenInjectedInto<TInjectionTarget>() where TInjectionTarget : class
        {
            this.typeInjectedInto = typeof(T);
            this.shouldOnlyInjectExactlyInto = false;
        }

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected exactly into the specified.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the specified type is abstract or an interface, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <typeparam name="TInjectionTarget">The type the binding should only be injected into.</typeparam>
        public void WhenInjectedExactlyInto<TInjectionTarget>() where TInjectionTarget : class
        {
            // Validates whether the type is qualified
            Type newTypeInjectedInto = typeof(T);
            TypeInfo typeInformation = newTypeInjectedInto.GetTypeInfo();
            if (typeInformation.IsAbstract || typeInformation.IsInterface)
                throw new InvalidOperationException("Type injected into must not be abstract or interface.");

            // Assigns the new values
            this.typeInjectedInto = newTypeInjectedInto;
            this.shouldOnlyInjectExactlyInto = true;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of all the resources that were allocated by the binding.
        /// </summary>
        public void Dispose()
        {
            // Checks if the kernel is currently being disposed of (this is needed because the user could use a binding for the kernel itself, which would then result in an infinite loop (disposing the kernel means disposing the binding for
            // the kernel, which would itself dispose of the kernel)
            if (this.isDisposing)
                return;
            this.isDisposing = true;

            // Cycles over all the instances that have been created by this binding and disposes of them if they are IDisposable
            foreach (WeakReference<T> reference in this.resolvedInstances.ToList())
            {
                T instance;
                if (reference.TryGetTarget(out instance))
                {
                    IDisposable disposibleInstance = instance as IDisposable;
                    if (disposibleInstance != null)
                        disposibleInstance.Dispose();
                }
                this.resolvedInstances.Remove(reference);
            }

            // Checks if there is a singleton instance that was created, and disposes of it, if it implements IDisposable
            IDisposable singletonDisposable = this.resolvedSingletonInstance as IDisposable;
            if (singletonDisposable != null)
                singletonDisposable.Dispose();
            this.resolvedSingletonInstance = null;
        }

        #endregion
    }
}