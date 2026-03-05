using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RGSK
{
    public class EnumFlagsAttribute : PropertyAttribute { }

    public class EnumFlags
    {
        public static List<int> GetSelectedIndexes<T>(T val) where T : IConvertible
        {
            var selectedIndexes = new List<int>();

            for (int i = 0; i < System.Enum.GetValues(typeof(T)).Length; i++)
            {
                int layer = 1 << i;
                if ((Convert.ToInt32(val) & layer) != 0)
                {
                    selectedIndexes.Add(i);
                }
            }

            return selectedIndexes;
        }
    }
}