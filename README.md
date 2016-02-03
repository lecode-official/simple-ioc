# Simple IoC

A simple and light-weight inversion of control container, which makes it possible to define dependencies declaratively in code, rather than in configuration files.

## Using the Project

The project is available on NuGet: https://www.nuget.org/packages/System.InversionOfControl.

```batch
PM> Install-Package System.InversionOfControl
```

If you want to you can download and manually build the solution. The project was built using Visual Studio 2015. Basically any version of Visual Studio 2015 will
suffice, no extra plugins or tools are needed (except for the `System.InversionOfControl.Nuget` project, which needs the
[NuBuild Project System](https://visualstudiogallery.msdn.microsoft.com/3efbfdea-7d51-4d45-a954-74a2df51c5d0) Visual Studio extension for building the NuGet
package). Just clone the Git repository, open the solution in Visual Studio, and build the solution.

```batch
git pull https://github.com/lecode-official/simple-ioc.git
```