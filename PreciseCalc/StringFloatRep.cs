﻿namespace PreciseCalc;

/// <summary>
/// A scientific notation representation of an approximation to a constructive real.
/// Generated by CR.ToStringFloatRep.
/// </summary>
public sealed class StringFloatRep
{
    /// <summary>
    /// The sign associated with this approximation. May be -1, 1, or 0.
    /// </summary>
    public int Sign { get; }

    /// <summary>
    /// A string representation of the mantissa. The decimal point is implicitly
    /// to the left of the string of digits, and is not explicitly represented.
    /// </summary>
    public string Mantissa { get; }

    /// <summary>
    /// The radix of the representation. Also the base of the exponent field.
    /// </summary>
    public int Radix { get; }

    /// <summary>
    /// The mantissa is scaled by radix^exponent.
    /// </summary>
    public int Exponent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringFloatRep"/> class.
    /// </summary>
    public StringFloatRep(int sign, string mantissa, int radix, int exponent)
    {
        Sign = sign;
        Mantissa = mantissa ?? throw new ArgumentNullException(nameof(mantissa));
        Radix = radix;
        Exponent = exponent;
    }

    /// <summary>
    /// Produce a textual representation including the sign and exponent.
    /// </summary>
    public override string ToString() => $"{(Sign < 0 ? "-" : "")}{Mantissa}E{Exponent}{(Radix == 10 ? "" : $"(radix {Radix})")}";
}