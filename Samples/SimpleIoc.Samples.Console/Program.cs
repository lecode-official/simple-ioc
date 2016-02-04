
#region Using Directives

using System;
using System.InversionOfControl;
using System.Threading;

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
            kernel.Bind<IVehicle>().ToType<Motorcycle>().WhenInjectedInto<SuperCoolPerson>(); // Obviously super cool people drive motorcycles!

            // Creates some persons
            Person person = kernel.Resolve<Person>();
            Person superCoolPerson = kernel.Resolve<SuperCoolPerson>();
            Person namedPerson = kernel.Resolve<NamedPerson>("Bob");

            // Prints out the personal information about the persons that were created
            System.Console.WriteLine(person);
            System.Console.WriteLine(superCoolPerson);
            System.Console.WriteLine(namedPerson);

            // Demonstrates singleton scope, where the resolved instance is always the same
            kernel.Bind<DateTime>().ToFactory(() => DateTime.UtcNow).InSingletonScope();
            System.Console.WriteLine(kernel.Resolve<DateTime>().Ticks);
            Thread.Sleep(1000);
            System.Console.WriteLine(kernel.Resolve<DateTime>().Ticks);

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
        /// Represents a person, which has a name, for whatever that is good for.
        /// </summary>
        private class NamedPerson : Person
        {
            #region Constructors

            /// <summary>
            /// Initializes a new <see cref="NamedPerson"/> instance (this constructor is only here to show off that the best matching constructor is selected when resolving a type).
            /// </summary>
            /// <param name="vehicle">The vehicle the person is driving.</param>
            public NamedPerson(IVehicle vehicle)
                : base(vehicle) { }
            
            /// <summary>
            /// Initializes a new <see cref="NamedPerson"/> instance.
            /// </summary>
            /// <param name="name">The name of the person.</param>
            /// <param name="vehicle">The vehicle the person is driving.</param>
            public NamedPerson(string name, IVehicle vehicle)
                : base(vehicle)
            {
                this.Name = name;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the name of the person.
            /// </summary>
            public string Name { get; set; }
            
            #endregion

            #region Object Implementation

            /// <summary>
            /// Generates a string out of the person object.
            /// </summary>
            /// <returns>Returns the textual representation of the person.</returns>
            public override string ToString() => $"{this.Name} is driving a {this.Vehicle.Name}.";

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