namespace PreciseCalc;

/// <summary>
/// Exception thrown when a precision overflow occurs.
/// </summary>
public class PrecisionOverflowException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    public PrecisionOverflowException() : base("Precision overflow occurred.")
    {
    }
}