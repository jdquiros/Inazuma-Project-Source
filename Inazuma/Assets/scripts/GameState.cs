using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState{

	public enum State
    {
        MainMenu, Gameplay, DeathScreen
    }
    public static int gameState = (int)State.MainMenu;

    public static int getState()
    {
        return gameState;
    }
    public static void setState(int x)
    {
        gameState = x;
    }
    public static void setState(State x)
    {
        gameState = (int)x;
    }
}
