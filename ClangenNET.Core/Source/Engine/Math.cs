using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClangenNET;

/// <summary>
/// Helper static class for math-related utilities
/// </summary>
public static class Math
{
    public const double REAL_UNIT_INT = 1.0 / (int.MaxValue + 1.0);
    public const double REAL_UNIT_UINT = 1.0 / (uint.MaxValue + 1.0);

    /// <summary>
    /// Helper class for faster weighted randoms using <see href="https://www.keithschwarz.com/darts-dice-coins/">Voses's Alias method</see>,
    /// assuming it will be used again and the weights will not be changed.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class StaticWeightedRandom<TResult>
    {
        /// <summary>
        /// A readonly collection of <typeparamref name="TResult"/>
        /// </summary>
        private readonly ReadOnlyCollection<TResult> Items;

        /// <summary>
        /// The total amount of items
        /// </summary>
        private readonly int Length;

        /// <summary>
        /// An array representing an alias table
        /// </summary>
        private readonly int[] Alias;

        /// <summary>
        /// An array representing a probabilty table
        /// </summary>
        private readonly double[] Probability;
        private uint x, y, z, w;

        public StaticWeightedRandom(TResult[] Items, int[] Weights) 
            : this(Items, Weights, (uint)Environment.TickCount) {}


        public StaticWeightedRandom(TResult[] Items, int[] Weights, uint Seed)
        {
            if (Items.Length != Weights.Length)
                throw new IndexOutOfRangeException("Items must match size of weights");

            x = Seed; y = 842502087; z = 3579807591; w = 273326509;

            this.Items = new ReadOnlyCollection<TResult>(Items);
            int SmallCurrent, LargeCurrent, SmallIndex = 0, LargeIndex = 0, Length = this.Length = Items.Length;
            double Sum = Weights.Sum();

            double[] Probability = new double[Length], NormalProbability = new double[Length];
            int[] Alias = new int[Items.Length], Large = new int[Length], Small = new int[Length];

            for (int k = Length - 1; k >= 0; k--)
                if ((NormalProbability[k] = Weights[k] * Length / (Sum + .0d)) < 1)
                    Small[SmallIndex++] = k;
                else
                    Large[LargeIndex++] = k;


            while (0 < SmallIndex && 0 < LargeIndex)
            {
                SmallCurrent = Small[--SmallIndex]; LargeCurrent = Large[--LargeIndex];

                Probability[SmallCurrent] = NormalProbability[SmallCurrent];

                Alias[SmallCurrent] = LargeCurrent;

                if ((NormalProbability[LargeCurrent] += NormalProbability[SmallCurrent] - 1) < 1)
                    Small[SmallIndex++] = LargeCurrent;
                else
                    Large[LargeIndex++] = LargeCurrent;
            }

            while (0 < LargeIndex) Probability[Large[--LargeIndex]] = 1;
            while (0 < SmallIndex) Probability[Small[--SmallIndex]] = 1;


            this.Alias = Alias;
            this.Probability = Probability;
        }

        /// <summary>
        /// Get a weighted-random item.
        /// </summary>
        public TResult Next()
        {
            uint t = x ^ (x << 11); x = y; y = z; z = w;

            double R = REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * Length;
            int I = (int)R;

            return Items[(R - I) > Probability[I] ? Alias[I] : I];
        }

        /// <summary>
        /// Get a certain amount of weighted-random items.
        /// </summary>
        public TResult[] Next(uint Population)
        { // This method is O(1), but for smaller weights is slightly slower than RandomEx.ChooseFrom(T[], K[], uint)"/>
          // You shouldnt really worry about this! If something is slowing down its likely not this, its just a note for me
            if (Population == 0)
                return Array.Empty<TResult>();

            uint t, x = this.x, y = this.y, z = this.z, w = this.w;
            var Items = this.Items;
            var Alias = this.Alias;
            var Probability = this.Probability;

            TResult[] Results = new TResult[Population];
            for (uint i = 0; i < Population; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;

                double R = REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * Length;
                int I = (int)R;

                Results[i] = Items[(R - I) > Probability[I] ? Alias[I] : I];
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
            return Results;
        }

        /// <summary>
        /// Fill an array of type <typeparamref name="TResult"/> with weighted-random items. 
        /// </summary>
        public void Next(TResult[] Array)
        {
            if (Array.Length == 0) return;

            uint t, x = this.x, y = this.y, z = this.z, w = this.w;
            var Items = this.Items;
            var Alias = this.Alias;
            var Probability = this.Probability;

            for (uint i = 0; i < Array.Length; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;

                double R = REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * Length;
                int I = (int)R;

                Array[i] = Items[(R - I) > Probability[I] ? Alias[I] : I];
            }


            this.x = x; this.y = y; this.z = z; this.w = w;
        }
    }

    /// <summary>
    /// Helper class for psuedo-random number generation aswell as a few other utilities.
    /// Code adapted from <see href="https://www.codeproject.com/Articles/9187/A-fast-equivalent-for-System-Random.">here</see>
    /// </summary>
    public class RandomEx
    {
        private uint x, y, z, w;
        private uint BitBuffer;
        private uint BitMask;

        public RandomEx()
        {
            Seed((uint)Environment.TickCount);
        }

        public RandomEx(uint seed)
        {
            Seed(seed);
        }

        /// <summary>
        /// Sets the first element of state to <paramref name="seed"/>.
        /// </summary>
        public void Seed(uint Seed)
        {
            x = Seed; y = 842502087; z = 3579807591; w = 273326509;
        }

        /// <summary>
        /// Sets the whole state to given unsigned integers.
        /// </summary>
        public void Seed(uint State1, uint State2, uint State3, uint State4)
        {
            x = State1; y = State2; z = State3; w = State4;
        }

        /// <summary>
        /// Generate a random unsigned integer.
        /// </summary>
        public uint Random()
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            return w ^= (w >> 19) ^ t ^ (t >> 8);
        }

        /// <summary>
        /// Generate a random given <see cref="IUnsignedNumber{TSelf}"/> integer of <typeparamref name="TResult"/> 
        /// type that is within the constraints of <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The unsigned integer type</typeparam>
        public TResult Random<TResult>() where TResult : IUnsignedNumber<TResult>, IMinMaxValue<TResult>, IBinaryInteger<TResult>
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            return TResult.CreateTruncating(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * uint.CreateTruncating(TResult.MaxValue));
        }

