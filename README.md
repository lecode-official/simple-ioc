# Simple IoC

![Simple IoC Logo](https://github.com/lecode-official/simple-ioc/blob/master/Documentation/Images/Banner.png "Simple IoC Logo")

A simple and light-weight inversion of control container, which makes it possible to define dependencies declaratively in code, rather than in configuration files.

## Acknowledgements

[Ninject](http://www.ninject.org/) users may probably have noticed, that Simple IoC looks a lot like [Ninject](http://www.ninject.org/). This is actually no coincidence,
this project was originally created because we were developing on a platform, where [Ninject](http://www.ninject.org/) was unavailable at the time. Simple IoC is
heavily influenced by [Ninject](http://www.ninject.org/), which is a great inversion of control container and still our personal favorite. Because of that we want give
a big shoutout to [Nate Kohari](http://nate.io/) who is the brilliant developer behind [Ninject](http://www.ninject.org/). We really appreciate his great work for the
community. Although [Ninject](http://www.ninject.org/) is a more professional and feature-complete inversion of control container, we still think that Simple IoC has
its right to exist, because it was designed to be very simple and light-weight without lacking some of the most used features that [Ninject](http://www.ninject.org/)
offers.

## Using the Project

The project is available on NuGet: https://www.nuget.org/packages/System.InversionOfControl.

```batch
PM> Install-Package System.InversionOfControl
```

If you want to you can download and manually build the solution. The project was built using Visual Studio 2015. Basically any version of Visual Studio 2015 will
suffice, no extra plugins or tools are needed (except for the `System.InversionOfControl.nuproj` project, which needs the
[NuBuild Project System](https://visualstudiogallery.msdn.microsoft.com/3efbfdea-7d51-4d45-a954-74a2df51c5d0) Visual Studio extension for building the NuGet
package). Just clone the Git repository, open the solution in Visual Studio, and build the solution.

```batch
git pull https://github.com/lecode-official/simple-ioc.git
```

## Samples

It is very simple to create a new IoC container, which is called kernel in Simple IoC speak:

```csharp
Kernel kernel = new Kernel();
```

Once you have obtained a kernel, you can start to bind types to it:

```csharp
kernel.Bind<IVehicle>().ToType<Car>();
```
You can also bind types only when they are injected into another type. In this instance, a `Motorcycle` is injected into a `Person` if the `Person` is a `SuperCoolPerson`:

```csharp
kernel.Bind<IVehicle>().ToType<Motorcycle>().WhenInjectedInto<SuperCoolPerson>(); // Obviously super cool people drive motorcycles!
```

Now you can use the kernel to retrieve instances of the types:

```csharp
Person person = kernel.Resolve<Person>();
Person superCoolPerson = kernel.Resolve<SuperCoolPerson>();
```

You will notice, when you evaluate these two statements, that `person` will get an instance `Car` inject, while the `superCoolPerson` will get an instance of `Motorcycle`
injected. You can also pass arguments to the `Resolve` method, which will be prioritzed when injecting values into the constructors. When you evaluate the following
statement, you will see, that the `namedPerson` will get the `string` `"Bob"` injected as its name.

```csharp
Person namedPerson = kernel.Resolve<NamedPerson>("Bob");
```

## Contributions

Currently we are not accepting any contributors, but if you want to help, we would greatly appreciate feedback and bug reports. To file a bug, please use GitHub's
issue system. Alternatively, you can clone the repository and send us a pull request.
