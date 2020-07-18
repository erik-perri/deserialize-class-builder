using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DeserializeClassBuilder
{
    /// <summary>
    /// This is a copy of the Java Boyer-Moore implementation found at Wikipedia
    /// https://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string-search_algorithm
    /// </summary>
    public sealed class BoyerMoore
    {
        /// <summary>
        /// Returns the index within this string of the first occurrence of the
        /// specified substring. If it is not a substring, return -1.
        /// </summary>
        /// <param name="haystack">The string to be scanned</param>
        /// <param name="needle">The target string to search</param>
        /// <param name="resultLimit">The maximum number of results to return</param>
        /// <returns>The start index of the substring</returns>
        public static ReadOnlyCollection<int> IndexesOf(byte[] haystack, byte[] needle, int resultLimit = -1)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }

            if (haystack == null)
            {
                throw new ArgumentNullException(nameof(haystack));
            }

            if (haystack.Length == 0)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException("Haystack empty", nameof(haystack));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            if (needle.Length == 0)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException("Needle empty", nameof(needle));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            int[] charTable = MakeCharTable(needle);
            int[] offsetTable = MakeOffsetTable(needle);
            List<int> offsets = new List<int>();

            for (int i = needle.Length - 1, j; i < haystack.Length;)
            {
                for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
                {
                    if (j == 0)
                    {
                        offsets.Add(i);

                        if (resultLimit != -1 && offsets.Count > resultLimit)
                        {
                            return new ReadOnlyCollection<int>(offsets);
                        }

                        break;
                    }
                }
                
                // i += needle.length - j; // For naive method
                i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
            }

            return new ReadOnlyCollection<int>(offsets);
        }

        /// <summary>
        /// Returns the index within this string of the first occurrence of the
        /// specified substring. If it is not a substring, return -1.
        /// </summary>
        /// <param name="haystack">The string to be scanned</param>
        /// <param name="needle">The target string to search</param>
        /// <returns></returns>
        public static int IndexOf(byte[] haystack, byte[] needle)
        {
            return IndexesOf(haystack, needle).FirstOrDefault();
        }

        /// <summary>
        /// Makes the jump table based on the mismatched character information.
        /// </summary>
        /// <param name="needle">The target string to search</param>
        /// <returns></returns>
        private static int[] MakeCharTable(byte[] needle)
        {
            const int ALPHABET_SIZE = 65536;
            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; ++i)
            {
                table[i] = needle.Length;
            }
            for (int i = 0; i < needle.Length - 2; ++i)
            {
                table[needle[i]] = needle.Length - 1 - i;
            }
            return table;
        }

        /// <summary>
        /// Makes the jump table based on the scan offset which mismatch occurs.
        /// (bad character rule).
        /// </summary>
        /// <param name="needle"></param>
        /// <returns></returns>
        private static int[] MakeOffsetTable(byte[] needle)
        {
            int[] table = new int[needle.Length];
            int lastPrefixPosition = needle.Length;
            for (int i = needle.Length; i > 0; --i)
            {
                if (IsPrefix(needle, i))
                {
                    lastPrefixPosition = i;
                }
                table[needle.Length - i] = lastPrefixPosition - i + needle.Length;
            }
            for (int i = 0; i < needle.Length - 1; ++i)
            {
                int slen = SuffixLength(needle, i);
                table[slen] = needle.Length - 1 - i + slen;
            }
            return table;
        }

        /// <summary>
        /// Is needle[p:end] a prefix of needle?
        /// </summary>
        /// <param name="needle"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool IsPrefix(byte[] needle, int p)
        {
            for (int i = p, j = 0; i < needle.Length; ++i, ++j)
            {
                if (needle[i] != needle[j])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the maximum length of the substring ends at p and is a suffix.
        /// (good suffix rule)
        /// </summary>
        /// <param name="needle"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static int SuffixLength(byte[] needle, int p)
        {
            int len = 0;
            for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
            {
                len += 1;
            }
            return len;
        }
    }
}
