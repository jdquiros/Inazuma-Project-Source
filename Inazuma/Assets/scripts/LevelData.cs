using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class LevelData
{

    private static Dictionary<int, Dictionary<string, int>> dict;
    //int level -> string objectCategory (checkpoint, enemy, door, etc) -> string objectName (MUST BE UNIQUE) -> Vector2<int,int> (X is current state, Y is defaultState)

    public static void addItem(int level, string objectName, int defaultState)
    {
        if (dict == null)
        {
            dict = new Dictionary<int, Dictionary<string, int>>();
        }
        if (!dict.ContainsKey(level))
        {
            dict[level] = new Dictionary<string,int>();
        }
        
        if (!dict[level].ContainsKey(objectName))
        {
            dict[level][objectName] = defaultState;
        }
        //if this value does not exist, add the supplied default value
    }
    public static int getState(int level, string objectName)
    {
        return dict[level][objectName];
    }
    public static void setState(int level, string objectName, int newState)
    {
        dict[level][objectName] = newState;
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

