namespace PreciseCalc;

/// <summary>
/// Simple helper class to hold summary statistics for the contents of an expression.
/// Sometimes useful for comparing the complexity of equivalent expressions.
/// </summary>
/// <remarks>
/// The bit length of a constant is the bit length of its normalized
/// rational or integer representation.
/// Operator counts include pi and e constants.
/// PreEvals count as constants of length 100 appearing as arguments to "interesting"
/// functions.
/// </remarks>
internal class ExprStats
{
    /// <summary>
    /// Total length in bits of all constants.
    /// </summary>
    public int TotalConstBitLength;

    /// <summary>
    /// Total len of constants appearing as sqrt etc. args.
    /// </summary>
    public int InterestingConstBitLength;

    /// <summary>
    /// Total # of operators.
    /// </summary>
    public int NOps;

    /// <summary>
    /// Number of trig, exponential, log function invocations.
    /// </summary>
    public int TranscendentalFns;

    /// <summary>
    /// Total # of +, -, * operators, pi and e constants.
    /// </summary>
    public int NCommonOps;

    /// <summary>
    /// Total number of decimal points or exponents in constants.
    /// </summary>
    public int NDecimals;

    /// <summary>
    /// Total # of uncommon operations.
    /// </summary>
    public int NUncommonOps => NOps - NCommonOps;

    /// <summary>
    /// Add the stats from another ExprStats object to this one.
    /// </summary>
    /// <param name="other"></param>
    public void Add(ExprStats other)
    {
        NCommonOps += other.NCommonOps;
        NOps += other.NOps;
        TranscendentalFns += other.TranscendentalFns;
        TotalConstBitLength += other.TotalConstBitLength;
        InterestingConstBitLength += other.InterestingConstBitLength;
        NDecimals += other.NDecimals;
    }
}