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

// The menu controller handles the drawing and user interactions
// from the menus in the game. These include the main menu, game menu and the settings m,enu.
namespace MyGame
{
    static class MenuController
    {
        // the menu structure for the game.
        // these are the text captions for the menu items.
        private static readonly string[][] MENU_STRUCTURE =
        {
            new [] { "PLAY", "SETUP", "SCORES", "QUIT" },
            new [] { "RETURN", "SURRENDER", "QUIT" },
            new [] { "EASY", "MEDIUM", "HARD" }
        };

        private const int MenuTop = 575;
        private const int MenuLeft = 30;
        private const int MenuGap = 0;
        private const int ButtonWidth = 75;
        private const int ButtonHeight = 15;
        private const int ButtonSep = ButtonWidth + MenuGap;
        private const int TextOffset = 0;

        private const int MainMenu = 0;
        private const int GameMenu = 1;
        private const int SetupMenu = 2;

        private const int MainMenuPlayButton = 0;
        private const int MainMenuSetupButton = 1;
        private const int MainMenuTopScoresButton = 2;
        private const int MainMenuQuitButton = 3;

        private const int SetupMenuEasyButton = 0;
        private const int SetupMenuMediumButton = 1;
        private const int SetupMenuHardButton = 2;
        private const int SetupMenuExitButton = 3;

        private const int GameMenuReturnButton = 0;
        private const int GameMenuSurrenderButton = 1;
        private const int GameMenuQuitButton = 2;

        private static readonly Color MENU_COLOR = SwinGame.RGBAColor(2, 167, 252, 255);
        private static readonly Color HIGHLIGHT_COLOR = SwinGame.RGBAColor(1, 57, 86, 255);

        // handles the processing of user input when the main menu is showing
        public static void HandleMainMenuInput()
        {
            HandleMenuInput(MainMenu, 0, 0);
        }

        // handles the processing of user input when the main menu is showing
        public static void HandleSetupMenuInput()
        {
            var handled = HandleMenuInput(SetupMenu, 1, 1);

            if (!handled)
            {
                HandleMenuInput(MainMenu, 0, 0);
            }
        }

        // handle input in the game menu.
        // player can return to the game, surrender, or quit entirely
        public static void HandleGameMenuInput()
        {
            HandleMenuInput(GameMenu, 0, 0);
        }

        // handles input for the specified menu.
        private static bool HandleMenuInput(int menu, int level, int xOffset)
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                GameController.EndCurrentState();
                return true;
            }

            if (SwinGame.MouseClicked(MouseButton.LeftButton))
            {
                int i;
                for (i = 0; i <= MENU_STRUCTURE[menu].Length - 1; i++)
                {
                    // IsMouseOver the current indexed button of the menu
                    if (IsMouseOverMenu(i, level, xOffset))
                    {
                        PerformMenuAction(menu, i);
                        return true;
                    }
                }

                if (level > 0)
                    // none clicked - so end this sub menu
                    GameController.EndCurrentState();
            }

