
#region Using Directives

using System;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an interface for the binding-to-syntax.
    /// </summary>
    /// <typeparam name="T">The type to which the binding should be bound.</typeparam>
    public interface IBindingToSyntax<T>
    {
        #region Methods

        /// <summary>
        /// Specifies the type to which the binding should be resolved.
        /// </summary>
        /// <typeparam name="TResolve">The type to which the binding is bound.</typeparam>
        /// <returns>Returns the binding for chaining calls.</returns>
        IBindingInScopeSyntax ToType<TResolve>() where TResolve : class, T;

        /// <summary>
        /// Specifies that the binding should resolve to itself.
        /// </summary>
        /// <returns>Returns the binding for chaining calls.</returns>
        IBindingInScopeSyntax ToSelf();

        /// <summary>
        /// Specified the factory to which the binding should be resolved.
        /// </summary>
        /// <typeparam name="TResolve">The type to which the binding is bound.</typeparam>
        /// <returns>Returns the binding for chaining calls.</returns>
        IBindingInScopeSyntax ToFactory<TResolve>(Func<TResolve> factory) where TResolve : class, T;

        #endregion
    }
}