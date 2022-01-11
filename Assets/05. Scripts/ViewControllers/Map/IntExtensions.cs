using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtensions {

    /// <summary>
    /// Checks to see if an init is between two other ints. [Inclusive]
    /// </summary>
    /// <param name="x"></param>
    /// <param name="lower"></param>
    /// <param name="upper"></param>
    /// <returns></returns>
    public static bool IsBetween(this int x, int lower, int upper) {

        bool n = false;

        if (x >= lower && x <= upper)
            n = true;


        return n;
    }

    public static int FindBiggest(this List<int> list) {
        if (list.Count > 0) {
            int biggest = list[0];
            foreach (int i in list)
            {
                if (i > biggest)
                {
                    biggest = i;
                }
            }

            return biggest;
        }

        return 0;

    }


}
