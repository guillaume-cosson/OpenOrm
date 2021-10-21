using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extensions
    {
        public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        {
            List<List<T>> result = new List<List<T>>();

            if (list.Count < chunkSize || chunkSize <= 0)
            {
                result.Add(list);
                return result;
            }

            int currentChunkSize = 0;

            List<T> batch = new List<T>();
            foreach (T item in list)
            {
                if (currentChunkSize == chunkSize)
                {
                    result.Add(batch);
                    batch = new List<T>();
                    currentChunkSize = 0;
                }

                batch.Add(item);

                currentChunkSize++;
            }

            if (batch.Count > 0)
            {
                result.Add(batch);
            }

            return result;
        }
    }
}
