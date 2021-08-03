// Author: Joshua Nies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheoryKitCOBOL
{
    /// <summary>
    /// Utilities for Theory Kit COBOL.
    /// </summary>
    public static class COBOLUtils
    {
        /// <summary>
        /// Get whether a value has a numeric type.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value has a numeric type.</returns>
        public static bool IsNumeric(dynamic value) => value is int or float;

        /// <summary>
        /// Get whether a value is an alphabetic string.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value is an alphabetic string.</returns>
        public static bool IsAlphabetic(dynamic value)
        {
            if (value is not string) return false;

            return Regex.IsMatch(value, @"^[a-zA-Z\s]+$");
        }

        /// <summary>
        /// Get whether a value is an all-uppercase alphabetic string.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value is an all-uppercase alphabetic string.</returns>
        public static bool IsAlphabeticUpper(dynamic value)
        {
            if (value is not string) return false;

            return Regex.IsMatch(value, @"^[A-Z\s]+$");
        }

        /// <summary>
        /// Get whether a value is an all-lowercase alphabetic string.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value is an all-lowercase alphabetic string.</returns>
        public static bool IsAlphabeticLower(dynamic value)
        {
            if (value is not string) return false;

            return Regex.IsMatch(value, @"^[a-z\s]+$");
        }

        /// <summary>
        /// Get whether a value is an alphanumeric string.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Whether the value is an alphanumeric string.</returns>
        public static bool IsAlphanumeric(dynamic value)
        {
            if (value is not string) return false;

            return Regex.IsMatch(value, @"^[a-zA-Z0-9\s]+$");
        }

        /// <summary>
        /// Call a function in a given array based on a given number.
        /// <para>Replicates COBOL "GO TO DEPENDING ON" statements.</para>
        /// </summary>
        public static void CallByNum(int num, params Action[] funcs)
        {
            funcs[num]();
        }

        /// <summary>
        /// Wrapper for LINQ OrderBy method.
        /// </summary>
        /// <typeparam name="T">Enumerable type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="source">Enumerable.</param>
        /// <param name="selector">Selector.</param>
        /// <param name="direction">Sort direction.</param>
        /// <returns>Order enumerable.</returns>
        public static IOrderedEnumerable<T> OrderBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> selector,
            SortDirection direction
        )
        {
            if (direction == SortDirection.Ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        /// <summary>
        /// Wrapper for LINQ ThenBy method.
        /// </summary>
        /// <typeparam name="T">Enumerable type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="source">Enumerable.</param>
        /// <param name="selector">Selector.</param>
        /// <param name="direction">Sort direction.</param>
        /// <returns>Order enumerable.</returns>
        public static IOrderedEnumerable<T> ThenOrderBy<T, TKey>(
            this IOrderedEnumerable<T> source,
            Func<T, TKey> selector,
            SortDirection direction
        )
        {
            if (direction == SortDirection.Ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }

        /// <summary>
        /// Run the given sequence of functions in order.
        /// </summary>
        /// <param name="funcs"></param>
        public static void RunSequence(params Action[] funcs)
        {
            foreach (var f in funcs)
            {
                f();
            }
        }

        /// <summary>
        /// Run the given sequence of functions in order.
        /// </summary>
        /// <param name="funcs"></param>
        public static void RunSequenceUntil(Func<bool> conditionFunc, params Action[] funcs)
        {
            while (!conditionFunc())
            {
                foreach (var f in funcs)
                {
                    f();
                }
            }
        }
    }
}
