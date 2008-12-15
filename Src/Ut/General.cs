using System;
using System.Collections.Generic;

namespace RT.Util
{
    /// <summary>
    /// This class offers some generic static functions which are hard to categorize
    /// under any more specific classes.
    /// </summary>
    public static partial class Ut
    {
        /// <summary>
        /// Compares two arrays with the elements of the specified type for equality.
        /// Arrays are equal if both are null, or if all elements are equal.
        /// </summary>
        public static bool ArraysEqual<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
        {
            if (arr1 == null && arr2 == null)
                return true;
            else if (arr1 == null || arr2 == null)
                return false;
            else if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
                if (arr1[i].Equals(arr2[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Counts the number of occurrences of string in another string
        /// </summary>
        /// <param name="inString">Main string</param>
        /// <param name="toBeCounted">String to be counted</param>
        /// <returns>Number of occurrences of to_be_counted</returns>
        public static int CountStrings(string inString, string toBeCounted)
        {
            int result = -1;
            int last = -1;
            do
            {
                result++;
                last = inString.IndexOf(toBeCounted, last + 1);
            } while (last != -1);
            return result;
        }

        /// <summary>
        /// Converts file size in bytes to a string in bytes, kbytes, Mbytes
        /// or Gbytes accordingly. The suffix appended is kB, MB or GB.
        /// </summary>
        /// <param name="size">Size in bytes</param>
        /// <returns>Converted string</returns>
        public static string SizeToString(long size)
        {
            if (size == 0)
                return "0";
            else if (size < 1024)
                return size.ToString("#,###");
            else if (size < 1024 * 1024)
                return (size / 1024d).ToString("#,###.## kB");
            else if (size < 1024 * 1024 * 1024)
                return (size / (1024d * 1024d)).ToString("#,###.## MB");
            else
                return (size / (1024d * 1024d * 1024d)).ToString("#,###.## GB");
        }

        /// <summary>
        /// Returns an IEnumerable containing all integers between the specified First and Last integers (all inclusive).
        /// </summary>
        /// <param name="first">First integer to return.</param>
        /// <param name="last">Last integer to return.</param>
        /// <returns>An IEnumerable containing all integers between the specified First and Last integers (all inclusive).</returns>
        public static IEnumerable<int> Range(int first, int last)
        {
            for (int i = first; i <= last; i++)
                yield return i;
        }
    }
}
