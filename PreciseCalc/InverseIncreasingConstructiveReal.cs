using System.Diagnostics;
using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Nested class that computes the inverse function using an iterative approach.
/// </summary>
internal class InverseIncreasingConstructiveReal : ConstructiveReal
{
    private static readonly BigInteger Big1023 = new(1023);
    private readonly ConstructiveReal _arg;
    private readonly Data _data;

    /// <summary>
    /// Nested class that computes the inverse function using an iterative approach.
    /// </summary>
    public InverseIncreasingConstructiveReal(ConstructiveReal x, Data data)
    {
        this._arg = data.FNegated ? -x : x;
        this._data = data;
    }

    /// <summary>
    /// Compares two BigIntegers with a difference of one treated as equality.
    /// </summary>
    private static int SloppyCompare(BigInteger x, BigInteger y)
    {
        var difference = x - y;
        if (difference > Big1) return 1;
        if (difference < BigM1) return -1;
        return 0;
    }

    private protected override BigInteger Approximate(int precision)
    {
        const int extraArgPrec = 4;
        const int smallStepThreshold = 30;
        var fn = _data.F;
        var smallStepDeficit = 0; // Number of ineffective steps not yet compensated for by a binary search step

        var digitsNeeded = _data.MaxMsd - precision;
        if (digitsNeeded < 0) return Big0;

        var workingArgPrec = precision - extraArgPrec;
        if (workingArgPrec > _data.MaxArgPrec)
        {
            workingArgPrec = _data.MaxArgPrec;
        }

        var workingEvalPrec = workingArgPrec + _data.DerivMsd - 20; // Initial guess

        // We use a combination of binary search and something like
        // the secant method.  This always converges linearly,
        // and should converge quadratically under favorable assumptions.
        // fL and fH are always the approximate images of l and h.
        // At any point, arg is between fL and fH, or no more than
        // one outside [fL, fH].
        // L and h are implicitly scaled by workingArgPrec.
        // The scaled values of l and h are strictly between low and high.
        // If atLeft is true, then l is logically at the left
        // end of the interval.  We approximate this by setting l to
        // a point slightly inside the interval, and letting fL
        // approximate the function value at the endpoint.
        // If atRight is true, h and fH are set correspondingly.
        // At the endpoints of the interval, fL and fH may correspond
        // to the endpoints, even if l and h are slightly inside.
        // fL and fH are scaled by workingEvalPrec.
        // workingEvalPrec may need to be adjusted depending
        // on the derivative of f.
        bool atLeft;
        bool atRight;
        BigInteger l, fL;
        BigInteger h, fH;

        var lowAppr = _data.Low.GetApproximation(workingArgPrec) + Big1;
        var highAppr = _data.High.GetApproximation(workingArgPrec) - Big1;
        var argAppr = _arg.GetApproximation(workingEvalPrec);

        var haveGoodAppr = ApprValid && MinPrec < _data.MaxMsd;

        if (digitsNeeded < smallStepThreshold && !haveGoodAppr)
        {
            Data.Trace("Setting interval to entire domain");
            h = highAppr;
            fH = _data.FHigh.GetApproximation(workingEvalPrec);
            l = lowAppr;
            fL = _data.FLow.GetApproximation(workingEvalPrec);

            // Check for clear out-of-bounds case.
            // Close cases may fail in other ways.
            if (fH < argAppr - Big1 || fL > argAppr + Big1)
            {
                throw new ArithmeticException("Inverse: argument out of bounds");
            }

            atLeft = true;
            atRight = true;
            smallStepDeficit = 2; // Start with binary search steps
        }
        else
        {
            var roughPrec = precision + digitsNeeded / 2;

            if (haveGoodAppr && (digitsNeeded < smallStepThreshold || MinPrec < precision + 3 * digitsNeeded / 4))
            {
                roughPrec = MinPrec;
            }

            var roughAppr = GetApproximation(roughPrec);
            Data.Trace($"Setting interval based on prev. appr: {roughAppr}, precision: {roughPrec}");

            h = (roughAppr + Big1) << (roughPrec - workingArgPrec);
            l = (roughAppr - Big1) << (roughPrec - workingArgPrec);

            if (h > highAppr)
            {
                h = highAppr;
                fH = _data.FHigh.GetApproximation(workingEvalPrec);
                atRight = true;
            }
            else
            {
                var hCr = FromBigInteger(h) << workingArgPrec;
                fH = fn.Execute(hCr).GetApproximation(workingEvalPrec);
                atRight = false;
            }

            if (l < lowAppr)
            {
                l = lowAppr;
                fL = _data.FLow.GetApproximation(workingEvalPrec);
                atLeft = true;
            }
            else
            {
                var lCr = FromBigInteger(l) << workingArgPrec;
                fL = fn.Execute(lCr).GetApproximation(workingEvalPrec);
                atLeft = false;
            }
        }

        var difference = h - l;

        // Main iteration loop
        for (var i = 0;; i++)
        {
            if (PleaseStop)
                throw new OperationCanceledException("Calculation was cancelled");

            Data.Trace($"***Iteration: {i}");
            Data.Trace($"Arg prec = {workingArgPrec}, eval prec = {workingEvalPrec}, arg appr = {argAppr}");
            Data.Trace($"l = {l}, h = {h}");
            Data.Trace($"f(l) = {fL}, f(h) = {fH}");

            if (difference < Big6)
            {
                // Answer is less than 1/2 ulp away from h
                return Scale(h, -extraArgPrec);
            }

            var fDifference = fH - fL;

            // Narrow the interval by dividing at a cleverly chosen point (guess) in the middle
            BigInteger guess;
            var binaryStep = smallStepDeficit > 0 || fDifference == 0;

            if (binaryStep)
            {
                // Binary search step to guarantee linear convergence
                Data.Trace("binary step");
                guess = (l + h) >> 1;
                smallStepDeficit--;
            }
            else
            {
                // interpolate.
                // fDifference is nonzero here.
                Data.Trace("interpolating");
                var argDifference = argAppr - fL;
                var t = argDifference * difference;
                var adj = t / fDifference; // Tentative adjustment to l to compute guess

                // If we are within 1/1024 of either end, back off
                // This greatly improves the odds of bounding the answer within the smaller interval.
                // Note that interpolation will often get us MUCH closer than this.
                if (adj < difference >> 10)
                {
                    adj <<= 8;
                    Data.Trace("adjusting left");
                }
                else if (adj > (difference * Big1023) >> 10)
                {
                    adj = difference - ((difference - adj) << 8);
                    Data.Trace("adjusting right");
                }

                if (adj <= 0)
                    adj = Big2;
                if (adj >= difference)
                    adj = difference - Big2;

                guess = adj <= 0 ? l + Big2 : l + adj;
            }

            int outcome;
            var tweak = Big2;
            BigInteger fGuess;

            for (var adjPrec = false;; adjPrec = !adjPrec)
            {
                var guessCr = FromBigInteger(guess) << workingArgPrec;
                Data.Trace($"Evaluating at {guessCr} with precision {workingEvalPrec}");

                var fGuessCr = fn.Execute(guessCr);
                Data.Trace($"fn value = {fGuessCr}");
                fGuess = fGuessCr.GetApproximation(workingEvalPrec);
                outcome = SloppyCompare(fGuess, argAppr);

                if (outcome != 0) break;

                // Alternately increase evaluation precision and adjust guess slightly
                // This should be an unlikely case.
                if (adjPrec)
                {
                    // Adjust workingEvalPrec to get enough resolution
                    int adjustment = (int)-fGuess.GetBitLength() / 4;
                    if (adjustment > -20) adjustment = -20;
                    var lCr = FromBigInteger(l) << workingArgPrec;
                    var hCr = FromBigInteger(h) << workingArgPrec;
                    workingEvalPrec += adjustment;

                    Data.Trace($"New eval prec = {workingEvalPrec}" +
                               (atLeft ? "(at left)" : "") +
                               (atRight ? "(at right)" : ""));

                    fL = atLeft
                        ? _data.FLow.GetApproximation(workingEvalPrec)
                        : fn.Execute(lCr).GetApproximation(workingEvalPrec);

                    fH = atRight
                        ? _data.FHigh.GetApproximation(workingEvalPrec)
                        : fn.Execute(hCr).GetApproximation(workingEvalPrec);

                    argAppr = _arg.GetApproximation(workingEvalPrec);
                }
                else
                {
                    // Guess might be exactly right; tweak it slightly
                    Data.Trace("tweaking guess");
                    var newGuess = guess + tweak;
                    guess = newGuess >= h ? guess - tweak : newGuess;
                    // If we keep hitting the right answer, it's
                    // important to alternate which side we move it
                    // to, so that the interval shrinks rapidly.
                    tweak = -tweak;
                }
            }

            if (outcome > 0)
            {
                h = guess;
                fH = fGuess;
                atRight = false;
            }
            else
            {
                l = guess;
                fL = fGuess;
                atLeft = false;
            }

            var newDifference = h - l;
            if (!binaryStep)
            {
                if (newDifference >= difference >> 1)
                    smallStepDeficit++;
                else
                    smallStepDeficit--;
            }

            difference = newDifference;
        }
    }

