
#region Using Directives

using System;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents an exception that is thrown when a type could not be resolved by the dependency injection system.
    /// </summary>
    public sealed class ResolveException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ResolveException"/> instance.
        /// </summary>
        public ResolveException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ResolveException"/> instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ResolveException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ResolveException"/> instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The original exception that caused this exception to be thrown.</param>
        public ResolveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}