        /// <summary>
        /// Generate a random signed integer.
        /// </summary>
        public int RandomSigned()
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            return (int)(w ^= (w >> 19) ^ t ^ (t >> 8));
        }

        /// <summary>
        /// Generate a random given <see cref="IUnsignedNumber{TSelf}"/> integer of <typeparamref name="TResult"/> 
        /// type that is within the constraints of <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The signed integer type</typeparam>
        public TResult RandomSigned<TResult>() where TResult : ISignedNumber<TResult>, IMinMaxValue<TResult>, IBinaryInteger<TResult>
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            int Min = int.CreateTruncating(TResult.MinValue);
            return TResult.CreateTruncating(Min + (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * (int.CreateTruncating(TResult.MaxValue) - Min)));
        }

        /// <summary>
        /// Generate a random signed integer that is between 0 (inc) and <paramref name="maxValue"/> (exc).
        /// </summary>
        /// <param name="maxValue">The maximum value the integer can be (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int RandomSigned(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be >= 0");

            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            return (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * maxValue);
        }

        /// <summary>
        /// Generate a random signed integer that is between <paramref name="minValue"/> (inc) 
        /// and <paramref name="maxValue"/> (exclusive).
        /// </summary>
        /// <param name="minValue">The minimum value the integer can be (inclusive)</param>
        /// <param name="maxValue">The maximum value the integer can be (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int RandomSigned(int minValue, int maxValue)
        {
            if (minValue > maxValue || maxValue == minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be greater than minValue");

            uint t = x ^ (x << 11);
            x = y; y = z; z = w;

            int range = maxValue - minValue;
            if (range < 0)
                return minValue + (int)(REAL_UNIT_UINT * (w ^= (w >> 19) ^ t ^ (t >> 8)) * (maxValue - minValue));
            
            return minValue + (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * range);
        }

        /// <summary>
        /// Generate a random double greater than or equal to 0 and less than 1
        /// </summary>
        public double Double()
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            return REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8)));
        }

        /// <summary>
        /// Fill a given byte array with random bytes.
        /// </summary>
        /// <param name="Buffer">The byte array to be overwritten</param>
        public void Bytes(byte[] Buffer)
        {
            uint t, i = 0, x = this.x, y = this.y, z = this.z, w = this.w;

            for (int bound = Buffer.Length - 3; i < bound;)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                w ^= (w >> 19) ^ t ^ (t >> 8);
                Buffer[i++] = (byte)w;
                Buffer[i++] = (byte)(w >> 8);
                Buffer[i++] = (byte)(w >> 16);
                Buffer[i++] = (byte)(w >> 24);
            }

            if (i < Buffer.Length)
            {
                t = x ^ (x << 11);
                x = y; y = z; z = w;
                w ^= (w >> 19) ^ t ^ (t >> 8);

                Buffer[i++] = (byte)w;
                if (i < Buffer.Length)
                {
                    Buffer[i++] = (byte)(w >> 8);
                    if (i < Buffer.Length)
                    {
                        Buffer[i++] = (byte)(w >> 16);
                        if (i < Buffer.Length)
                            Buffer[i] = (byte)(w >> 24);
                    }
                }
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        /// <summary>
        /// Generate a random array of bytes. If byte array will generate multiple times, have a 
        /// look at <see cref="Bytes(byte[])"/>
        /// </summary>
        /// <param name="Length">The length of the new byte array</param>
        public byte[] Bytes(uint Length)
        {
            byte[] Buffer = new byte[Length];
            uint t, i = 0, x = this.x, y = this.y, z = this.z, w = this.w;

            for (uint bound = Length - 3; i < bound;)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                w ^= (w >> 19) ^ t ^ (t >> 8);
                Buffer[i++] = (byte)w;
                Buffer[i++] = (byte)(w >> 8);
                Buffer[i++] = (byte)(w >> 16);
                Buffer[i++] = (byte)(w >> 24);
            }

            if (i < Length)
            {
                t = x ^ (x << 11);
                x = y; y = z; z = w;
                w ^= (w >> 19) ^ t ^ (t >> 8);

                Buffer[i++] = (byte)w;
                if (i < Length)
                {
                    Buffer[i++] = (byte)(w >> 8);
                    if (i < Length)
                    {
                        Buffer[i++] = (byte)(w >> 16);
                        if (i < Length)
                            Buffer[i] = (byte)(w >> 24);
                    }
                }
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
            return Buffer;
        }

        /// <summary>
        /// Generate a random boolean value based on a given probability. If <paramref name="Chance"/> is 1
        /// or greater, it will return <see langword="true"/>.
        /// </summary>
        /// <param name="Chance">A double greater than 0 and less than 1.</param>
        public bool Chance(double Chance)
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            return (w ^= (w >> 19) ^ t ^ (t >> 8)) < (Chance * uint.MaxValue);
        }

        /// <summary>
        /// Shuffles given Array randomly.
        /// </summary>
        /// <param name="Array">The array to shuffle</param>
        public void Shuffle<TResult>(TResult[] Array)
        {
            uint Index, t, x = this.x, y = this.y, z = this.z, w = this.w;

            for (int i = 0; i < Array.Length - 1; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                Index = (uint)(i + (REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * (Array.Length - i)));
                (Array[Index], Array[i]) = (Array[i], Array[Index]);
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        /// <summary>
        /// Shuffles given List randomly.
        /// </summary>
        /// <param name="List">The list to shuffle</param>
        public void Shuffle<TResult>(IList<TResult> List)
        {
            uint t, x = this.x, y = this.y, z = this.z, w = this.w;
            int Index;

            for (int i = 0; i < List.Count - 1; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                Index = (int)(i + (REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * (List.Count - i)));
                (List[Index], List[i]) = (List[i], List[Index]);
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        /// <summary>
        /// Generate a random boolean.
        /// </summary>
        public bool Choose()
        {
            if (BitMask == 1)
            {
                uint t = x ^ (x << 11); x = y; y = z; z = w;
                return ((BitBuffer = w ^= (w >> 19) ^ t ^ (t >> 8)) & (BitMask = 0x8000000)) == 0;
            }

            return (BitBuffer & (BitMask >>= 1)) == 0;
        }

        /// <summary>
        /// Return a random item from given array.
        /// </summary>
        /// <param name="Array">Array to choose from</param>
        /// <exception cref="ArgumentException">Array is empty</exception>
        public TResult ChooseFrom<TResult>(TResult[] Array)
        {
            if (Array.Length == 0)
                throw new ArgumentException("Array is empty", nameof(Array));

            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            return Array[(uint)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * Array.Length)];
        }

        /// <summary>
        /// Return a weighted item from given array. weights must be a <see cref="ISignedNumber{TSelf}"/> object
        /// </summary>
        /// <param name="Array">Array to choose from</param>
        /// <param name="Weights">The weights to adhere to</param>
        /// <exception cref="ArgumentException">Array or weights are empty, or Array isnt the size of weights</exception>
        public TResult ChooseFrom<TResult, WeightType>(TResult[] Array, WeightType[] Weights) where WeightType : ISignedNumber<WeightType>
        { // We could define K individually in functions i.e int[] weights, but on Release it is anywhere from 0 ms - 3 ms slower so I don think it matters
            if (Array.Length == 0)
                throw new ArgumentException("Array is empty", nameof(Array));
            else if (Weights.Length == 0)
                throw new ArgumentException("weights is empty", nameof(Weights));
            else if (Weights.Length != Array.Length)
                throw new ArgumentException("weights length must be the same as the given Arrays' length");

            WeightType RemainingDistance = WeightType.Zero;
            for (int i = 0; i < Weights.Length; i++)
                RemainingDistance += Weights[i];

            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            RemainingDistance = WeightType.CreateTruncating(
                double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8)))
            );

            for (int i = 0; i < Weights.Length; i++)
            {
                RemainingDistance -= Weights[i];
                if (WeightType.IsNegative(RemainingDistance))
                    return Array[i];
            }

            return Array[0];
        }

        /// <summary>
        /// Return a given amount of random of items from given array.
        /// </summary>
        /// <param name="Array">Array to choose from</param>
        /// <param name="Population">The amount of items to retrieve</param>
        /// <exception cref="ArgumentException">Array is empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">Population should be non-zero</exception>
        public TResult[] ChooseFrom<TResult>(TResult[] Array, uint Population)
        {
            if (Array.Length == 0)
                throw new ArgumentException("Array is empty", nameof(Array));
            else if (Population == 0)
                throw new ArgumentOutOfRangeException(nameof(Population), "Population should be non-zero");

            uint t, x = this.x, y = this.y, z = this.z, w = this.w;
            TResult[] Results = new TResult[Population];

            for (int i = 0; i < Population; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                Results[i] = Array[(uint)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8))) * Array.Length)];
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
            return Results;
        }

        /// <summary>
        /// Return given amount of weighted items from given array. weights must be a <see cref="ISignedNumber{TSelf}"/> object
        /// </summary>
        /// <param name="Array">Array to choose from</param>
        /// <param name="Weights">The weights to adhere to</param>
        /// <param name="Population">The amount of items to retrieve</param>
        /// <exception cref="ArgumentException">Array or weights are empty, or Array isnt the size of weights</exception>
        public TResult[] ChooseFrom<TResult, WeightType>(TResult[] Array, WeightType[] Weights, uint Population) where WeightType : ISignedNumber<WeightType>
        {
            if (Array.Length == 0)
                throw new ArgumentException("Array is empty", nameof(Array));
            else if (Weights.Length == 0)
                throw new ArgumentException("weights is empty", nameof(Weights));
            else if (Weights.Length != Array.Length)
                throw new ArgumentException("weights length must be the same as the given Arrays' length");
            else if (Population == 0)
                return System.Array.Empty<TResult>();

            WeightType RemainingDistance = WeightType.Zero, CumulativeWeight = WeightType.Zero;
            for (int i = 0; i < Weights.Length; i++)
                CumulativeWeight += Weights[i];

            uint t, x = this.x, y = this.y, z = this.z, w = this.w;

            TResult[] Results = new TResult[Population];

            for (uint i = 0; i < Population; i++)
            {
                t = x ^ (x << 11); x = y; y = z; z = w;
                RemainingDistance = WeightType.CreateTruncating(
                    double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (w ^= (w >> 19) ^ t ^ (t >> 8)))
                );

                for (int k = 0; k < Weights.Length; k++)
                {
                    RemainingDistance -= Weights[k];
                    if (WeightType.IsNegative(RemainingDistance))
                    {
                        Results[i] = Array[k];
                        break;
                    }
                }
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
            return Results;
        }
    }
}
