
#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents the default binding which can be used to instantiate types with a default constructor. This is useful if there is no binding for the type.
    /// </summary>
    internal class DefaultBinding : IBinding
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="DefaultBinding"/> instance. It is private because this class uses the factory pattern to instantiate new instances.
        /// </summary>
        /// <param name="kernel">The kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.</param>
        /// <param name="typeToBeResolved">The type that is to be resolved.</param>
        private DefaultBinding(Kernel kernel, Type typeToBeResolved)
        {
            this.kernel = kernel;
            this.typeToBeResolved = typeToBeResolved;
        }

        #endregion
        
        #region Private Fields

        /// <summary>
        /// Contains the kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.
        /// </summary>
        private Kernel kernel;

        /// <summary>
        /// Contains the type that is to be resolved by the default binding.
        /// </summary>
        private Type typeToBeResolved;

        /// <summary>
        /// Contains a list of all the instances that have been resolved by this binding.
        /// </summary>
        private List<object> resolvedInstances = new List<object>();

        /// <summary>
        /// Contains a value that determines whether binding is currently being disposed.
        /// </summary>
        private bool isDisposing;
        
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a new default binding for the specified type.
        /// </summary>
        /// <typeparam name="typeToBind">The type that should be bound by the binding.</typeparam>
        /// <param name="kernel">The kernel, which is needed to resolve any dependencies that this binding has to other bindings, when resolving a type.</param>
        /// <exception cref="InvalidOperationException">If the specified type is generic, abstract, an interface or has no default constructor, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created default binding.</returns>
        public static DefaultBinding Create(Kernel kernel, Type typeToBind)
        {
            // Gets the type information for the type that is to be bound and validates if the type qualifies for binding, if not then an OperationException is thrown
            TypeInfo typeInformation = typeToBind.GetTypeInfo();
            if (typeInformation.ContainsGenericParameters)
                throw new InvalidOperationException("Bound type must not be generic.");
            if (!typeInformation.IsClass || typeInformation.IsAbstract)
                throw new InvalidOperationException("Bound type must be non abstract class.");

            // Creates the new binding and returns it
            return new DefaultBinding(kernel, typeToBind);
        }

        #endregion

        #region IBinding Implementation

        /// <summary>
        /// Determines whether the specified type can be resolved by this binding.
        /// </summary>
        /// <param name="typeToResolve">The type that is to be resolved.</param>
        /// <param name="injectionTargetType">The type that the resolved type will be injected into. Can be <c>null</c> if the type is only resolved and not injected into anything.</param>
        /// <returns>Returns a value that determines whether the type can be resolved or not.</returns>
        public bool CanResolve(Type typeToResolve, Type typeInjectedInto)
        {
            // The default binding is only able to resolve excactly the type it was created for
            return this.typeToBeResolved == typeToResolve;
        }

        /// <summary>
        /// Resolves the specified type by creating a new instance of it.
        /// </summary>
        /// <exception cref="ResolveException">If the type could not be resolved, an <see cref="ResolveException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the type that is to be resolved.</returns>
        public object Resolve()
        {
            // Gets the information about all the constructors of the type and sorts them by their parameter count (the algorithm is greedy and tries to take the constructor that has the most parameters it is able to resolve)
            TypeInfo typeInformation = this.typeToBeResolved.GetTypeInfo();
            IOrderedEnumerable<ConstructorInfo> constructorInformations = typeInformation.DeclaredConstructors.OrderByDescending(constructorInformation => constructorInformation.GetParameters().Count());

            // Cycles over all the constructors and tries them one by one
            foreach (ConstructorInfo constructorInformation in constructorInformations)
            {
                // Gets all the parameters of the constructor
                Dictionary<ParameterInfo, IBinding> parameterInformations = constructorInformation.GetParameters().ToDictionary(parameterInformation => parameterInformation, parameterInformation => this.kernel.FindMatchingBinding(parameterInformation.ParameterType, this.typeToBeResolved));

                // Tries to resolve all the parameters
                List<object> parameterValues = new List<object>();
                try
                {
                    foreach (KeyValuePair<ParameterInfo, IBinding> parameterInformation in parameterInformations)
                    {
                        // Tries to resolve the type of the paramter, if that does not work, then the default value for the parameter is used, if it has no default value, then the next constructor is tried
                        try
                        {
                            // Checks if a binding for the parameter could be found, if not then a resolve exception is thronw, otherwise, it tries to resolve the paramter type
                            if (parameterInformation.Value == null)
                                throw new ResolveException("No binding for constructor parameter found.");
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
                    return constructorInformation.Invoke(parameterValues.ToArray());
                }
                catch (NullReferenceException) { }
                catch (MemberAccessException) { }
                catch (ArgumentException) { }
                catch (TargetInvocationException) { }
                catch (TargetParameterCountException) { }
                catch (NotSupportedException) { }
                catch (SecurityException) { }
            }

            // If we have come this far, an exception has occurred, therefore an resolve exception is thrown
            throw new ResolveException("Type could not be resolved by default binding.");
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
            foreach (object instance in this.resolvedInstances.ToList())
            {
                // Disposed of the instance, if it implements IDisposable, if it already has been disposed of, then nothing is done
                try
                {
                    IDisposable disposibleInstance = instance as IDisposable;
                    if (disposibleInstance != null)
                        disposibleInstance.Dispose();
                }
                catch (ObjectDisposedException) { }

                // Removes the instance from the list of resolved instances
                this.resolvedInstances.Remove(instance);
            }
        }

        #endregion
    }
}