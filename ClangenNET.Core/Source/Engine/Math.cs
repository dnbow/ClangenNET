using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClangenNET;

/// <summary>
/// Helper static class for math-related utilities
/// </summary>
public static class MathEx
{
    public const double REAL_UNIT_INT = 1.0 / (int.MaxValue + 1.0);
    public const double REAL_UNIT_UINT = 1.0 / (uint.MaxValue + 1.0);

    /// <summary>
    /// Helper class for psuedo-random number generation aswell as a few other utilities.
    /// Code adapted from <see href="https://www.codeproject.com/Articles/9187/A-fast-equivalent-for-System-Random.">here</see>
    /// </summary>
    public class RandomEx
    {
        private const uint MT19937 = 1812433253;
        private uint X, Y, Z, W;
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
        /// Sets the first element of state to <paramref name="Seed"/>.
        /// </summary>
        public void Seed(uint Seed)
        {
            X = Seed; Y = MT19937 * X + 1; Z = MT19937 * Y + 1; W = MT19937 * Z + 1;
        }

        /// <summary>
        /// Generate a random unsigned integer.
        /// </summary>
        public uint Unsigned()
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return W ^= W >> 19 ^ T ^ T >> 8;
        }

        /// <summary>
        /// Generate a random given <see cref="IUnsignedNumber{TSelf}"/> integer of <typeparamref name="TResult"/> 
        /// type that is within the constraints of <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The unsigned integer type</typeparam>
        public TResult Unsigned<TResult>() where TResult : IUnsignedNumber<TResult>, IMinMaxValue<TResult>, IBinaryInteger<TResult>
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return TResult.CreateTruncating(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * ulong.CreateTruncating(TResult.MaxValue));
        }

        /// <summary>
        /// Generate a random signed integer.
        /// </summary>
        public int Signed()
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return (int)(W ^= W >> 19 ^ T ^ T >> 8);
        }

        /// <summary>
        /// Generate a random given <see cref="IUnsignedNumber{TSelf}"/> integer of <typeparamref name="TResult"/> 
        /// type that is within the constraints of <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The signed integer type</typeparam>
        public TResult Signed<TResult>() where TResult : ISignedNumber<TResult>, IMinMaxValue<TResult>, IBinaryInteger<TResult>
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            int Min = int.CreateTruncating(TResult.MinValue);
            return TResult.CreateTruncating(Min + (int)(REAL_UNIT_INT * (int)(W ^= W >> 19 ^ T ^ T >> 8) * (long.CreateTruncating(TResult.MaxValue) - Min)));
        }

        /// <summary>
        /// Generate a random signed integer that is between 0 (inc) and <paramref name="maxValue"/> (exc).
        /// </summary>
        /// <param name="maxValue">The maximum value the integer can be (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Signed(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be >= 0");

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8)) * maxValue);
        }

        /// <summary>
        /// Generate a random signed integer that is between <paramref name="Min"/> (inc) 
        /// and <paramref name="Max"/> (exclusive).
        /// </summary>
        /// <param name="Min">The minimum value the integer can be (inclusive)</param>
        /// <param name="Max">The maximum value the integer can be (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Signed(int Min, int Max)
        {
            if (Min > Max || Max == Min)
                throw new ArgumentOutOfRangeException(nameof(Max), Max, "Max must be greater than Min");

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;

            return Min + (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8)) * (Max - Min));
        }

        /// <summary>
        /// Generate a random double greater than or equal to 0 and less than 1
        /// </summary>
        public double Chance()
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8);
        }

        /// <summary>
        /// Generate a random boolean value based on a given probability. If <paramref name="Chance"/> is 1
        /// or greater, it will return <see langword="true"/>.
        /// </summary>
        /// <param name="Chance">A double greater than 0 and less than 1</param>
        public bool Chance(double Chance)
        {
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return (W ^= W >> 19 ^ T ^ T >> 8) < Chance * uint.MaxValue;
        }

        /// <summary>
        /// Fill a given byte array with random bytes.
        /// </summary>
        /// <param name="Buffer">The byte array to be overwritten</param>
        public void Bytes(byte[] Buffer)
        {
            uint T, I = 0, X = this.X, Y = this.Y, Z = this.Z, W = this.W;

            for (int Bound = Buffer.Length - 3; I < Bound;)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                W ^= W >> 19 ^ T ^ T >> 8;
                Buffer[I++] = (byte)W;
                Buffer[I++] = (byte)(W >> 8);
                Buffer[I++] = (byte)(W >> 16);
                Buffer[I++] = (byte)(W >> 24);
            }

            if (I < Buffer.Length)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                W ^= W >> 19 ^ T ^ T >> 8;

                Buffer[I++] = (byte)W;
                if (I < Buffer.Length)
                {
                    Buffer[I++] = (byte)(W >> 8);
                    if (I < Buffer.Length)
                    {
                        Buffer[I++] = (byte)(W >> 16);
                        if (I < Buffer.Length)
                            Buffer[I] = (byte)(W >> 24);
                    }
                }
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
        }

        /// <summary>
        /// Fill a given span of bytes with random ones.
        /// </summary>
        /// <param name="Buffer">The span of bytes to be overwritten</param>
        public void Bytes(Span<byte> Buffer)
        {
            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;
            int I = 0;

            for (int Bound = Buffer.Length - 3; I < Bound;)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                W ^= W >> 19 ^ T ^ T >> 8;
                Buffer[I++] = (byte)W;
                Buffer[I++] = (byte)(W >> 8);
                Buffer[I++] = (byte)(W >> 16);
                Buffer[I++] = (byte)(W >> 24);
            }

            if (I < Buffer.Length)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                W ^= W >> 19 ^ T ^ T >> 8;

                Buffer[I++] = (byte)W;
                if (I < Buffer.Length)
                {
                    Buffer[I++] = (byte)(W >> 8);
                    if (I < Buffer.Length)
                    {
                        Buffer[I++] = (byte)(W >> 16);
                        if (I < Buffer.Length)
                            Buffer[I] = (byte)(W >> 24);
                    }
                }
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
        }

        /// <summary>
        /// Shuffles given List randomly.
        /// </summary>
        /// <param name="List">The list to shuffle</param>
        public void Shuffle<TResult>(IList<TResult> List)
        {
            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;
            int Index;

            for (int I = 0; I < List.Count - 1; I++)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                Index = (int)(I + REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8)) * (List.Count - I));
                (List[Index], List[I]) = (List[I], List[Index]);
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
        }

        /// <summary>
        /// Return a random boolean.
        /// </summary>
        public bool Choose()
        {
            if (BitMask == 1)
            {
                uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
                return ((BitBuffer = W ^= W >> 19 ^ T ^ T >> 8) & (BitMask = 0x8000000)) == 0;
            }

            return (BitBuffer & (BitMask >>= 1)) == 0;
        }

        /// <summary>
        /// Return a random value from a given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum to give</typeparam>
        /// <exception cref="ArgumentException"></exception>
        public TEnum Choose<TEnum>() where TEnum : struct, Enum
        {
            TEnum[] EnumValues = Enum.GetValues<TEnum>();

            if (EnumValues.Length == 0)
                throw new ArgumentException("Enum has no value", nameof(TEnum));

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return EnumValues[(uint)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * EnumValues.Length)];
        }

        /// <summary>
        /// Return a certain amount of random of items from given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum to give</typeparam>
        /// <exception cref="ArgumentException">Enum has no values to choose from</exception>
        public TEnum[] Choose<TEnum>(uint Population) where TEnum : struct, Enum
        {
            TEnum[] EnumValues = Enum.GetValues<TEnum>();

            if (EnumValues.Length == 0)
                throw new ArgumentException("Enum has no value", nameof(TEnum));

            TEnum[] Results = new TEnum[Population];
            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;

            for (uint I = 0; I < Population; I++)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                Results[I] = EnumValues[(uint)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * EnumValues.Length)];
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
            return Results;
        }

        /// <summary>
        /// Return a random item from given Enumerable.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        public TResult Choose<TResult>(IEnumerable<TResult> Sample)
        {
            if (!Sample.Any())
                throw new ArgumentException("Sample is empty", nameof(Sample));

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;

            IEnumerator<TResult> Enumerator = Sample.GetEnumerator();
            for (int _ = (int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count()) - 1; _ >= 0; _--) Enumerator.MoveNext();
            return Enumerator.Current;
        }

        /// <summary>
        /// Return a random item from given 2 dimensional Enumerable.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        public TResult Choose<TResult>(IEnumerable<IEnumerable<TResult>> Sample)
        {
            if (!Sample.Any())
                throw new ArgumentException("Sample is empty", nameof(Sample));

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;

            IEnumerator<IEnumerable<TResult>> Enumerator = Sample.GetEnumerator();
            for (int _ = (int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count()) - 1; _ >= 0; _--) Enumerator.MoveNext();
            T = X ^ X << 11; X = Y; Y = Z; Z = W;

            IEnumerator<TResult> EnumeratorFinal = Enumerator.Current.GetEnumerator();
            for (int _ = (int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Enumerator.Current.Count()) - 1; _ >= 0; _--) Enumerator.MoveNext();

            return EnumeratorFinal.Current;
        }

        /// <summary>
        /// Return a weighted item from given Enumerable. Weights must be a <see cref="ISignedNumber{TSelf}"/> object
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <param name="Weights">The Weights to adhere to</param>
        /// <exception cref="ArgumentException">Sample or Weights are empty, or Sample isnt the size of Weights</exception>
        public TResult Choose<TResult, TWeight>(IEnumerable<TResult> Sample, IList<TWeight> Weights) where TWeight : notnull, ISignedNumber<TWeight>
        {
            if (!Sample.Any())
                throw new ArgumentException("Sample is empty", nameof(Sample));
            else if (Weights.Count == 0)
                throw new ArgumentException("Weights is empty", nameof(Weights));
            else if (Weights.Count != Sample.Count())
                throw new ArgumentException("Weights length must be the same as the given Sample' length");

            TWeight RemainingDistance = TWeight.Zero;
            for (int I = 0; I < Weights.Count; I++)
                RemainingDistance += Weights[I];

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            RemainingDistance = TWeight.CreateTruncating(
                double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8))
            );

            IEnumerator<TResult> Enumerator = Sample.GetEnumerator();
            for (int I = 0; I < Weights.Count; I++)
            {
                Enumerator.MoveNext();
                if (TWeight.IsNegative(RemainingDistance -= Weights[I]))
                    return Enumerator.Current;
            }

            return Sample.First();
        }

        /// <summary>
        /// Return a given amount of random of items from given Enumerable.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <param name="Population">The amount of items to retrieve</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">Population should be non-zero</exception>
        public TResult[] Choose<TResult>(IEnumerable<TResult> Sample, uint Population)
        {
            if (!Sample.Any())
                throw new ArgumentException("Sample is empty", nameof(Sample));
            else if (Population == 0)
                throw new ArgumentOutOfRangeException(nameof(Population), "Population should be non-zero");

            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;
            TResult[] Results = new TResult[Population];

            for (int I = 0; I < Population; I++)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                IEnumerator<TResult> Enumerator = Sample.GetEnumerator();
                for (int _ = (int)(REAL_UNIT_INT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count()) - 1; I >= 0; I--) Enumerator.MoveNext();
                Results[I] = Enumerator.Current;
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
            return Results;
        }

        /// <summary>
        /// Return a random item from given List.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        public TResult Choose<TResult>(IReadOnlyList<TResult> Sample)
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Sample is empty", nameof(Sample));

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return Sample[(int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count)];
        }

        /// <summary>
        /// Return a random item from given 2 dimensional List.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        public TResult Choose<TResult>(IReadOnlyList<IReadOnlyList<TResult>> Sample)
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Sample is empty", nameof(Sample));

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            int F = (int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count);
            T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return Sample[F][(int)(REAL_UNIT_UINT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample[F].Count)];
        }

        /// <summary>
        /// Return a weighted item from given Sample. Weights must be a <see cref="ISignedNumber{TSelf}"/> object
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <param name="Weights">The Weights to adhere to</param>
        /// <exception cref="ArgumentException">Sample or Weights are empty, or Sample isnt the size of Weights</exception>
        public TResult Choose<TResult, TWeight>(IReadOnlyList<TResult> Sample, IList<TWeight> Weights) where TWeight : ISignedNumber<TWeight>
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Sample is empty", nameof(Sample));
            else if (Weights.Count == 0)
                throw new ArgumentException("Weights is empty", nameof(Weights));
            else if (Weights.Count != Sample.Count)
                throw new ArgumentException("Weights length must be the same as the given Sample' length");

            TWeight RemainingDistance = TWeight.Zero;
            for (int I = 0; I < Weights.Count; I++)
                RemainingDistance += Weights[I];

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            RemainingDistance = TWeight.CreateTruncating(
                double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8))
            );

            for (int I = 0; I < Weights.Count; I++)
                if (TWeight.IsNegative(RemainingDistance -= Weights[I]))
                    return Sample[I];

            return Sample[0];
        }

        /// <summary>
        /// Return a given amount of random of items from given Sample.
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <param name="Population">The amount of items to retrieve</param>
        /// <exception cref="ArgumentException">Sample is empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">Population should be non-zero</exception>
        public TResult[] Choose<TResult>(IReadOnlyList<TResult> Sample, uint Population)
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Sample is empty", nameof(Sample));
            else if (Population == 0)
                throw new ArgumentOutOfRangeException(nameof(Population), "Population should be non-zero");

            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;
            TResult[] Results = new TResult[Population];

            for (int I = 0; I < Population; I++)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                Results[I] = Sample[(int)(REAL_UNIT_INT * (W ^= W >> 19 ^ T ^ T >> 8) * Sample.Count)];
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
            return Results;
        }

        /// <summary>
        /// Return given amount of weighted items from given Sample. Weights must be a <see cref="ISignedNumber{TSelf}"/> object
        /// </summary>
        /// <param name="Sample">Sample to choose from</param>
        /// <param name="Weights">The Weights to adhere to</param>
        /// <param name="Population">The amount of items to retrieve</param>
        /// <exception cref="ArgumentException">Sample or Weights are empty, or Sample isnt the size of Weights</exception>
        public TResult[] Choose<TResult, TWeight>(IReadOnlyList<TResult> Sample, IList<TWeight> Weights, uint Population) where TWeight : ISignedNumber<TWeight>
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Sample is empty", nameof(Sample));
            else if (Weights.Count == 0)
                throw new ArgumentException("Weights is empty", nameof(Weights));
            else if (Weights.Count != Sample.Count)
                throw new ArgumentException("Weights length must be the same as the given Sample' length", nameof(Weights));
            else if (Population == 0)
                return [];

            TWeight RemainingDistance = TWeight.Zero, CumulativeWeight = TWeight.Zero;
            for (int I = 0; I < Weights.Count; I++)
                CumulativeWeight += Weights[I];

            uint T, X = this.X, Y = this.Y, Z = this.Z, W = this.W;

            TResult[] Results = new TResult[Population];

            for (uint I = 0; I < Population; I++)
            {
                T = X ^ X << 11; X = Y; Y = Z; Z = W;
                RemainingDistance = TWeight.CreateTruncating(
                    double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8))
                );

                for (int k = 0; k < Weights.Count; k++)
                {
                    RemainingDistance -= Weights[k];
                    if (TWeight.IsNegative(RemainingDistance))
                    {
                        Results[I] = Sample[k];
                        break;
                    }
                }
            }

            this.X = X; this.Y = Y; this.Z = Z; this.W = W;
            return Results;
        }

        /// <summary>
        /// Return an index according to given Weights.
        /// </summary>
        /// <param name="Weights">The Weights to adhere to</param>
        public uint ChooseIndex<TWeight>(TWeight[] Weights) where TWeight : notnull, ISignedNumber<TWeight>
        {
            TWeight RemainingDistance = TWeight.Zero;
            for (int i = 0; i < Weights.Length; i++)
                RemainingDistance += Weights[i];

            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            RemainingDistance = TWeight.CreateTruncating(
                double.CreateTruncating(RemainingDistance) * REAL_UNIT_INT * (int)(0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8))
            );

            for (uint I = 0; I < Weights.Length; I++)
                if (TWeight.IsNegative(RemainingDistance -= Weights[I]))
                    return I;

            return 0;
        }


        // NICHE but unoptimized cases, will optimise later
        public TResult ChooseExcept<TResult>(IReadOnlyList<TResult> Sample, int ExceptionIndex)
        {
            if (Sample.Count == 0)
                throw new ArgumentException("Array is empty", nameof(Sample));

            int Index;
            uint T = X ^ X << 11; X = Y; Y = Z; Z = W;
            return Sample[
                (Index = (int)(REAL_UNIT_UINT * (0x7FFFFFFF & (W ^= W >> 19 ^ T ^ T >> 8)) * (Sample.Count - 1))) == ExceptionIndex ? Sample.Count - 1 : Index
            ];
        }
        public TResult ChooseExcept<TResult>(IReadOnlyList<TResult> Sample, TResult Exception) 
            => ChooseExcept(Sample, Sample.First(I => I?.Equals(Exception) ?? false));
        public TResult Choose<TResult, TWeight>(IReadOnlyList<IReadOnlyList<TResult>> Sample, IList<TWeight> Weights) where TWeight : ISignedNumber<TWeight>
            => Choose(Choose<IReadOnlyList<TResult>, TWeight>(Sample, Weights));
    }
}
