
#region Using Directives

using System.InversionOfControl;

#endregion

namespace SimpleIoc.Samples.Console
{
    /// <summary>
    /// Represents the console app sample program for the Simple IoC.
    /// </summary>
    public class Program
    {
        #region Public Static Methods

        /// <summary>
        /// Represents the entry point to the sample application.
        /// </summary>
        /// <param name="args">The command line arguments, that were passed to the program. Are not used.</param>
        public static void Main(string[] args)
        {
            // Creates a new Simple IoC kernel
            Kernel kernel = new Kernel();

            // Binds the vehicles to the kernel
            kernel.Bind<IVehicle>().ToType<Car>();
            kernel.Bind<IVehicle>().ToType<Motorcycle>().InTransientScope().WhenInjectedInto<SuperCoolPerson>(); // Obviously super cool people drive motorcycles!

            // Creates two persons and prints them out
            Person person = kernel.Resolve<Person>();
            Person superCoolPerson = kernel.Resolve<SuperCoolPerson>();
            System.Console.WriteLine(person);
            System.Console.WriteLine(superCoolPerson);

            // Waits for a key stroke, before the application is quit
            System.Console.ReadLine();
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Represents a person.
        /// </summary>
        private class Person
        {
            #region Constructors

            /// <summary>
            /// Initializes a new <see cref="Person"/> instance.
            /// </summary>
            /// <param name="vehicle">The vehicle that the person is driving.</param>
            public Person(IVehicle vehicle)
            {
                this.Vehicle = vehicle;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the vehicle that the person is driving.
            /// </summary>
            public IVehicle Vehicle { get; set; }

            #endregion

            #region Object Implementation

            /// <summary>
            /// Generates a string out of the person object.
            /// </summary>
            /// <returns>Returns the textual representation of the person.</returns>
            public override string ToString() => $"The person is driving a {this.Vehicle.Name}.";

            #endregion
        }

        /// <summary>
        /// Represents a super cool person.
        /// </summary>
        private class SuperCoolPerson : Person
        {
            #region Constructors

            /// <summary>
            /// Initializes a new <see cref="SuperCoolPerson"/> instance.
            /// </summary>
            /// <param name="vehicle">The vehicle that the super cool person is driving.</param>
            public SuperCoolPerson(IVehicle vehicle)
                : base(vehicle) { }

            #endregion
        }

        /// <summary>
        /// Represents an interface for vehicles.
        /// </summary>
        private interface IVehicle
        {
            #region Properties

            /// <summary>
            /// Gets the name of the vehicle.
            /// </summary>
            string Name { get; }

            #endregion
        }

        /// <summary>
        /// Represents a car.
        /// </summary>
        private class Car : IVehicle
        {
            #region IVehicle Implementation

            /// <summary>
            /// Gets the name of the vehicle.
            /// </summary>
            public string Name
            {
                get
                {
                    return "car";
                }
            }

            #endregion
        }

        /// <summary>
        /// Represents a motorcycle.
        /// </summary>
        private class Motorcycle : IVehicle
        {
            #region IVehicle Implementation

            /// <summary>
            /// Gets the name of the vehicle.
            /// </summary>
            public string Name
            {
                get
                {
                    return "motorcycle";
                }
            }

            #endregion
        }

        #endregion
    }
}