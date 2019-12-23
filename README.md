J2N - Java-like Components for .NET
=========

J2N is a library that helps bridge the gap between .NET and Java.

### Our Goals

* Java-like behaviors
* .NET-like APIs
* Be the defacto library to use when porting from Java to .NET
* Provide high quality, high performance components that can be used in a wide range of .NET applications

Basically, if you are looking for a "JDK.NET", this is about as close as you can get. While we recommend using purely .NET components where possible when porting from Java, there are some Java features that have no .NET counterpart or the .NET counterpart is lacking behaviors that are not easy to reproduce without reinventing the wheel. Even if you prefer to reinvent the wheel by designing your own ".NETified" component, you may still need a Java-like component to compare your component against in tests.

That is why we created J2N.

### Our Focus

1. **Text analysis:** code points, normalizing behaviors between different "character sequence" types, tokenizing, etc.
2. **I/O:** Reading and writing types in both big-endian and little-endian format and providing specialized behaviors for interop with Java-centric file formats.
3. **Collections:** .NET's cupboard is a little bare when it comes to specialized collections, so we fill in some gaps.
4. **Equality:** Compare collections for structural equality with behaviors that are specific to each collection family, and provide .NET equality comparers for other types that differ in behavior.
5. **Localization:** Bridge the gap between .NET's culture-aware and Java's culture-neutral defaults.


### Status: Beta

Much of what is here has been tested pretty thoroughly already and the APIs are pretty stable, however, we are still adding additional components and getting it ready for use in production.

## NuGet

```
Install-Package J2N -Pre
```

### Contributing

We love getting contributions! If you need something from the JDK that we don't have, this is the right place to submit it. Basically, the following are things that would be a good fit for this library:

1. Components in the JDK that have no direct counterpart in .NET, or the counterpart is lacking features
2. Features that make J2N easier to work with in .NET such as extension methods and adapters
3. Features that make .NET interoperate with Java better