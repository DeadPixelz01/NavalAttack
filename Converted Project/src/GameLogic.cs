using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SwinGameSDK;

namespace MyGame
{
    internal static class GameLogic
    {
        public static void Main()
        {
            // opens a new graphics window
            SwinGame.OpenGraphicsWindow("Naval Attack", 800, 600);

            // load Resources
            GameResources.LoadResources();
            SwinGame.PlayMusic(GameResources.GameMusic("Background"));

            // game Loop
            do
            {
                GameController.HandleUserInput();
                GameController.DrawScreen();
            }
            while (!SwinGame.WindowCloseRequested() || GameController.CurrentState == GameState.Quitting);
            SwinGame.StopMusic();

            // Free Resources and Close Audio, to end the program.
            GameResources.FreeResources();
        }
    }
}
