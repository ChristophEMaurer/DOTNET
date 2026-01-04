using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BitcoinLib
{
    public static class ArrayHelpers
    {
        /// <summary>
        /// Joins the two lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static List<T> Join<T>(List<T> first, List<T> second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }

            List<T> result = first.Concat(second).ToList();

            return result;
        }

        /// <summary>
        /// Return the data of array data1: len1 bytes from index start1 and len2 bytes from array data2 starting at index start2.
        /// If len1 or len2 are -1 , then all data until the end of the array will be copied.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data1"></param>
        /// <param name="start1"></param>
        /// <param name="len1"></param>
        /// <param name="data2"></param>
        /// <param name="start2"></param>
        /// <param name="len2"></param>
        /// <returns></returns>
        public static T[] JoinArrays<T>(T[] data1, int start1, int len1, T[] data2, int start2, int len2)
        {
            if (len1 == -1)
            {
                len1 = data1.Length;
            }
            if (len2 == -1)
            {
                len2 = data1.Length;
            }

            int size = len1 + len2;
            T[] result = new T[size];

            Buffer.BlockCopy(data1, start1, result, 0, len1);
            Buffer.BlockCopy(data2, start2, result, len1, len2);

            return result;
        }

        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var result = new T[arrays.Sum(arr => arr.Length)];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                var arr = arrays[i];
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }

        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {
            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start)
        {
            return SubArray(arr, start, arr.Length - start);
        }
    }
}
