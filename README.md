interlook-commons
=================

Another library of daily basis tools, one could wish to be included in .NET


Supporting .Net Core 2.2, .NET 4.5.1 and .NET 4.7.2

## Description

This package provides several helper classes and extension methods, which try to support general to specific tasks.

It contains:

- Collections
  - `CompositeObservableEnumerable` - an observable sequence, that automatically merges several observable sequences
  - `ObservableDictionary` - an observable generic dictionary
  - `NullEntryDictionary` - a generic dictionary with an optional default value, whose key is `null`
  - `NullEntryObservableDictionary` - a combination of the two above
- versatile components
  - `ChainOfResponsibility<>` and `AsyncChainOfResponsibility<>` - dispatching objects for items, to be processed by different handlers, depending on their status
  - `DelegateComparer` - an implementation for `IEqualityComparer<T>` using delegates
  - `DisposableToken` - base class for objects implementing `IDisposable` to easily perform actions when disposing
  - `DelegateDisposableToken` - helper object for actions to execute when leaving an `using` block
  - `MethodResult` and `MethodResult<T>` - classes for return values of methods supporting status
- Eventing
    - `EventBus` - implements right this, an event bus, offering methods for subcribing and publishing events, supporting filtering, subscription tokens and UI context
    - interfaces and classes to customize/extend `EventBus`
- LINQ and Functional Programming
  - `FunctionalExtensions` - extension methods for currying and partial applying functions/actions
  - `DictionaryExtensions` - helper methods for generic dictionaries (e.g. getting values safely)
  - `EnumerationExtensions` - helper methods for enumerators and Linq2Objects
  - Classical monads from FP, namely `Maybe`,`Either`,`Try` as well as their lazy versions `MaybeLazy`,`EitherLazy` and `TryLazy`. Notice, that the lazy types are often the counterparts of the native pendants in functional languages (Haskell, F# etc.).
  - Additionally: `Reader`, `Writer` and `State`, which are lazy by default.
- String and text handling
  - checking and comparing string
  - constraining strings
  - encrypt/decrypt text
  - applying (crypto-)hash functions to text
  - multi-strings (indexed alternate contents)
  - parsing strings
  - secure string equality check mitigating timing-attacks
 

 