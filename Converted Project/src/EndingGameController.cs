using MyGame.Model;
using SwinGameSDK;

// the EndingGameController is responsible for managing the interactions at the end of a game.
namespace MyGame
{
    static class EndingGameController
    {
        // draw the end of the game screen, shows the win/lose state
        public static void DrawEndOfGame()
        {
            var toDraw = new Rectangle();

            UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer,
                true);
            UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

            toDraw.X = 0;
            toDraw.Y = 250;
            toDraw.Width = SwinGame.ScreenWidth();
            toDraw.Height = SwinGame.ScreenHeight();

            var whatShouldIPrint = GameController.HumanPlayer.IsDestroyed ? "YOU LOSE!" : "-- WINNER --";
            SwinGame.DrawText(whatShouldIPrint, Color.White, Color.Transparent,
                GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, toDraw);
        }

        // handle the input during the end of the game. Any interaction
        // will result in it reading in the highsSwinGame.
        public static void HandleEndOfGameInput()
        {
            if (!SwinGame.MouseClicked(MouseButton.LeftButton) && !SwinGame.KeyTyped(KeyCode.ReturnKey) &&
                !SwinGame.KeyTyped(KeyCode.EscapeKey)) return;
            HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
            GameController.EndCurrentState();
        }
    }
}
