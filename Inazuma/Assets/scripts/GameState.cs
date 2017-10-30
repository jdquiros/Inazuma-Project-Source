﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState{

    // Use this for initialization
    public enum State
    {
        MainMenu, InGame, PlayerDead
    }
    public static State gameState = State.MainMenu;

    public static bool compareState(int x)
    {
        return gameState == (State)x;
    }
    public static bool compareState(State x)
    {
        return gameState == x;
    }
    public static int getState()
    {
        return (int)gameState;
    }
    public static void setState(int x)
    {
        gameState = (State)x;
    }
    public static void setState(State x)
    {
        gameState = x;
    }
}
