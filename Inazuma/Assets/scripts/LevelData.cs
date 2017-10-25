using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class LevelData
{

    private static Dictionary<int, Dictionary<string, Dictionary<string, int>>> dict;
    //int level -> string objectCategory (checkpoint, enemy, door, etc) -> string objectName (MUST BE UNIQUE) -> Vector2<int,int> (X is current state, Y is defaultState)

    public static void addItem(int level, string objectCategory, string objectName, int defaultState)
    {
        if (dict == null)
        {
            dict = new Dictionary<int, Dictionary<string, Dictionary<string, int>>>();
        }
        if (!dict.ContainsKey(level))
        {
            dict[level] = new Dictionary<string, Dictionary<string, int>>();
        }
        if (!dict[level].ContainsKey(objectCategory))
        {
            dict[level][objectCategory] = new Dictionary<string, int>();
        }
        if (!dict[level][objectCategory].ContainsKey(objectName))
        {
            dict[level][objectCategory][objectName] = defaultState;
        }
        //if this value does not exist, add the supplied default value
    }
    public static int getState(int level, string objectCategory, string objectName)
    {
        return dict[level][objectCategory][objectName];
    }
    public static void setState(int level, string objectCategory, string objectName, int newState)
    {
        dict[level][objectCategory][objectName] = newState;
    }
    public static void resetLevel(int level)
    {
        dict[level].Clear();
    }
    public static void resetAll()
    {
        dict.Clear();
    }


}

