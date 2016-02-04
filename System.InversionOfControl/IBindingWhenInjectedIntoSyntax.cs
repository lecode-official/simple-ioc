
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
        void WhenInjectedInto<T>();

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected into the specified type or a sub-class of it.
        /// </summary>
        /// <param name="type">The type the binding should only be injected into.</param>
        void WhenInjectedInto(Type type);

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected exactly into the specified.
        /// </summary>
        /// <typeparam name="T">The type the binding should only be injected into.</typeparam>
        void WhenInjectedExactlyInto<T>();

        /// <summary>
        /// Determines that the binding should only be used when the binding is being injected exactly into the specified.
        /// </summary>
        /// <param name="type">The type the binding should only be injected into.</param>
        void WhenInjectedExactlyInto(Type type);

        #endregion
    }
}