using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;

namespace MyGame
{
    // the battle phase is handled by the DiscoveryController.
    static class DiscoveryController
    {
        // handles input during the discovery phase of the game.
        // escape opens the game menu. Clicking the mouse will
        // attack a location.
        public static void HandleDiscoveryInput()
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                GameController.AddNewState(GameState.ViewingGameMenu);
            }

            if (SwinGame.MouseClicked(MouseButton.LeftButton))
            {
                DoAttack();
            }
        }

        // attack the location that the mouse if over
        private static void DoAttack()
        {
            var mouse = SwinGame.MousePosition();

            // calculate the row/col clicked
            var row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) /
                                                 (double)(UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
            var col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) /
                                                 (double)(UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height)
            {
                if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width)
                    GameController.Attack(row, col);
            }
        }
        
        // draws the game during the attack phase.
        public static void DrawDiscovery()
        {
            const int scoresLeft = 172;
            const int shotsTop = 157;
            const int hitsTop = 206;
            const int splashTop = 256;

            if ((SwinGame.KeyDown(KeyCode.LeftShiftKey) || SwinGame.KeyDown(KeyCode.RightShiftKey)) &&
                SwinGame.KeyDown(KeyCode.CKey))
            {
                UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer,
                    true);
            }
            else
            {
                UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer,
                    false);
            }

            UtilityFunctions.DrawSmallField(GameController.HumanPlayer.EnemyGrid, GameController.HumanPlayer);
            UtilityFunctions.DrawMessage();

            SwinGame.DrawText(GameController.HumanPlayer.Shots.ToString(), Color.White,
                GameResources.GameFont("Menu"), scoresLeft, shotsTop);
            SwinGame.DrawText(GameController.HumanPlayer.Hits.ToString(), Color.White,
                GameResources.GameFont("Menu"), scoresLeft, hitsTop);
            SwinGame.DrawText(GameController.HumanPlayer.Missed.ToString(), Color.White,
                GameResources.GameFont("Menu"), scoresLeft, splashTop);
        }
    }
}