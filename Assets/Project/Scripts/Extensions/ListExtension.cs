using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    //
    public static bool IsContainIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;
}
