
namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an interface for the when-injected-into-syntax.
    /// </summary>
    public interface IBindingWhenInjectedIntoSyntax
    {
        #region Methods

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected into the specified type or a sub-class of it.
        /// </summary>
        /// <typeparam name="T">The type the binding should only be injected into.</typeparam>
        void WhenInjectedInto<T>() where T : class;

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected exactly into the specified.
        /// </summary>
        /// <typeparam name="T">The type the binding should only be injected into.</typeparam>
        void WhenInjectedExactlyInto<T>() where T : class;

        #endregion
    }
}