using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState{

    // Use this for initialization
    public enum State
    {
        MainMenu, InGame, PlayerDead, LevelWon
    }
    public static int levelNumber = 1;
    public static int controlLayout = 0;
    public static int keyboardLayout = 0;
    public static bool playTransition = false;
        //layout 0 is right stick aiming
        //layout 1 is left stick aiming
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
    public static int getLevel()
    {
        return levelNumber;
    }
    public static void setLevel(int x)
    {
        levelNumber = x;
    }
}
