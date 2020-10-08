# Caliburn.Micro.ReactiveUI

## Goal

This project aims to simplify integration of [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro) and [ReactiveUI](https://github.com/reactiveui/ReactiveUI), using Caliburn.Micro's conventions and screen management with ReactiveUI's asynchronous features.

## Structure

The project contains the default screens and conductors provided by Caliburn.Micro that have been rewritten to inherit from `ReactiveObject`.  
The name of these classes matches their Caliburn.Micro counterpart, prefixed with Reactive.

## Targeting

- .NET Framework 4.6.2
- Caliburn.Micro 3.2.0
- ReactiveUI 9.12.1
