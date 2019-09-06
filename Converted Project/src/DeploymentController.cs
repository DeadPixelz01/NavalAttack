using System;
using NavalStrike;
using SwinGameSDK;

namespace MyGame
{
    // The DeploymentController controls the players actions
    // during the deployment phase.
    static class DeploymentController
    {
        private const int ShipsTop = 98;
        private const int ShipsLeft = 20;
        private const int ShipsHeight = 90;
        private const int ShipsWidth = 300;

        private const int TopButtonsTop = 72;
        private const int TopButtonsHeight = 46;

        private const int PlayButtonLeft = 693;
        private const int PlayButtonWidth = 80;

        private const int UpDownButtonLeft = 410;
        private const int LeftRightButtonLeft = 350;

        private const int RandomButtonLeft = 547;
        private const int RandomButtonWidth = 51;

        private const int DirButtonsWidth = 47;

        private const int TextOffset = 5;

        private static Direction _currentDirection = Direction.UpDown;
        private static ShipName _selectedShip = ShipName.Tug;

        // handles user input for the deployment phase of the game.
        // involves selecting the ships, deploying ships, changing the direction
        // of the ships to add, randomising deployment, end then ending
        // deployment
        public static void HandleDeploymentInput()
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                GameController.AddNewState(GameState.ViewingGameMenu);
            }

            if (SwinGame.KeyTyped(KeyCode.UpKey) || SwinGame.KeyTyped(KeyCode.DownKey))
            {
                _currentDirection = Direction.UpDown;
            }

            if (SwinGame.KeyTyped(KeyCode.LeftKey) || SwinGame.KeyTyped(KeyCode.RightKey))
            {
                _currentDirection = Direction.LeftRight;
            }

            if (SwinGame.KeyTyped(KeyCode.RKey))
            {
                GameController.HumanPlayer.RandomizeDeployment();
            }

            if (!SwinGame.MouseClicked(MouseButton.LeftButton)) return;
            var selected = GetShipMouseIsOver();
            if (selected != ShipName.None)
                _selectedShip = selected;
            else
                DoDeployClick();

            if (GameController.HumanPlayer.ReadyToDeploy &&
                UtilityFunctions.IsMouseInRectangle(PlayButtonLeft, TopButtonsTop, PlayButtonWidth, TopButtonsHeight))
            {
                GameController.EndDeployment();
            }
            else if (UtilityFunctions.IsMouseInRectangle(UpDownButtonLeft, TopButtonsTop, DirButtonsWidth, TopButtonsHeight))
            {
                _currentDirection = Direction.LeftRight;
            }
            else if (UtilityFunctions.IsMouseInRectangle(LeftRightButtonLeft, TopButtonsTop, DirButtonsWidth, TopButtonsHeight))
            {
                _currentDirection = Direction.LeftRight;
            }
            else if (UtilityFunctions.IsMouseInRectangle(RandomButtonLeft, TopButtonsTop, RandomButtonWidth, TopButtonsHeight))
            {
                GameController.HumanPlayer.RandomizeDeployment();
            }
        }

        ///     The user has clicked somewhere on the screen, check if its is a deployment and deploy
        ///     the current ship if that is the case.
        ///     If the click is in the grid it deploys to the selected location
        ///     with the indicated direction
        private static void DoDeployClick()
        {
            var mouse = SwinGame.MousePosition();

            // Calculate the row/col clicked
            var row = Convert.ToInt32(Math.Floor((mouse.Y) / (double)(UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
            var col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (double)(UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 & row < GameController.HumanPlayer.PlayerGrid.Height)
            {
                if (col >= 0 & col < GameController.HumanPlayer.PlayerGrid.Width)
                {
                    // if in the area try to deploy
                    try
                    {
                        GameController.HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection);
                    }
                    catch (Exception ex)
                    {
                        Audio.PlaySoundEffect(GameResources.GameSound("Error"));
                        UtilityFunctions.Message = ex.Message;
                    }
                }
            }
        }

        /// <summary>
        ///     ''' Draws the deployment screen showing the field and the ships
        ///     ''' that the player can deploy.
        ///     ''' </summary>
        public static void DrawDeployment()
        {
            UtilityFunctions.DrawField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer, true);

            // Draw the Left/Right and Up/Down buttons
            SwinGame.DrawBitmap(
                _currentDirection == Direction.LeftRight
                    ? GameResources.GameImage("LeftRightButton")
                    : GameResources.GameImage("UpDownButton"), LeftRightButtonLeft, TopButtonsTop);

            // DrawShips
            foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
            {
                var i = Convert.ToInt32(Convert.ToInt32(sn) - 1);
                if (i < 0) continue;
                if (sn == _selectedShip)
                {
                    SwinGame.DrawBitmap(GameResources.GameImage("SelectedShip"), ShipsLeft, ShipsTop + i * ShipsHeight);
                }
            }

            // checks if the player is ready to deploy
            if (GameController.HumanPlayer.ReadyToDeploy)
            {
                SwinGame.DrawBitmap(GameResources.GameImage("PlayButton"), PlayButtonLeft, TopButtonsTop);
            }
            SwinGame.DrawBitmap(GameResources.GameImage("RandomButton"), RandomButtonLeft, TopButtonsTop);
            UtilityFunctions.DrawMessage();
        }


        // Gets the ship that the mouse is currently over in the selection panel.
        // The ship selected or none
        private static ShipName GetShipMouseIsOver()
        {
            foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
            {
                var i = Convert.ToInt32(Convert.ToInt32(sn) - 1);

                if (UtilityFunctions.IsMouseInRectangle(ShipsLeft, ShipsTop + i * ShipsHeight, ShipsWidth, ShipsHeight))
                {
                    return sn;
                }
            }
            return ShipName.None;
        }
    }
}
