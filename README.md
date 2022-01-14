J2N - Java-like Components for .NET
=========

[![Nuget](https://img.shields.io/nuget/dt/J2N)](https://www.nuget.org/packages/J2N)
[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/NightOwl888/b2719dac-eeb4-46ff-9380-13b45ff0277b/1/release/v2.0)](https://dev.azure.com/NightOwl888/J2N/_build?definitionId=1)
[![GitHub](https://img.shields.io/github/license/NightOwl888/J2N)](https://github.com/NightOwl888/J2N/blob/master/LICENSE.txt)
[![GitHub Sponsors](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/NightOwl888)

J2N is a library that helps bridge the gap between .NET and Java.

### Our Goals

* Java-like behaviors
* .NET-like APIs
* Be the defacto library to use when porting from Java to .NET
* Provide high quality, high performance components that can be used in a wide range of .NET applications

Basically, if you are looking for a "JDK.NET", this is about as close as you can get. While we recommend using purely .NET components where possible when porting from Java, there are some Java features that have no .NET counterpart or the .NET counterpart is lacking behaviors that are not easy to reproduce without reinventing the wheel. Even if you prefer to reinvent the wheel by designing your own ".NETified" component, you may still need a Java-like component to compare your component against in tests.

That is why we created J2N. If you like this idea, please be sure to star our repository on GitHub.

### Our Focus

1. **Text analysis:** code points, normalizing behaviors between different "character sequence" types, tokenizing, etc.
2. **I/O:** Reading and writing types in both big-endian and little-endian byte order and providing specialized behaviors for interop with Java-centric file formats.
3. **Collections:** .NET's cupboard is a little bare when it comes to specialized collections, so we fill in some gaps.
4. **Equality:** Compare collections for structural equality with behaviors that are specific to each collection family, and provide .NET equality comparers for other types that differ in behavior.
5. **Localization:** Bridge the gap between .NET's culture-aware and Java's culture-neutral defaults.

## NuGet

```
Install-Package J2N
```

## Contributing

We love getting contributions! If you need something from the JDK that we don't have, this is the right place to submit it. Basically, the following are things that would be a good fit for this library:

1. Components in the JDK that have no direct counterpart in .NET, or the counterpart is lacking features
2. Features that make J2N easier to work with in .NET such as extension methods and adapters
3. Features that make .NET interoperate with Java better

## Building and Testing

To build the project from source, see the [Building and Testing documentation](https://github.com/NightOwl888/J2N/blob/main/docs/building-and-testing.md).

## Saying Thanks

If you find this library to be useful, please star us on GitHub and consider a sponsorship so we can continue bringing you great free tools like this one.

[![GitHub Sponsors](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/NightOwl888)