            return false;
        }

        // draws the main menu to the screen.
        public static void DrawMainMenu()
        {
            // clears the Screen to Black
            SwinGame.DrawText("Main Menu", Color.White, GameResources.GameFont("ArialLarge"), 50, 50);
            DrawButtons(MainMenu);
        }

        // draws the Game menu to the screen
        public static void DrawGameMenu()
        {
            // clears the Screen to Black
            SwinGame.DrawText("Paused", Color.White, GameResources.GameFont("ArialLarge"), 50, 50);

            DrawButtons(GameMenu);
        }

        // draws the settings menu to the screen.
        // also shows the main menu
        public static void DrawSettings()
        {
            // Clears the Screen to Black
            SwinGame.DrawText("Settings", Color.White, GameResources.GameFont("ArialLarge"), 50, 50);
            DrawButtons(MainMenu);
            DrawButtons(SetupMenu, 1, 1);
        }

        // draw the buttons associated with a top level menu.
        private static void DrawButtons(int menu)
        {
            DrawButtons(menu, 0, 0);
        }

        // Draws the menu at the indicated level.
        // The menu text comes from the _menuStructure field. The level indicates the height
        // of the menu, to enable sub menus. The xOffset repositions the menu horizontally
        // to allow the submenus to be positioned correctly.
        private static void DrawButtons(int menu, int level, int xOffset)
        {
            var toDraw = new Rectangle();

            var btnTop = MenuTop - (MenuGap + ButtonHeight) * level;
            int i;
            for (i = 0; i <= MENU_STRUCTURE[menu].Length - 1; i++)
            {
                var btnLeft = MenuLeft + ButtonSep * (i + xOffset);
                SwinGame.FillRectangle(Color.White, btnLeft, btnTop, ButtonWidth, ButtonHeight);
                toDraw.X = btnLeft + TextOffset;
                toDraw.Y = btnTop + TextOffset;
                toDraw.Width = ButtonWidth;
                toDraw.Height = ButtonHeight;
                SwinGame.DrawText(MENU_STRUCTURE[menu][i], MENU_COLOR, Color.Black,
                    GameResources.GameFont("Menu"), FontAlignment.AlignCenter, toDraw);

                if (SwinGame.MouseDown(MouseButton.LeftButton) & IsMouseOverMenu(i, level, xOffset))
                {
                    SwinGame.DrawRectangle(HIGHLIGHT_COLOR, btnLeft, btnTop, ButtonWidth, ButtonHeight);
                }
            }
        }

        // determined if the mouse is over one of the button in the main menu.
        private static bool IsMouseOverButton(int button)
        {
            return IsMouseOverMenu(button, 0, 0);
        }

        // checks if the mouse is over one of the buttons in a menu.
        private static bool IsMouseOverMenu(int button, int level, int xOffset)
        {
            var btnTop = MenuTop - (MenuGap + ButtonHeight) * level;
            var btnLeft = MenuLeft + ButtonSep * (button + xOffset);

            return UtilityFunctions.IsMouseInRectangle(btnLeft, btnTop, ButtonWidth, ButtonHeight);
        }

        // a button has been clicked, perform the associated action.
        private static void PerformMenuAction(int menu, int button)
        {
            switch (menu)
            {
                case MainMenu:
                    PerformMainMenuAction(button);
                    break;
                case SetupMenu:
                    PerformSetupMenuAction(button);
                    break;
                case GameMenu:
                    PerformGameMenuAction(button);
                    break;
            }
        }

        // the main menu was clicked, perform the button's action.
        private static void PerformMainMenuAction(int button)
        {
            switch (button)
            {
                case MainMenuPlayButton:
                    GameController.StartGame();
                    break;
                case MainMenuSetupButton:
                    GameController.AddNewState(GameState.AlteringSettings);
                    break;
                case MainMenuTopScoresButton:
                    GameController.AddNewState(GameState.ViewingHighScores);
                    break;
                case MainMenuQuitButton:
                    GameController.EndCurrentState();
                    break;
            }
        }

        // the setup menu was clicked, perform the button's action.
        private static void PerformSetupMenuAction(int button)
        {
            switch (button)
            {
                case SetupMenuEasyButton:
                    {
                        GameController.SetDifficulty(AIOption.Hard);
                        break;
                    }
                case SetupMenuMediumButton:
                    {
                        GameController.SetDifficulty(AIOption.Hard);
                        break;
                    }
                case SetupMenuHardButton:
                    {
                        GameController.SetDifficulty(AIOption.Hard);
                        break;
                    }
            }
            // Always end state - handles exit button as well
            GameController.EndCurrentState();
        }

        // the game menu was clicked, perform the button's action.
        private static void PerformGameMenuAction(int button)
        {
            switch (button)
            {
                case GameMenuReturnButton:
                    GameController.EndCurrentState();
                    break;
                case GameMenuSurrenderButton:
                    // end game menu
                    GameController.EndCurrentState();
                    // end game
                    GameController.EndCurrentState();
                    break;
                case GameMenuQuitButton:
                    GameController.AddNewState(GameState.Quitting);
                    break;
            }
        }
    }
}

