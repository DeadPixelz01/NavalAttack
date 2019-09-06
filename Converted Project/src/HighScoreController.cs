using System;
using System.Collections.Generic;
using System.IO;
using SwinGameSDK;

// controls displaying and collecting high score data.
// data is saved to a file.

namespace MyGame
{
    internal static class HighScoreController
    {
        private const int NameWidth = 3;
        private const int ScoresLeft = 490;

        // the score structure is used to keep the name and score of the top players together.
        private struct Score : IComparable
        {
            public string NAME;
            public int VALUE;

            // allows scores to be compared to facilitate sorting
            public int CompareTo(object obj)
            {
                if (obj is Score other)
                {
                    return other.VALUE - this.VALUE;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static readonly List<Score> SCORES = new List<Score>();
        // loads the scores from the highscores text file.
        // the format is # of scores NNNSSS where NNN is the name and SSS is the score
        private static void LoadScores()
        {
            var filename = SwinGame.PathToResource("highscores.txt");

            var input = new StreamReader(filename);

            // read in the # of scores
            var numScores = Convert.ToInt32(input.ReadLine());

            SCORES.Clear();

            int i;
            for (i = 1; i <= numScores; i++)
            {
                Score s;
                var line = input.ReadLine();

                s.NAME = line.Substring(0, NameWidth);
                s.VALUE = Convert.ToInt32(line.Substring(NameWidth));
                SCORES.Add(s);
            }
            input.Close();
        }

        // saves the scores back to the highscores text file.
        // the format is # of scores NNNSSS Where NNN is the name and SSS is the score
        private static void SaveScores()
        {
            var filename = SwinGame.PathToResource("highscores.txt");

            var output = new StreamWriter(filename);

            output.WriteLine(SCORES.Count);

            foreach (var s in SCORES)
                output.WriteLine(s.NAME + s.VALUE);

            output.Close();
        }

        // draws the high scores to the screen.
        public static void DrawHighScores()
        {
            const int scoresHeading = 40;
            const int scoresTop = 80;
            const int scoreGap = 30;

            if (SCORES.Count == 0)
                LoadScores();

            SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"),
                ScoresLeft, scoresHeading);

            // for all of the scores
            int i;
            for (i = 0; i <= SCORES.Count - 1; i++)
            {
                var s = SCORES[i];
                // for scores 1 - 9 use 01 - 09
                if (i < 9)
                    SwinGame.DrawText(" " + (i + 1) + ":   " + s.NAME + "   " + s.VALUE, Color.White,
                        GameResources.GameFont("Courier"), ScoresLeft, scoresTop + i * scoreGap);
                else
                    SwinGame.DrawText(i + 1 + ":   " + s.NAME + "   " + s.VALUE, Color.White,
                        GameResources.GameFont("Courier"), ScoresLeft, scoresTop + i * scoreGap);
            }
        }

        // handles the user input during the top score screen.
        public static void HandleHighScoreInput()
        {
            if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.EscapeKey) ||
                SwinGame.KeyTyped(KeyCode.ReturnKey))
                GameController.EndCurrentState();
        }

        // read the user's name for their highsSwinGame.
        // this verifies if the score is a highsSwinGame.
        public static void ReadHighScore(int value)
        {
            const int entryTop = 500;

            if (SCORES.Count == 0)
            {
                LoadScores();
            }

            // is it a high score
            if (value > SCORES[SCORES.Count - 1].VALUE)
            {
                var s = new Score {VALUE = value};

                GameController.AddNewState(GameState.ViewingHighScores);

                var x = ScoresLeft + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: ");

                SwinGame.StartReadingText(Color.White, NameWidth, GameResources.GameFont("Courier"),
                    x, entryTop);

                // Read the text from the user
                while (SwinGame.ReadingText())
                {
                    SwinGame.ProcessEvents();

                    UtilityFunctions.DrawBackground();
                    DrawHighScores();
                    SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"),
                        ScoresLeft, entryTop);
                    SwinGame.RefreshScreen();
                }

                s.NAME = SwinGame.TextReadAsASCII();

                if (s.NAME.Length < 3)
                    s.NAME += new string(Convert.ToChar(" "), 3 - s.NAME.Length);

                SCORES.RemoveAt(SCORES.Count - 1);
                SCORES.Add(s);
                SCORES.Sort();

                GameController.EndCurrentState();
            }
        }
    }
}