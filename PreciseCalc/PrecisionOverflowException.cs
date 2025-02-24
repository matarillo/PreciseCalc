namespace PreciseCalc;

public class PrecisionOverflowException : Exception
{
    public PrecisionOverflowException() : base("Precision overflow occurred.")
    {
    }
}