
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
        #region Properties

        /// <summary>
        /// Gets the type that is bound by the binding.
        /// </summary>
        Type TypeBound { get; }

        /// <summary>
        /// Gets the type to which the binding should be resolved to.
        /// </summary>
        Type TypeToResolveTo { get; }

        /// <summary>
        /// Gets the type into which the binding should only inject to.
        /// </summary>
        Type TypeInjectedInto { get; }

        /// <summary>
        /// Gets a value that determines whether the binding should inject into the type specified in <see cref="TypeInjectedInto"/> and its sub-classes or only into the type specified in <see cref="TypeInjectedInto"/>. Only applies if <see cref="TypeInjectedInto"/> is not <c>null</c>.
        /// </summary>
        bool ShouldOnlyInjectExactlyInto { get; }

        #endregion

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