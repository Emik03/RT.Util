using System;

namespace RT.Util
{
    /// <summary>
    /// This class offers static functions which generate random numbers in a thread-safe manner.
    /// </summary>
    public static class Rnd
    {
        private static Random _rnd = new Random();

        /// <summary>
        /// Resets the random number generator using the specified <paramref name="seed"/>.
        /// </summary>
        public static void Reset(int seed)
        {
            _rnd = new Random(seed);
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        public static double NextDouble()
        {
            lock (_rnd)
                return _rnd.NextDouble();
        }

        /// <summary>
        /// Returns a random double between <paramref name="min"/> and <paramref name="max"/>. It is unclear whether
        /// <paramref name="min"/> or <paramref name="max"/> can ever be returned.
        /// </summary>
        public static double NextDouble(double min, double max)
        {
            lock (_rnd)
                return _rnd.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random non-negative integer.
        /// </summary>
        public static int Next()
        {
            lock (_rnd)
                return _rnd.Next();
        }

        /// <summary>
        /// Returns a non-negative random number less than <paramref name="max"/>.
        /// </summary>
        public static int Next(int max)
        {
            lock (_rnd)
                return _rnd.Next(max);
        }

        /// <summary>
        /// Returns a random integer between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive).
        /// </summary>
        public static int Next(int min, int max)
        {
            lock (_rnd)
                return _rnd.Next(min, max);
        }
    }
}
