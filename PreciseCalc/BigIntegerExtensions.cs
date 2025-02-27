using System.Numerics;
using System.Text;

namespace PreciseCalc;

/// <summary>
/// Provides extension methods for <see cref="BigInteger"/> to support additional functionality.
/// This includes base conversion and integer square root calculation, which are available in Java's BigInteger API
/// but missing in .NET's <see cref="BigInteger"/> class.
/// </summary>
internal static class BigIntegerExtensions
{
    /// <summary>
    /// Converts a <see cref="BigInteger"/> to a string representation in the specified radix (base).
    /// Supports bases from 2 to 36.
    /// This method provides similar functionality to Java's `BigInteger#toString(int radix)`.
    /// </summary>
    /// <param name="value">The BigInteger value to convert.</param>
    /// <param name="radix">The base (radix) of the output string, ranging from 2 to 36.</param>
    /// <returns>A string representation of the value in the given base.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the radix is not between 2 and 36.</exception>
    public static string ToString(this BigInteger value, int radix)
    {
        if (radix is < 2 or > 36)
            throw new ArgumentOutOfRangeException(nameof(radix), "Radix must be between 2 and 36.");

        if (value.IsZero)
            return "0";

        bool isNegative = value.Sign < 0;
        value = BigInteger.Abs(value);
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = new StringBuilder();

        while (value > 0)
        {
            result.Insert(0, chars[(int)(value % radix)]);
            value /= radix;
        }

        if (isNegative)
        {
            result.Insert(0, '-');
        }

        return result.ToString();
    }
}