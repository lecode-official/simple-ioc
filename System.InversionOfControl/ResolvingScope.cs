
namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an enumeration of the scopes in which a type can be resolved.
    /// </summary>
    public enum ResolvingScope
    {
        /// <summary>
        /// Each time a type is resolved, a new instance is created.
        /// </summary>
        Transient,

        /// <summary>
        /// A new instance of a type that is being resolved is only created once. If it already exists, then the type is resolved to the existing instance.
        /// </summary>
        Singleton
    }
}