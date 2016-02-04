
namespace System.InversionOfControl
{
    /// <summary>
    /// Represents a binding syntax, which can either be a binding in scope syntax or a binding when injected into syntax.
    /// </summary>
    public interface IBindingInScopeOrWhenInjectedIntoSyntax : IBindingInScopeSyntax, IBindingWhenInjectedIntoSyntax { }
}