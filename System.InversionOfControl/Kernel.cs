﻿
#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents a simple dependency injection kernel, that can be used to configure dependencies and instantiate instances of classes.
    /// </summary>
    public sealed class Kernel : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Contains all the bindings of the kernel.
        /// </summary>
        private List<IBinding> bindings = new List<IBinding>();
        
        /// <summary>
        /// Contains a value that determines whether kernel is currently being disposed.
        /// </summary>
        private bool isDisposing;
        
        #endregion

        #region Internal Methods

        /// <summary>
        /// Finds the first matching binding for the type that is to be resolved and the type into which is to be injected.
        /// </summary>
        /// <param name="typeToResolve">The type that is to be resolved.</param>
        /// <param name="typeInjectedInto">The type into which the resolved type is to be injected into. May be <c>null</c> if the type is only resolved.</param>
        /// <returns>Returns the first matching binding or <c>null</c> if none could be found.</returns>
        internal IBinding FindMatchingBinding(Type typeToResolve, Type typeInjectedInto)
        {
            // Gets the first matching binding and returns it if it was found
            IBinding matchedBinding = this.bindings.FirstOrDefault(binding => binding.CanResolve(typeToResolve, typeInjectedInto));
            if (matchedBinding != null)
                return matchedBinding;

            // If no matching binding was found, then we try to create a default binding for it
            try
            {
                IBinding defaultBinding = DefaultBinding.Create(this, typeToResolve);
                this.bindings.Add(defaultBinding);
                return defaultBinding;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new binding for the specified type.
        /// </summary>
        /// <typeparam name="T">The type that is to be bound.</typeparam>
        /// <returns>Returns the created binding.</returns>
        public IBindingToSyntax<T> Bind<T>() where T : class
        {
            Binding<T> newBinding = Binding<T>.Create(this);
            this.bindings.Add(newBinding);
            return newBinding;
        }

        /// <summary>
        /// Tries to resolve the specified type.
        /// </summary>
        /// <param name="type">The type for which an instance is to be created.</param>
        /// <exception cref="ResolveException">If the type could not be resolved, an <see cref="ResolveException"/> exception is thrown.</exception>
        /// <returns>Returns the resolved object.</returns>
        public object Resolve(Type type)
        {
            IBinding binding = this.FindMatchingBinding(type, null);
            if (binding == null)
                throw new ResolveException("No matching binding found.");
            return binding.Resolve();
        }

        /// <summary>
        /// Tries to resolve the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which an instance is to be created.</typeparam>
        /// <exception cref="ResolveException">If the type could not be resolved, an <see cref="ResolveException"/> exception is thrown.</exception>
        /// <returns>Returns the resolved object.</returns>
        public T Resolve<T>()
        {
            return (T)this.Resolve(typeof(T));
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of all the resources that have been allocated by the kernel.
        /// </summary>
        public void Dispose()
        {
            // Checks if the kernel is currently being disposed of (this is needed because the user could use a binding for the kernel itself, which would then result in an infinite loop (disposing the kernel means disposing the binding for
            // the kernel, which would itself dispose of the kernel)
            if (this.isDisposing)
                return;
            this.isDisposing = true;

            // Diposes of all the binding, which in turn dispose of all the instances they have resolved
            foreach (IBinding binding in this.bindings.ToList())
            {
                binding.Dispose();
                this.bindings.Remove(binding);
            }
        }

        #endregion
    }
}