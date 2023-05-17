using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        
        /// <summary>
        /// Changes all elements of IEnumerable by the change function
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="change">The way you want to change the stuff</param>
        /// <returns>An IEnumerable with all changes applied</returns>
        public static IEnumerable<T> Change<T>(this IEnumerable<T> enumerable, Func<T, T> change)
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(change, "change");

            foreach (var item in enumerable)
            {
                yield return change(item);
            }
        }

        /// <summary>
        /// Changes all elements of IEnumerable by the change function, that fullfill the where function
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="change">The way you want to change the stuff</param>
        /// <param name="where">The function to check where changes should be made</param>
        /// <returns>
        /// An IEnumerable with all changes applied
        /// </returns>
        public static IEnumerable<T> ChangeWhere<T>(this IEnumerable<T> enumerable, Func<T, T> change, Func<T, bool> @where)
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(change, "change");
            //ArgumentCheck.IsNullorWhiteSpace(@where, "where");

            foreach (var item in enumerable)
            {
                if (@where(item))
                {
                    yield return change(item);
                }
                else
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Changes all elements of IEnumerable by the change function that do not fullfill the except function
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="change">The way you want to change the stuff</param>
        /// <param name="where">The function to check where changes should not be made</param>
        /// <returns>
        /// An IEnumerable with all changes applied
        /// </returns>
        public static IEnumerable<T> ChangeExcept<T>(this IEnumerable<T> enumerable, Func<T, T> change, Func<T, bool> @where)
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(change, "change");
            //ArgumentCheck.IsNullorWhiteSpace(@where, "where");

            foreach (var item in enumerable)
            {
                if (!@where(item))
                {
                    yield return change(item);
                }
                else
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Update all elements of IEnumerable by the update function (only works with reference types)
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="update">The way you want to change the stuff</param>
        /// <returns>
        /// The same enumerable you passed in
        /// </returns>
        public static IEnumerable<T> Update<T>(this IEnumerable<T> enumerable, Action<T> update) where T : class
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(update, "update");
            foreach (var item in enumerable)
            {
                update(item);
            }
            return enumerable;
        }

        /// <summary>
        /// Update all elements of IEnumerable by the update function (only works with reference types)
        /// where the where function returns true
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="update">The way you want to change the stuff</param>
        /// <param name="where">The function to check where updates should be made</param>
        /// <returns>
        /// The same enumerable you passed in
        /// </returns>
        public static IEnumerable<T> UpdateWhere<T>(this IEnumerable<T> enumerable, Action<T> update, Func<T, bool> where) where T : class
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(update, "update");
            foreach (var item in enumerable)
            {
                if (where(item))
                {
                    update(item);
                }
            }
            return enumerable;
        }

        /// <summary>
        /// Update all elements of IEnumerable by the update function (only works with reference types)
        /// Except the elements from the where function
        /// </summary>
        /// <param name="enumerable">The enumerable where you want to change stuff</param>
        /// <param name="update">The way you want to change the stuff</param>
        /// <param name="where">The function to check where changes should not be made</param>
        /// <returns>
        /// The same enumerable you passed in
        /// </returns>
        public static IEnumerable<T> UpdateExcept<T>(this IEnumerable<T> enumerable, Action<T> update, Func<T, bool> where) where T : class
        {
            //ArgumentCheck.IsNullorWhiteSpace(enumerable, "enumerable");
            //ArgumentCheck.IsNullorWhiteSpace(update, "update");

            foreach (var item in enumerable)
            {
                if (!where(item))
                {
                    update(item);
                }
            }
            return enumerable;
        }

        //public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        //{
        //    List<List<T>> result = new List<List<T>>();

        //    if (list.Count() < chunkSize || chunkSize <= 0)
        //    {
        //        result.Add(list);
        //        return result;
        //    }

        //    int currentChunkSize = 0;

        //    List<T> chunk = new List<T>();
        //    foreach (T item in list)
        //    {
        //        if (currentChunkSize == chunkSize)
        //        {
        //            result.Add(chunk);
        //            chunk = new List<T>();
        //            currentChunkSize = 0;
        //        }

        //        chunk.Add(item);

        //        currentChunkSize++;
        //    }

        //    if (chunk.Count > 0)
        //    {
        //        result.Add(chunk);
        //    }

        //    return result;
        //}

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        
    }


    //public static class ArgumentCheck
    //{
    //    /// <summary>
    //    /// Checks if a value is string or any other object if it is string
    //    /// it checks for nullorwhitespace otherwhise it checks for null only
    //    /// </summary>
    //    /// <typeparam name="T">Type of the item you want to check</typeparam>
    //    /// <param name="item">The item you want to check</param>
    //    /// <param name="nameOfTheArgument">Name of the argument</param>
    //    public static void IsNullorWhiteSpace<T>(T item, string nameOfTheArgument = "")
    //    {

    //        Type type = typeof(T);
    //        if (type == typeof(string) ||
    //            type == typeof(String))
    //        {
    //            if (string.IsNullOrWhiteSpace(item as string))
    //            {
    //                throw new ArgumentException(nameOfTheArgument + " is null or Whitespace");
    //            }
    //        }
    //        else
    //        {
    //            if (item == null)
    //            {
    //                throw new ArgumentException(nameOfTheArgument + " is null");
    //            }
    //        }

    //    }
    //}
}
