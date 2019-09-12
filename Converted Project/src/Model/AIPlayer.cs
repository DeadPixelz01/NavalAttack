
// The AIPlayer is a type of player. It can readomly deploy ships, it also has the
// functionality to generate coordinates and shoot at tiles

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

namespace MyGame.Model
{
    public abstract class AiPlayer : Player
    {
        // Location can store the location of the last hit made by an
        // AI Player. The use of which determines the difficulty.
        protected class Location
        {
            // The row of the shot
            public int Row { get; set; }

            // The column of the shot
            public int Column { get; set; }

            // Sets the last hit made to the local variables
            public Location(int row, int column)
            {
                Column = column;
                Row = row;
            }

            // Check if two locations are equal
            public static bool operator ==(Location @this, Location other)
            {
                return @this != null && other != null && @this.Row == other.Row && @this.Column == other.Column;
            }

            // Check if two locations are not equal
            public static bool operator !=(Location @this, Location other)
            {
                return @this == null || other == null || @this.Row != other.Row || @this.Column != other.Column;
            }
        }


        protected AiPlayer(BattleShipsGame game) : base(game)
        {
        }

        // Generate a valid row, column to shoot at
        protected abstract void GenerateCoords(ref int row, ref int column);

        // The last shot had the following result. Child classes can use this to prepare for the next shot.
        protected abstract void ProcessShot(int row, int col, AttackResult result);

        // The AI takes its attacks until its go is over.
        public override AttackResult Attack()
        {
            AttackResult result;
            var row = 0;
            var column = 0;

            do
            {
                Delay();
                GenerateCoords(ref row, ref column);
                result = _game.Shoot(row, column);
                ProcessShot(row, column, result);
            }
            // generate coordinates for shot// take shot
            while (result.Value != ResultOfAttack.Miss && result.Value != ResultOfAttack.GameOver
                                                       && !SwinGame.WindowCloseRequested());

            return result;
        }

        // Wait a short period to simulate the think time
        private static void Delay()
        {
            int i;
            for (i = 0; i <= 150; i++)
            {
                // Dont delay if window is closed
                if (SwinGame.WindowCloseRequested())
                    return;

                SwinGame.Delay(5);
                SwinGame.ProcessEvents();
                SwinGame.RefreshScreen();
            }
        }
    }
}

