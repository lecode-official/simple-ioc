
#region Using Directives

using System.Reflection;

#endregion

namespace System.InversionOfControl
{
    /// <summary>
    /// Represents a set of extension methods for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Extension Methods

        /// <summary>
        /// Injects the properties of the second object into the properties with the same name of the first object.
        /// </summary>
        /// <typeparam name="T">The type of the object into which is being injected.</typeparam>
        /// <param name="objectToInjectInto">The object into which the properties are to be injected.</param>
        /// <param name="injectionValues">An anonymous object, whose properties are matched to the properties of the object into which is being injected and assigns all possible values.</param>
        /// <returns>Returns the object into which the properties were injected.</returns>
        public static T Inject<T>(this T objectToInjectInto, object injectionValues) where T : class
        {
            // Validates the parameters
            if (objectToInjectInto == null)
                throw new ArgumentNullException(nameof(objectToInjectInto));
            
            // If there are any injection values, then they are injected into the first object
            if (injectionValues != null)
            {
                // Cycles through all properties of the injection values
                foreach (PropertyInfo sourcePropertyInformation in injectionValues.GetType().GetTypeInfo().DeclaredProperties)
                {
                    // Gets the property information of the corresponding property of the object into which is being injected
                    PropertyInfo targetPropertyInformation = objectToInjectInto.GetType().GetTypeInfo().GetDeclaredProperty(sourcePropertyInformation.Name);

                    // Checks if the property was found, the types match and if the setter is implemented, if not then the value cannot be assigned and the algorithm turns to the next parameter
                    if (targetPropertyInformation == null || !targetPropertyInformation.CanWrite || !targetPropertyInformation.PropertyType.GetTypeInfo().IsAssignableFrom(sourcePropertyInformation.PropertyType.GetTypeInfo()))
                        continue;

                    // Sets the value of the property in the object into which is being injected to the value provided in the injection values
                    targetPropertyInformation.SetValue(objectToInjectInto, sourcePropertyInformation.GetValue(injectionValues));
                }
            }
            
            // Returns the original object, so that the caller is able to chain calls
            return objectToInjectInto;
        }

        #endregion
    }
}