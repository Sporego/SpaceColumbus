﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public interface INamed
    {
        string Name { get; }
    }

    public interface IIdentifiable
    {
        int GetId();
    }

    public static class Slicable
    {
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
}
