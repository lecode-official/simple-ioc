
#region Using Directives

using System;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an interface for bindings.
    /// </summary>
    internal interface IBinding : IDisposable
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified type can be resolved by this binding.
        /// </summary>
        /// <param name="typeToResolve">The type that is to be resolved.</param>
        /// <param name="typeInjectedInto">The type that the resolved type will be injected into. Can be <c>null</c> if the type is only resolved and not injected into anything.</param>
        /// <returns>Returns a value that determines whether the type can be resolved or not.</returns>
        bool CanResolve(Type typeToResolve, Type typeInjectedInto);

        /// <summary>
        /// Resolves the specified type by creating a new instance of it.
        /// </summary>
        /// <returns>Returns an instance of the type that is to be resolved.</returns>
        object Resolve();

        /// <summary>
        /// Resolves the specified type by creating a new instance of it. The objects specified <c>explicitConstructorParameters</c> are preferred, when injecting into the constructor.
        /// </summary>
        /// <param name="explicitConstructorParameters">A list of constructor parameters, which are preferred, when injecting into the constructor. Not all explicit parameters may be used.</param>
        /// <returns>Returns an instance of the type that is to be resolved.</returns>
        object Resolve(params object[] explicitConstructorParameters);

        #endregion
    }
}