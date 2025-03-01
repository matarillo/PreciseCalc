namespace PreciseCalc;

/// <summary>
///     Zero to the power of zero
/// </summary>
public class ZeroToTheZerothException : ArithmeticException
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message"></param>
    public ZeroToTheZerothException(string message) : base(message)
    {
    }
}