
namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an interface for the scoped binding syntax.
    /// </summary>
    public interface IBindingInScopeSyntax
    {
        #region Methods

        /// <summary>
        /// Binds the binding in transient scope, which means everytime the binding is resolved, a new instance of the bound type is created.
        /// </summary>
        /// <returns>Returns the binding for chaining calls.</returns>
        IBindingWhenInjectedIntoSyntax InTransientScope();

        /// <summary>
        /// Binds the binding in singleton scope, which means that only the first time the binding is resolved, a new instance of the bound type is creaeted. If one already exists, then is returned.
        /// </summary>
        /// <returns>Returns the binding for chaining calls.</returns>
        IBindingWhenInjectedIntoSyntax InSingletonScope();

        #endregion
    }
}