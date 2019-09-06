
// This includes a number of utility methods for drawing and interacting with the Mouse.
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
    internal static class UtilityFunctions
    {
        public const int FieldTop = 122;
        public const int FieldLeft = 349;
        private const int FieldWidth = 418;
        private const int FieldHeight = 418;

        private const int MessageTop = 548;

        public const int CellWidth = 40;
        public const int CellHeight = 40;
        public const int CellGap = 2;

        private const int ShipGap = 3;

        private static readonly Color SMALL_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
        private static readonly Color SMALL_SHIP = Color.Gray;
        private static readonly Color SMALL_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
        private static readonly Color SMALL_HIT = SwinGame.RGBAColor(169, 24, 37, 255);

        private static readonly Color LARGE_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
        private static readonly Color LARGE_SHIP = Color.Gray;
        private static readonly Color LARGE_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
        private static readonly Color LARGE_HIT = SwinGame.RGBAColor(252, 2, 3, 255);

        private static readonly Color OUTLINE_COLOR = SwinGame.RGBAColor(5, 55, 88, 255);
        private static readonly Color SHIP_FILL_COLOR = Color.Gray;
        private static readonly Color SHIP_OUTLINE_COLOR = Color.White;
        private static readonly Color MESSAGE_COLOR = SwinGame.RGBAColor(2, 167, 252, 255);

        private const int AnimationCells = 7;
        private const int FramesPerCell = 8;

        // determines if the mouse is in a given rectangle.
        public static bool IsMouseInRectangle(int x, int y, int w, int h)
        {
            var result = false;
            var mouse = SwinGame.MousePosition();

            // if the mouse is inline with the button horizontally
            if (mouse.X >= x & mouse.X <= x + w)
            {
                // check vertical position
                if (mouse.Y >= y & mouse.Y <= y + h)
                    result = true;
            }

            return result;
        }

        // draws a large field using the grid and the indicated player's ships.
        public static void DrawField(ISeaGrid grid, Player thePlayer, bool showShips)
        {
            DrawCustomField(grid, thePlayer, false, showShips, FieldLeft, FieldTop,
                FieldWidth, FieldHeight, CellWidth, CellHeight, CellGap);
        }

        // draws a small field, showing the attacks made and the locations of the player's ships
        public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
        {
            const int smallFieldLeft = 39;
            const int smallFieldTop = 373;
            const int smallFieldWidth = 166;
            const int smallFieldHeight = 166;
            const int smallFieldCellWidth = 13;
            const int smallFieldCellHeight = 13;
            const int smallFieldCellGap = 4;

            DrawCustomField(grid, thePlayer, true, true, smallFieldLeft, smallFieldTop,
            smallFieldWidth, smallFieldHeight, smallFieldCellWidth,
            smallFieldCellHeight, smallFieldCellGap);
        }

        private static void GridFixMethod(ISeaGrid grid, bool small, int row, int col, ref Color fillColor, ref bool draw)
        {
            switch (grid.get_Item(row, col))

            {
                case TileView.Ship:
                    draw = false;
                    break;
                case TileView.Miss:
                    fillColor = small ? SMALL_MISS : LARGE_MISS;
                    break;
                case TileView.Hit:
                    fillColor = small ? SMALL_HIT : LARGE_HIT;
                    break;
                case TileView.Sea:
                    if (small)
                    {
                        fillColor = SMALL_SEA;
                    }
                    else
                    {
                        draw = false;
                    }
                    break;
            }
        }

        // draws the player's grid and ships.
        private static void DrawCustomField(ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight, int cellGap)
        {
            SwinGame.FillRectangle(Color.Blue, left, top, width, height);
            int rowTop;
            int colLeft;

            // Draw the grid
            for (var row = 0; row <= 9; row++)
            {
                rowTop = top + (cellGap + cellHeight) * row;

                for (var col = 0; col <= 9; col++)
                {
                    colLeft = left + (cellGap + cellWidth) * col;
                    Color fillColor = null;
                    var draw = true;

                    GridFixMethod(grid, small, row, col, ref fillColor, ref draw);

                    if (draw)
                    {
                        SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
                        if (!small)
                        {
                            SwinGame.DrawRectangle(OUTLINE_COLOR, colLeft, rowTop, cellWidth, cellHeight);
                        }
                    }
                }
            }

            if (!showShips)
            {
                return;
            }

            // draw the ships
            foreach (Ship s in thePlayer)
            {
                if (s == null || !s.IsDeployed)
                    continue;
                rowTop = top + (cellGap + cellHeight) * s.Row + ShipGap;
                colLeft = left + (cellGap + cellWidth) * s.Column + ShipGap;


                var shipHeight = 0;
                var shipWidth = 0;
                string shipName = null;

                if (s.Direction == Direction.LeftRight)
                {
                    shipName = "ShipLR" + s.Size;
                    shipHeight = cellHeight - (ShipGap * 2);
                    shipWidth = (cellWidth + cellGap) * s.Size - (ShipGap * 2) - cellGap;
                }
                else
                {
                    // Up down
                    shipName = "ShipUD" + s.Size;
                    shipHeight = (cellHeight + cellGap) * s.Size - (ShipGap * 2) - cellGap;
                    shipWidth = cellWidth - (ShipGap * 2);
                }

                if (!small)
                {
                    SwinGame.DrawBitmap(GameResources.GameImage(shipName), colLeft, rowTop);
                }

                else
                {
                    SwinGame.FillRectangle(SHIP_FILL_COLOR, colLeft, rowTop, shipWidth, shipHeight);
                    SwinGame.DrawRectangle(SHIP_OUTLINE_COLOR, colLeft, rowTop, shipWidth, shipHeight);
                }
            }
        }

        // the message to display
        public static string Message { get; set; }

        // draws the message to the screen
        public static void DrawMessage()
        {
            SwinGame.DrawText(Message, MESSAGE_COLOR, GameResources.GameFont("Courier"), FieldLeft, MessageTop);
        }

        // draws the background for the current state of the game
        public static void DrawBackground()
        {
            switch (GameController.CurrentState)
            {
                case GameState.ViewingMainMenu:
                case GameState.ViewingGameMenu:
                case GameState.AlteringSettings:
                case GameState.ViewingHighScores:
                    {
                        SwinGame.DrawBitmap(GameResources.GameImage("Menu"), 0, 0);
                        break;
                    }
                case GameState.Discovering:
                case GameState.EndingGame:
                    {
                        SwinGame.DrawBitmap(GameResources.GameImage("Discovery"), 0, 0);
                        break;
                    }
                case GameState.Deploying:
                    {
                        SwinGame.DrawBitmap(GameResources.GameImage("Deploy"), 0, 0);
                        break;
                    }
                default:
                    {
                        SwinGame.ClearScreen();
                        break;
                    }
            }

            SwinGame.DrawFramerate(675, 585);
        }

        public static void AddExplosion(int row, int col)
        {
            AddAnimation(row, col, "Splash");
        }

        public static void AddSplash(int row, int col)
        {
            AddAnimation(row, col, "Splash");
        }

        private static readonly List<Sprite> ANIMATIONS = new List<Sprite>();

        private static void AddAnimation(int row, int col, string image)
        {
            var imgObj = GameResources.GameImage(image);
            imgObj.SetCellDetails(40, 40, 3, 3, 7);
            var animation = SwinGame.LoadAnimationScript("splash.txt");
            var s = SwinGame.CreateSprite(imgObj, animation);
            s.X = FieldLeft + col * (CellWidth + CellGap);
            s.Y = FieldTop + row * (CellHeight + CellGap);
            s.StartAnimation("splash");
            ANIMATIONS.Add(s);
        }

        public static void UpdateAnimations()
        {
            var ended = new List<Sprite>();
            foreach (var s in ANIMATIONS)
            {
                SwinGame.UpdateSprite(s);
                if (s.animationHasEnded)
                    ended.Add(s);
            }

            foreach (var s in ended)
            {
                ANIMATIONS.Remove(s);
                SwinGame.FreeSprite(s);
            }
        }

        public static void DrawAnimations()
        {
            foreach (var s in ANIMATIONS)
                SwinGame.DrawSprite(s);
        }

        public static void DrawAnimationSequence()
        {
            int i;
            for (i = 1; i <= AnimationCells * FramesPerCell; i++)
            {
                UpdateAnimations();
                GameController.DrawScreen();
            }
        }
    }
}

