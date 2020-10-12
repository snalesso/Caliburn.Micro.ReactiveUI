# Caliburn.Micro.ReactiveUI

## Goal

This project aims to simplify integration of [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro) and [ReactiveUI](https://github.com/reactiveui/ReactiveUI).

## Structure

The project contains Caliburn.Micro's main `IScreen` and `IConductor` implementations, exactly as they are, with the difference of deriving from ReactiveUI's [`ReactiveObject`](https://github.com/reactiveui/ReactiveUI/blob/main/src/ReactiveUI/ReactiveObject/ReactiveObject.cs) instead of Caliburn.Micro's [`PropertyChangedBase`](https://github.com/Caliburn-Micro/Caliburn.Micro/blob/master/src/Caliburn.Micro.Core/PropertyChangedBase.cs).
Classes' names match their Caliburn.Micro counterpart, prefixed with "Reactive" (e.g. [`Screen`](https://github.com/Caliburn-Micro/Caliburn.Micro/blob/master/src/Caliburn.Micro.Core/Screen.cs) becomes [`ReactiveScreen`](https://github.com/snalesso/Caliburn.Micro.ReactiveUI/blob/master/src/Caliburn.Micro.ReactiveUI/ReactiveScreen.cs)).

## Targeting

- .NET Core 3.1
  - Caliburn.Micro 4.0.136-rc
  - ReactiveUI 11.5.35
  
- .NET Framework 4.6.1
  - Caliburn.Micro 4.0.136-rc
  - ReactiveUI 11.5.35
