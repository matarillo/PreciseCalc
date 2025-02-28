namespace PreciseCalc;

/// <summary>
/// Result is too big
/// </summary>
public class TooBigException : ArithmeticException
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="s"></param>
    public TooBigException(string s) : base(s)
    {
    }
}