    internal class Data
    {
        internal readonly UnaryCRFunction F; // Monotone increasing. If it was monotone decreasing, we negate it.
        internal readonly bool FNegated;
        internal readonly ConstructiveReal Low, High;
        internal readonly ConstructiveReal FLow, FHigh;
        internal readonly int MaxMsd; // Bound on msd of both f(high) and f(low)
        internal readonly int MaxArgPrec; // base**MaxArgPrec is a small fraction of low - high.
        internal readonly int DerivMsd; // Rough approx. of msd of first derivative.

        internal static void Trace(string s)
        {
            Debug.WriteLine(s);
        }

        internal Data(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
        {
            Low = l;
            High = h;

            var tmpFLow = func.Execute(l);
            var tmpFHigh = func.Execute(h);
            // Since func is monotone and low < high, the following test converges.
            if (tmpFLow.CompareTo(tmpFHigh) > 0)
            {
                F = UnaryCRFunction.NegateFunction.Compose(func);
                FNegated = true;
                FLow = -tmpFLow;
                FHigh = -tmpFHigh;
            }
            else
            {
                F = func;
                FNegated = false;
                FLow = tmpFLow;
                FHigh = tmpFHigh;
            }

            MaxMsd = l.Abs().Max(h.Abs()).GetMsd();
            MaxArgPrec = (h - l).GetMsd() - 4;
            DerivMsd = ((FHigh - FLow) / (h - l)).GetMsd();
        }
    }
}