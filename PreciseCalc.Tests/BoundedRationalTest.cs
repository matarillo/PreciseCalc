using System.Numerics;

namespace PreciseCalc.Tests;

public class BoundedRationalTest
{
    // Test Case 0: Just for coverage
    [Fact]
    public void SimpleTest()
    {
        Assert.Equal(BoundedRational.MinusTwo, BoundedRational.FromLong(-2L));
        Assert.Equal(BoundedRational.MinusOne, BoundedRational.FromLong(-1L));
        Assert.Equal(BoundedRational.Zero, BoundedRational.FromLong(0L));
        Assert.Equal(BoundedRational.One, BoundedRational.FromLong(1L));
        Assert.Equal(BoundedRational.Two, BoundedRational.FromLong(2L));
        Assert.Equal(BoundedRational.Ten, BoundedRational.FromLong(10L));

        object target = BoundedRational.One;
        Assert.True(target.Equals(BoundedRational.One));
        Assert.False(target.Equals(""));

        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ToDouble());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ToConstructiveReal());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ToInt32());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ApproxLog2Abs());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.Floor());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ExtractSquareReduced());
    }
    
    // Test Case 1: Basic Constructors and Factory Methods
    [Fact]
    public void ConstructorAndFactoryMethodsTest()
    {
        // Default constructor creates an invalid rational number
        var nullRational = new BoundedRational();
        Assert.True(nullRational.IsNull);
        Assert.Equal(BoundedRational.Null, nullRational);

        // Integer constructor
        var intRational = new BoundedRational(5);
        Assert.False(intRational.IsNull);
        var (iNum, iDen) = intRational.NumDen;
        Assert.Equal(new BigInteger(5), iNum);
        Assert.Equal(BigInteger.One, iDen);

        // Fraction constructor
        var fracRational = new BoundedRational(3, 4);
        Assert.False(fracRational.IsNull);
        var (fNum, fDen) = fracRational.NumDen;
        Assert.Equal(new BigInteger(3), fNum);
        Assert.Equal(new BigInteger(4), fDen);

        // BigInteger constructor
        var bigIntNum = new BigInteger(123456789);
        var bigIntDen = new BigInteger(987654321);
        var bigIntRational = new BoundedRational(bigIntNum, bigIntDen);
        Assert.False(bigIntRational.IsNull);
        var (biNum, biDen) = bigIntRational.NumDen;
        Assert.Equal(new BigInteger(13717421), biNum);
        Assert.Equal(new BigInteger(109739369), biDen);

        // FromDouble method
        var doubleRational = BoundedRational.FromDouble(3.125); // 25/8
        Assert.False(doubleRational.IsNull);
        var (numerator, denominator) = doubleRational.NumDen;
        Assert.Equal(new BigInteger(25), numerator);
        Assert.Equal(new BigInteger(8), denominator);

        // FromLong method
        var longRational = BoundedRational.FromLong(10);
        Assert.Equal(BoundedRational.Ten, longRational);

        // Special edge cases
        var fromDoubleZero = BoundedRational.FromDouble(0.0);
        Assert.Equal(BoundedRational.Zero, fromDoubleZero);

        // Exception tests
        Assert.Throws<ArithmeticException>(() => BoundedRational.FromDouble(double.PositiveInfinity));
        Assert.Throws<ArithmeticException>(() => BoundedRational.FromDouble(double.NaN));
    }

    // Test Case 2: Constants and Properties
    [Fact]
    public void ConstantsAndPropertiesTest()
    {
        // Test constants
        var (num0, den0) = BoundedRational.Zero.NumDen;
        Assert.Equal(new BigInteger(0), num0);
        Assert.Equal(BigInteger.One, den0);

        var (num1, den1) = BoundedRational.One.NumDen;
        Assert.Equal(new BigInteger(1), num1);
        Assert.Equal(BigInteger.One, den1);

        var (numHalf, denHalf) = BoundedRational.Half.NumDen;
        Assert.Equal(new BigInteger(1), numHalf);
        Assert.Equal(new BigInteger(2), denHalf);

        // Test properties
        var twoThirds = new BoundedRational(2, 3);

        // Sign property
        Assert.Equal(1, twoThirds.Sign);
        Assert.Equal(-1, (-twoThirds).Sign);
        Assert.Equal(0, BoundedRational.Zero.Sign);

        // NumDen property
        var (num, den) = twoThirds.NumDen;
        Assert.Equal(new BigInteger(2), num);
        Assert.Equal(new BigInteger(3), den);

        // BitLength property
        Assert.Equal(4, twoThirds.BitLength); // 2 and 3 are both 2 bits, so total is 4 bits

        // DigitsRequired property
        Assert.Equal(0, BoundedRational.One.DigitsRequired);
        Assert.Equal(1, BoundedRational.Half.DigitsRequired);
        Assert.Equal(int.MaxValue,
            new BoundedRational(1, 3).DigitsRequired); // 1/3 cannot be represented as a finite decimal

        // IsNull property
        Assert.True(BoundedRational.Null.IsNull);
        Assert.False(twoThirds.IsNull);

        // WholeNumberBits property
        Assert.Equal(-1, BoundedRational.Half.WholeNumberBits); // 1/2 is 0.1 in binary, so 0 whole number bits
        Assert.Equal(1, BoundedRational.Two.WholeNumberBits); // 2 is 10 in binary, so 2 whole number bits

        // Property access on invalid rational
        var nullRational = new BoundedRational();
        Assert.Throws<InvalidOperationException>(() => nullRational.Sign);
        Assert.Throws<InvalidOperationException>(() => nullRational.NumDen);
        Assert.Throws<InvalidOperationException>(() => nullRational.BitLength);
        Assert.Throws<InvalidOperationException>(() => nullRational.WholeNumberBits);
    }

    // Test Case 3: String Conversion and Display
    [Fact]
    public void StringConversionTest()
    {
        // ToString
        Assert.Equal("3/4", new BoundedRational(3, 4).ToString());
        Assert.Equal("Null", BoundedRational.Null.ToString());

        // ToDisplayString
        var threeQuarters = new BoundedRational(3, 4);
        Assert.Equal("3/4", threeQuarters.ToDisplayString(false, false));

        // Negative rational
        var negativeRational = new BoundedRational(-5, 8);
        Assert.Equal("-5/8", negativeRational.ToString());
        Assert.Equal("-5/8", negativeRational.ToDisplayString());

        // Mixed number display
        var improperFraction = new BoundedRational(11, 4);
        Assert.Equal("11/4", improperFraction.ToString());
        Assert.Equal("2 3/4", improperFraction.ToDisplayString(false, true));

        // Unicode fractions
        Assert.Contains("⁄", threeQuarters.ToDisplayString(true, false)); // Unicode fraction slash

        // Truncated string representation
        Assert.Equal("0.75", threeQuarters.ToStringTruncated(2));
        Assert.Equal("0.750", threeQuarters.ToStringTruncated(3));
        Assert.Equal("-0.62", negativeRational.ToStringTruncated(2));
    }

    // Test Case 4: Type Conversion Methods
    [Fact]
    public void TypeConversionTest()
    {
        // ToDouble
        Assert.Equal(0.75, new BoundedRational(3, 4).ToDouble());
        Assert.Equal(0.0, BoundedRational.Zero.ToDouble());
        Assert.Equal(-2.5, new BoundedRational(-5, 2).ToDouble());

        // Very small values
        var verySmall = new BoundedRational(1, BigInteger.Pow(2, 1200));
        Assert.Equal(0.0, verySmall.ToDouble()); // Returns 0.0 as it exceeds double precision

        // ToInt32
        Assert.Equal(5, new BoundedRational(5).ToInt32());
        Assert.Throws<ArithmeticException>(() => new BoundedRational(5, 2).ToInt32()); // Error for non-integer values

        // ToBigInteger
        Assert.Equal(new BigInteger(7), new BoundedRational(7).ToBigInteger());
        Assert.Null(new BoundedRational(7, 3).ToBigInteger()); // Null for non-integer values

        // ApproxLog2Abs
        Assert.Equal(1.0, BoundedRational.Two.ApproxLog2Abs(), 0.001);
        Assert.Equal(-1.0, BoundedRational.Half.ApproxLog2Abs(), 0.001);
        Assert.Equal(int.MinValue, BoundedRational.Zero.ApproxLog2Abs(), 0.001);
        Assert.Equal(0.585, new BoundedRational(3, 2).ApproxLog2Abs(), 0.001); // log2(1.5) ≈ 0.585

        // Conversion with invalid rationals
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ToDouble());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ToInt32());
        Assert.Null(BoundedRational.Null.ToBigInteger());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.ApproxLog2Abs());
    }

    // Test Case 5: Basic Arithmetic Operations
    [Fact]
    public void BasicArithmeticOperationsTest()
    {
        var a = new BoundedRational(1, 2); // 1/2
        var b = new BoundedRational(1, 3); // 1/3

        // Addition
        var sum = a + b;
        Assert.Equal(new BoundedRational(5, 6), sum); // 1/2 + 1/3 = 5/6

        // Subtraction
        var diff = a - b;
        Assert.Equal(new BoundedRational(1, 6), diff); // 1/2 - 1/3 = 1/6

        // Multiplication
        var prod = a * b;
        Assert.Equal(new BoundedRational(1, 6), prod); // 1/2 * 1/3 = 1/6

        // Division
        var quot = a / b;
        Assert.Equal(new BoundedRational(3, 2), quot); // 1/2 ÷ 1/3 = 3/2

        // Operations with negative numbers
        var negA = -a; // -1/2
        Assert.Equal(new BoundedRational(-1, 2), negA);
        Assert.Equal(new BoundedRational(-1, 6), negA + b); // -1/2 + 1/3 = -1/6

        // Operations with zero
        Assert.Equal(a, a + BoundedRational.Zero);
        Assert.Equal(a, a - BoundedRational.Zero);
        Assert.Equal(BoundedRational.Zero, a * BoundedRational.Zero);
        Assert.Equal(BoundedRational.Zero, BoundedRational.Zero / a);

        // Operations with one
        Assert.Equal(a, a * BoundedRational.One);
        Assert.Equal(a, a / BoundedRational.One);

        // Division by zero
        Assert.Throws<DivideByZeroException>(() => a / BoundedRational.Zero);

        // Operations with Null
        Assert.Equal(BoundedRational.Null, a + BoundedRational.Null);
        Assert.Equal(BoundedRational.Null, a - BoundedRational.Null);
        Assert.Equal(BoundedRational.Null, a * BoundedRational.Null);
        Assert.Equal(BoundedRational.Null, a / BoundedRational.Null);
    }

    // Test Case 6: Advanced Arithmetic Operations
    [Fact]
    public void AdvancedArithmeticOperationsTest()
    {
        // Inverse
        var third = new BoundedRational(1, 3);
        var invThird = BoundedRational.Inverse(third);
        Assert.Equal(new BoundedRational(3, 1), invThird);
        Assert.Throws<DivideByZeroException>(() => BoundedRational.Inverse(BoundedRational.Zero));
        Assert.Equal(BoundedRational.Null, BoundedRational.Inverse(BoundedRational.Null));

        // Floor
        Assert.Equal(new BigInteger(2), new BoundedRational(5, 2).Floor());
        Assert.Equal(new BigInteger(-3), new BoundedRational(-5, 2).Floor());
        Assert.Equal(new BigInteger(0), new BoundedRational(1, 3).Floor());
        Assert.Throws<InvalidOperationException>(() => BoundedRational.Null.Floor());

        // NthRoot
        var four = new BoundedRational(4);
        Assert.Equal(new BoundedRational(2), BoundedRational.NthRoot(four, 2)); // √4 = 2
        Assert.Equal(BoundedRational.Null, BoundedRational.NthRoot(new BoundedRational(2), 2)); // √2 is irrational

        // Square root of negative numbers
        Assert.Throws<ArithmeticException>(() =>
            BoundedRational.NthRoot(new BoundedRational(-4), 2)); // √-4 is imaginary

        // Odd roots
        Assert.Equal(new BoundedRational(-2), BoundedRational.NthRoot(new BoundedRational(-8), 3)); // ∛(-8) = -2

        // Fractional roots
        var fourNinth = new BoundedRational(4, 9);
        Assert.Equal(new BoundedRational(2, 3), BoundedRational.NthRoot(fourNinth, 2)); // √(4/9) = 2/3

        // Inverse roots (negative exponents)
        Assert.Equal(new BoundedRational(1, 2), BoundedRational.NthRoot(new BoundedRational(4), -2)); // 4^(-1/2) = 1/2

        // Sqrt alias
        Assert.Equal(new BoundedRational(3), BoundedRational.Sqrt(new BoundedRational(9)));
        Assert.Equal(BoundedRational.Null, BoundedRational.Sqrt(new BoundedRational(2)));

        // ExtractSquareReduced
        var r343Over352 = new BoundedRational(343, 352); // 343=7*7*7, 352=4*4*22
        var extractResult = r343Over352.ExtractSquareReduced();
        Assert.Equal(new BoundedRational(7, 4), extractResult[0]); // 7/4
        Assert.Equal(new BoundedRational(7, 22), extractResult[1]); // 7/22

        // Non-square numbers
        var tenOverSeven = new BoundedRational(10, 7);
        extractResult = tenOverSeven.ExtractSquareReduced();
        Assert.Equal(BoundedRational.One, extractResult[0]);
        Assert.Equal(tenOverSeven, extractResult[1]);

        // Integer powers
        Assert.Equal(new BoundedRational(8), new BoundedRational(2).Pow(new BigInteger(3))); // 2^3 = 8
        Assert.Equal(new BoundedRational(1, 8), new BoundedRational(2).Pow(new BigInteger(-3))); // 2^(-3) = 1/8
        Assert.Equal(BoundedRational.One, new BoundedRational(2).Pow(BigInteger.Zero)); // 2^0 = 1

        // Fractional powers
        Assert.Equal(new BoundedRational(8, 27), new BoundedRational(2, 3).Pow(new BigInteger(3))); // (2/3)^3 = 8/27

        // Rational exponents
        Assert.Equal(new BoundedRational(2),
            BoundedRational.Pow(new BoundedRational(4), new BoundedRational(1, 2))); // 4^(1/2) = 2
        Assert.Equal(new BoundedRational(3),
            BoundedRational.Pow(new BoundedRational(27), new BoundedRational(1, 3))); // 27^(1/3) = 3

        // Irrational results
        Assert.Equal(BoundedRational.Null,
            BoundedRational.Pow(new BoundedRational(2), new BoundedRational(1, 2))); // 2^(1/2) = √2 is irrational

        // Special cases
        Assert.Equal(BoundedRational.One,
            BoundedRational.Pow(BoundedRational.Zero, BoundedRational.Zero)); // 0^0 = 1 (mathematical convention)
        Assert.Equal(BoundedRational.Zero, BoundedRational.Pow(BoundedRational.Zero, BoundedRational.One)); // 0^1 = 0
        Assert.Equal(BoundedRational.One,
            BoundedRational.Pow(BoundedRational.One, new BoundedRational(1000))); // 1^1000 = 1
        Assert.Equal(BoundedRational.One,
            BoundedRational.Pow(new BoundedRational(-1), new BoundedRational(2))); // (-1)^2 = 1
        Assert.Equal(BoundedRational.MinusOne,
            BoundedRational.Pow(new BoundedRational(-1), new BoundedRational(3))); // (-1)^3 = -1
    }


    // Test Case 7: Comparison and Equality Tests
    [Fact]
    public void ComparisonAndEqualityTest()
    {
        var a = new BoundedRational(3, 4);
        var b = new BoundedRational(6, 8); // Equivalent to 3/4
        var c = new BoundedRational(2, 3);
        var d = BoundedRational.Null;
        var e = BoundedRational.Null;

        // Equals
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.True(d.Equals(e)); // Null equals Null
        Assert.False(a.Equals(d));

        // Operator ==
        Assert.True(a == b);
        Assert.False(a == c);
        Assert.True(d == e);
        Assert.False(a == d);

        // Operator !=
        Assert.False(a != b);
        Assert.True(a != c);
        Assert.False(d != e);
        Assert.True(a != d);

        // CompareTo
        Assert.Equal(0, a.CompareTo(b));
        Assert.True(a.CompareTo(c) > 0); // 3/4 > 2/3
        Assert.True(c.CompareTo(a) < 0); // 2/3 < 3/4

        // Comparison with Null
        Assert.True(a.CompareTo(d) > 0); // Valid value > Null
        Assert.True(d.CompareTo(a) < 0); // Null < Valid value
        Assert.Equal(0, d.CompareTo(e)); // Null == Null

        // CompareToOne
        Assert.True(new BoundedRational(3, 2).CompareToOne() > 0); // 3/2 > 1
        Assert.True(new BoundedRational(1, 2).CompareToOne() < 0); // 1/2 < 1
        Assert.Equal(0, BoundedRational.One.CompareToOne()); // 1 == 1
        Assert.True(BoundedRational.Null.CompareToOne() < 0); // Null < 1

        // GetHashCode
        Assert.Equal(a.GetHashCode(), b.GetHashCode()); // Equivalent rationals should have the same hash code
        Assert.NotEqual(a.GetHashCode(), c.GetHashCode()); // Different rationals should have different hash codes
        Assert.Equal(0, d.GetHashCode()); // Null should have hash code 0
    }

    // Test Case 8: Edge Cases and Large Numbers
    [Fact]
    public void BoundaryAndLargeNumbersTest()
    {
        // Creating very large numbers
        var largeDenominator = BigInteger.Pow(2, 5000);
        var largeRational = new BoundedRational(1, largeDenominator);
        Assert.False(largeRational.IsNull);

        // Very large calculation results automatically become Null
        var a = new BoundedRational(BigInteger.Pow(2, 5000), 1);
        var b = new BoundedRational(BigInteger.Pow(2, 5001), 1);
        var pow = BoundedRational.Pow(a, b);
        Assert.True(pow.IsNull); // Result is too large

        // Reduction can make large values manageable
        var num = BigInteger.Pow(2, 5000);
        var c = new BoundedRational(num, num);
        var reduced = c.Reduce();
        Assert.Equal(BoundedRational.One, reduced); // n/n = 1

        // Still too large even after reduction
        var d = new BoundedRational(BigInteger.Pow(3, 4000), BigInteger.Pow(2, 4000));
        var result = d * d; // Results in a very large number
        Assert.True(result.IsNull);

        // Special case: Division by zero
        Assert.Throws<DivideByZeroException>(() => BoundedRational.One / BoundedRational.Zero);

        // Special case: Operations with Null
        Assert.Equal(BoundedRational.Null, BoundedRational.One + BoundedRational.Null);

        // DigitsRequired for large denominator
        var oneOverTen = new BoundedRational(1, 10);
        Assert.Equal(1, oneOverTen.DigitsRequired);

        // DigitsRequired for complex fraction
        var complexFraction = new BoundedRational(1, 7);
        Assert.Equal(int.MaxValue, complexFraction.DigitsRequired); // 1/7 is a repeating decimal

        // Special cases with zero
        Assert.Equal(new BigInteger(0), BoundedRational.Zero.Floor());
        Assert.Equal(BoundedRational.Zero, BoundedRational.NthRoot(BoundedRational.Zero, 2));
        Assert.Equal(BoundedRational.One, BoundedRational.Pow(BoundedRational.Zero, BoundedRational.Zero)); // 0^0 = 1
    }

    // Test Case 9: Combined Operations Tests
    [Fact]
    public void CompleteOperationsTest()
    {
        // Test with combination of multiple operations
        var a = new BoundedRational(1, 2);
        var b = new BoundedRational(2, 3);
        var c = new BoundedRational(3, 4);

        // a + b * c = 1/2 + 2/3 * 3/4 = 1/2 + 1/2 = 1
        var result = a + (b * c);
        Assert.Equal(BoundedRational.One, result);

        // (a + b) / (a - b) = (1/2 + 2/3) / (1/2 - 2/3) = 7/6 / (-1/6) = -7
        result = (a + b) / (a - b);
        Assert.Equal(new BoundedRational(-7), result);

        // Complex calculation with square roots
        var four = new BoundedRational(4);
        var nine = new BoundedRational(9);
        // √4 * √9 = 2 * 3 = 6
        result = BoundedRational.Sqrt(four) * BoundedRational.Sqrt(nine);
        Assert.Equal(new BoundedRational(6), result);

        // With powers: (2/3)^2 * (3/4)^2 = 4/9 * 9/16 = 1/4
        result = BoundedRational.Pow(b, BoundedRational.Two) * BoundedRational.Pow(c, BoundedRational.Two);
        Assert.Equal(new BoundedRational(1, 4), result);

        // Floor and division: Floor(7/3) / 2 = 2 / 2 = 1
        var sevenThirds = new BoundedRational(7, 3);
        result = new BoundedRational(sevenThirds.Floor()) / new BoundedRational(2);
        Assert.Equal(BoundedRational.One, result);

        // Using rationals for calculator-like operations
        var num1 = new BoundedRational(22, 7); // Approximation of π
        var num2 = BoundedRational.FromDouble(0.5); // 1/2

        // π * r^2 for circle area calculation
        var radius = new BoundedRational(10); // Radius 10
        var radiusSquared = radius * radius;
        var area = num1 * radiusSquared;

        // Verification: π is approximately 3.14, so area should be around 314
        var doubleArea = area.ToDouble();
        Assert.InRange(doubleArea, 313.0, 315.0);

        // Complex expression: (1 + 1/n)^n approximating e as n approaches infinity
        var n = new BoundedRational(10); // Using n = 10
        var expression = BoundedRational.Pow(BoundedRational.One + BoundedRational.Inverse(n), n);
        Assert.InRange(expression.ToDouble(), 2.5, 2.6); // Approximation of e ≈ 2.718...
    }

    // Test Case 10: Special Operations and Exceptions
    [Fact]
    public void SpecialOperationsAndExceptionsTest()
    {
        // Special cases for string conversion
        Assert.Equal("0", BoundedRational.Zero.ToDisplayString());
        Assert.Equal("-1", BoundedRational.MinusOne.ToDisplayString());

        // Addition with different denominators
        var result = new BoundedRational(1, 6) + new BoundedRational(1, 4);
        Assert.Equal(new BoundedRational(5, 12), result);

        // Integer conversion
        Assert.Equal(5, new BoundedRational(5, 1).ToInt32());
        Assert.Throws<ArithmeticException>(() => new BoundedRational(5, 2).ToInt32());

        // ToConstructiveReal
        var cr = new BoundedRational(3, 4).ToConstructiveReal();
        // Depends on ConstructiveReal implementation, but verify the value is correct
        var approx = cr.GetApproximation(-10); // Precision 2^-10
        Assert.InRange(approx, 767, 768); // 3/4 * 2^10 ≈ 768

        // ExtractSquareReduced edge cases
        var extractResult = BoundedRational.Zero.ExtractSquareReduced();
        Assert.Equal(BoundedRational.Zero, extractResult[0]);
        Assert.Equal(BoundedRational.One, extractResult[1]);

        // Non-trivial ExtractSquareReduced case
        var twelve = new BoundedRational(12);
        extractResult = twelve.ExtractSquareReduced();
        Assert.Equal(new BoundedRational(2), extractResult[0]); // 12 = 2^2 * 3
        Assert.Equal(new BoundedRational(3), extractResult[1]);

        // ExtractSquareWillSucceed
        Assert.True(new BoundedRational(100).ExtractSquareWillSucceed());

        // ExtractSquareWillSucceed for very large numbers
        var largeNumber = new BoundedRational(BigInteger.Pow(10, 2000));
        Assert.False(largeNumber.ExtractSquareWillSucceed());

        // Operations with Null always return Null
        Assert.Equal(BoundedRational.Null, BoundedRational.NthRoot(BoundedRational.Null, 2));
        Assert.Equal(BoundedRational.Null, BoundedRational.Inverse(BoundedRational.Null));
        Assert.Equal(BoundedRational.Null, BoundedRational.Pow(BoundedRational.Null, BoundedRational.One));
        Assert.Equal(BoundedRational.Null, BoundedRational.Pow(BoundedRational.One, BoundedRational.Null));
    }

    // Test Case 11: Edge Cases and Equivalence Partitioning
    [Fact]
    public void EdgeCasesAndEquivalencePartitioningTest()
    {
        // Edge cases with zero
        var zero = BoundedRational.Zero;
        Assert.Equal(0, zero.Sign);
        Assert.Equal(BoundedRational.Zero, zero * new BoundedRational(100));
        Assert.Equal(BoundedRational.Zero, zero / new BoundedRational(100));
        Assert.Throws<DivideByZeroException>(() => new BoundedRational(100) / zero);

        // Edge cases with negative values
        var negativeHalf = BoundedRational.MinusHalf;
        Assert.Equal(-1, negativeHalf.Sign);
        Assert.Equal(BoundedRational.MinusHalf, negativeHalf * BoundedRational.One);
        Assert.Equal(BoundedRational.MinusTwo, BoundedRational.One / negativeHalf);

        // Edge cases with one
        var one = BoundedRational.One;
        Assert.Equal(0, one.CompareToOne());
        // 1 * any = any
        Assert.Equal(BoundedRational.Half, one * BoundedRational.Half);
        Assert.Equal(BoundedRational.Two, one * BoundedRational.Two);

        // Case with negative denominator
        var explicitNegativeDenominator = new BoundedRational(5, -10);
        var normalized = explicitNegativeDenominator.Reduce();
        Assert.Equal(new BoundedRational(-1, 2), normalized);

        // Normalization to standard form (reduced fraction)
        var reducible = new BoundedRational(10, 15);
        var reduced = reducible.Reduce();
        Assert.Equal(new BoundedRational(2, 3), reduced);

        // Comparison with invalid rationals
        Assert.Equal(0, BoundedRational.Null.CompareTo(BoundedRational.Null));
        Assert.True(BoundedRational.One.CompareTo(BoundedRational.Null) > 0);
        Assert.True(BoundedRational.Null.CompareTo(BoundedRational.One) < 0);

        // FromDouble edge cases
        var nearIntegerDouble = 3.0000000001; // Very close to integer
        var rationalFromNearInt = BoundedRational.FromDouble(nearIntegerDouble);
        Assert.NotEqual(new BoundedRational(3), rationalFromNearInt); // clearly differentiated

        // For large integers, constructor always returns valid value
        var largeInt = new BoundedRational(BigInteger.Pow(10, 1000));
        Assert.False(largeInt.IsNull);

        // Very small value handling (underflow)
        var tinyValue = new BoundedRational(1, BigInteger.Pow(10, 1000));
        Assert.Equal(0.0, tinyValue.ToDouble()); // Becomes zero in floating point

        // Overflow testing
        var huge1 = new BoundedRational(BigInteger.Pow(10, 5000));
        var huge2 = new BoundedRational(BigInteger.Pow(10, 5001));
        Assert.False((huge1 * huge2).IsNull); // Result is too large, but still valid

        // Special integer cases
        Assert.Equal(BoundedRational.One,
            new BoundedRational(BigInteger.Pow(2, 100)) / new BoundedRational(BigInteger.Pow(2, 100)));
    }

    // Test Case 12: Performance Boundary Tests
    [Fact]
    public void PerformanceBoundaryTest()
    {
        // This test examines the design limits of the BoundedRational class
        // Note: This test may take significant time to execute

        // Maximum calculable values that don't exceed MaxSize
        var largeButValid1 = new BoundedRational(BigInteger.Pow(2, 4999));
        var largeButValid2 = new BoundedRational(BigInteger.Pow(2, 4999));
        var sum = largeButValid1 + largeButValid2;
        Assert.False(sum.IsNull); // Should be valid as it doesn't exceed MaxSize

        // Test with factorial calculation
        // 10! = 3628800 (small value)
        BoundedRational factorial = BoundedRational.One;
        for (int i = 1; i <= 10; i++)
        {
            factorial = factorial * new BoundedRational(i);
        }

        Assert.Equal(new BoundedRational(3628800), factorial);

        // Verify that extremely large powers return Null
        var big1 = new BoundedRational(BigInteger.Pow(2, 5001));
        var big2 = new BoundedRational(BigInteger.Pow(2, 5000));
        var bigPow = BoundedRational.Pow(big1, big2);
        Assert.True(bigPow.IsNull);

        // Test near ExtractSquareMaxLen boundary
        var largeSquareRoot = new BoundedRational(BigInteger.Pow(2, 4900));
        var extracted = largeSquareRoot.ExtractSquareReduced();
        Assert.False(extracted[0].IsNull);

        // Verify that exceeding ExtractSquareMaxLen doesn't work properly
        var tooLargeForExtract = new BoundedRational(BigInteger.Pow(2, 6000));
        Assert.False(tooLargeForExtract.ExtractSquareWillSucceed());
    }
}