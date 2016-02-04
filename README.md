# Simple IoC

A simple and light-weight inversion of control container, which makes it possible to define dependencies declaratively in code, rather than in configuration files.

## Acknowledgements

[Ninject](http://www.ninject.org/) users may probably have noticed, that Simple IoC looks a lot like [Ninject](http://www.ninject.org/). This is actually no coincidence,
this project originally was created because we were developing on a platform, where [Ninject](http://www.ninject.org/) was unavailable at the time. Simple IoC is
heavily influenced by [Ninject](http://www.ninject.org/), which is a great inversion of control container and still our personal favorite. Because of that we want give
a big shoutout to [Nate Kohari](http://nate.io/) who is the brilliant developer behind [Ninject](http://www.ninject.org/). We really appreciate his great work for the community.

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