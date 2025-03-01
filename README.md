# PreciseCalc

PreciseCalc is a .NET port of the real arithmetic library originally implemented in Java by Hans-J. Boehm.
It enables arbitrary-precision real number calculations and provides specialized functionality
for precision management and comparison operations.

## Overview

This library is a .NET adaptation of Boehm's original Java implementation.
The original implementation includes the `UnifiedReal` real arithmetic package,
which is used in Google's Android calculator app, and is described in detail in the paper
"Towards an API for the real numbers" ([DOI](https://dl.acm.org/doi/10.1145/3385412.3386037)).

PreciseCalc provides the following features:
- Arbitrary-precision arithmetic using `ConstructiveReal`
- Comparable real number representation using `UnifiedReal`
- Floating-point precision testing functionality
- Appropriate exception handling (`DomainException`, `PrecisionOverflowException`, etc.)

## Installation

PreciseCalc is planned to be distributed via NuGet at [NuGet.org](https://www.nuget.org/packages/PreciseCalc/).
It can be installed using the following command:

```sh
 dotnet add package PreciseCalc
```

## Class List

### PreciseCalc Namespace

| Class                                                                                                        | Description                                                                             |
|--------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------|
| [`ConstructiveReal`](https://github.com/matarillo/PreciseCalc/blob/main/doc/PreciseCalc.ConstructiveReal.md) | Represents an arbitrary-precision constructive real number.                             |
| [`UnifiedReal`](https://github.com/matarillo/PreciseCalc/blob/main/doc/PreciseCalc.UnifiedReal.md)           | Represents computable real numbers with exact comparison capabilities in certain cases. |
| [`BoundedRational`](https://github.com/matarillo/PreciseCalc/blob/main/doc/PreciseCalc.BoundedRational.md)   | Represents bounded rational numbers that allow exact comparisons.                       |



## References

- [Original Java Implementation (Android Calculator Related)](https://android-review.googlesource.com/c/platform/art/+/1012109)
- [Related Paper (ACM)](https://dl.acm.org/doi/10.1145/3385412.3386037)
