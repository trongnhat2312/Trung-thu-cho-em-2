using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Koi.MathUtils
{
    // ==++==
    // 
    //   Copyright (c) Microsoft Corporation.  All rights reserved.
    // 
    // ==--==
    /*============================================================
    **
    ** Class:  Random
    **
    **
    ** Purpose: A random number generator.
    **
    ** 
    ===========================================================*/

    public class FixedRandom
    {
        private const int MBIG = Int32.MaxValue;
        private const int MSEED = 161803398;
        private const int MZ = 0;

        private int inext;
        private int inextp;
        private int[] SeedArray = new int[56];

        private static FixedRandom Instance = new FixedRandom();

        public static void SetupSeed(int seed)
        {
            Instance.InitRandom(seed);
        }

        public static int Rand(int min, int max)
        {
            return Instance.Next(min, max);
        }

        public void InitRandom(int Seed)
        {
            int ii;
            int mj, mk;

            //Initialize our Seed array.
            //This algorithm comes from Numerical Recipes in C (2nd Ed.)
            int subtraction = (Seed == Int32.MinValue) ? Int32.MaxValue : Math.Abs(Seed);
            mj = MSEED - subtraction;
            SeedArray[55] = mj;
            mk = 1;
            for (int i = 1; i < 55; i++)
            {  //Apparently the range [1..55] is special (Knuth) and so we're wasting the 0'th position.
                ii = (21 * i) % 55;
                SeedArray[ii] = mk;
                mk = mj - mk;
                if (mk < 0) mk += MBIG;
                mj = SeedArray[ii];
            }
            for (int k = 1; k < 5; k++)
            {
                for (int i = 1; i < 56; i++)
                {
                    SeedArray[i] -= SeedArray[1 + (i + 30) % 55];
                    if (SeedArray[i] < 0) SeedArray[i] += MBIG;
                }
            }
            inext = 0;
            inextp = 21;
            Seed = 1;
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {

            }
            Contract.EndContractBlock();

            long range = (long)maxValue - minValue;
            if (range <= (long)Int32.MaxValue)
            {
                return ((int)(Sample() * range) + minValue);
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }

        protected virtual double Sample()
        {
            //Including this division at the end gives us significantly improved
            //random number distribution.
            return (InternalSample() * (1.0 / MBIG));
        }

        private int InternalSample()
        {
            int retVal;
            int locINext = inext;
            int locINextp = inextp;

            if (++locINext >= 56) locINext = 1;
            if (++locINextp >= 56) locINextp = 1;

            retVal = SeedArray[locINext] - SeedArray[locINextp];

            if (retVal == MBIG) retVal--;
            if (retVal < 0) retVal += MBIG;

            SeedArray[locINext] = retVal;

            inext = locINext;
            inextp = locINextp;

            return retVal;
        }

        private double GetSampleForLargeRange()
        {
            // The distribution of double value returned by Sample 
            // is not distributed well enough for a large range.
            // If we use Sample for a range [Int32.MinValue..Int32.MaxValue)
            // We will end up getting even numbers only.

            int result = InternalSample();
            // Note we can't use addition here. The distribution will be bad if we do that.
            bool negative = (InternalSample() % 2 == 0) ? true : false;  // decide the sign based on second sample
            if (negative)
            {
                result = -result;
            }
            double d = result;
            d += (Int32.MaxValue - 1); // get a number in range [0 .. 2 * Int32MaxValue - 1)
            d /= 2 * (uint)Int32.MaxValue - 1;
            return d;
        }
    }
}