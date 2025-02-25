namespace PreciseCalc;

/// <summary>
/// Argument out of domain
/// </summary>
public class DomainException : ArithmeticException
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    public DomainException(string message) : base(message)
    {
    }
}