/// <summary>
/// ''' The AIMediumPlayer is a type of AIPlayer where it will try and destroy a ship
/// ''' if it has found a ship
/// ''' </summary>
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

namespace MyGame.Model
{
   public class AiMediumPlayer : AiPlayer
    {
        // Private enumerator for AI states. currently there are two states,
        // the AI can be searching for a ship, or if it has found a ship it will target the same ship
        private enum AiStates
        {
            Searching,
            TargetingShip
        }

        private AiStates _CurrentState = AiStates.Searching;
        private readonly Stack<Location> _Targets = new Stack<Location>();

        public AiMediumPlayer(BattleShipsGame controller) : base(controller)
        {
        }

        // GenerateCoordinates should generate random shooting coordinates
        // only when it has not found a ship, or has destroyed a ship and needs new shooting coordinates
        protected override void GenerateCoords(ref int row, ref int column)
        {
            do
            {
                // check which state the AI is in and upon that choose which coordinate generation
                // method will be used.
                switch (_CurrentState)
                {
                    case AiStates.Searching:
                        {
                            SearchCoords(ref row, ref column);
                            break;
                        }
                    case AiStates.TargetingShip:
                        {
                            TargetCoords(ref row, ref column);
                            break;
                        }
                    default:
                        {
                            throw new ApplicationException("AI has gone in an invalid state");
                            break;
                        }
                }
            }
            // while inside the grid and not a sea tile do the search
            while ((row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width ||
                    EnemyGrid.Item(row, column) != TileView.Sea));
        }

        // TargetCoords is used when a ship has been hit and it will try and destroy this ship
        private void TargetCoords(ref int row, ref int column)
        {
            var l = _Targets.Pop();

            if ((_Targets.Count == 0))
                _CurrentState = AiStates.Searching;
            row = l.Row;
            column = l.Column;
        }

        // SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
        private void SearchCoords(ref int row, ref int column)
        {
            row = Convert.ToInt32(_random.Next(0, EnemyGrid.Height));
            column = Convert.ToInt32(_random.Next(0, EnemyGrid.Width));
        }

        // ProcessShot will be called upon when a ship is found.
        // It will create a stack with targets it will try to hit. These targets will be around the
        // tile that has been hit.
        protected override void ProcessShot(int row, int col, AttackResult result)
        {
            switch (result.Value)
            {
                case ResultOfAttack.Hit:
                    _CurrentState = AiStates.TargetingShip;
                    AddTarget(row - 1, col);
                    AddTarget(row, col - 1);
                    AddTarget(row + 1, col);
                    AddTarget(row, col + 1);
                    break;
                case ResultOfAttack.ShotAlready:
                    throw new ApplicationException("Error in AI");
            }
        }

        // AddTarget will add the targets it will shoot onto a stack
        private void AddTarget(int row, int column)
        {
            if (row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width &&
                EnemyGrid.Item(row, column) == TileView.Sea)
                _Targets.Push(new Location(row, column));
        }
    }
}

