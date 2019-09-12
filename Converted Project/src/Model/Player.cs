
// Player has its own _PlayerGrid, and can see an _EnemyGrid, it can also check if
// all ships are deployed and if all ships are detroyed. A Player can also attach.

using System;
using System.Collections;
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
using MyGame.Model;

namespace MyGame.Model
{
    public class Player : IEnumerable<Ship>
    {
        protected readonly Random _random = new Random();
        private static readonly Dictionary<ShipName, Ship> _Ships = new Dictionary<ShipName, Ship>();
        protected BattleShipsGame _game;
        private SeaGrid _playerGrid;
        private ISeaGrid _enemyGrid;

        private int _shots;
        private int _hits;
        private int _misses;

        // Returns the game that the player is part of.
        public BattleShipsGame Game
        {
            get => _game;
            set => _game = value;
        }

        // Sets the grid of the enemy player
        public ISeaGrid Enemy
        {
            set => _enemyGrid = value;
        }

        // Constructor for player object. Initialises
        public Player(BattleShipsGame controller)
        {
            _playerGrid = new SeaGrid(_Ships);

            _game = controller;

            // for each ship add the ships name so the sea grid knows about them
            foreach (ShipName name in Enum.GetValues(typeof(ShipName)))
            {
                if (name != ShipName.None)
                    _Ships.Add(name, new Ship(name));
            }

            RandomizeDeployment();
        }

        // The EnemyGrid is a ISeaGrid because you shouldn't be allowed to see the enemies ships
        public ISeaGrid EnemyGrid
        {
            get
            {
                return _enemyGrid;
            }
            set
            {
                _enemyGrid = value;
            }
        }

        // The PlayerGrid is just a normal SeaGrid where the players ships can be deployed and seen
        public SeaGrid PlayerGrid
        {
            get
            {
                return _playerGrid;
            }
        }

        // ReadyToDeploy returns true if all ships are deployed
        public bool ReadyToDeploy
        {
            get
            {
                return _playerGrid.AllDeployed;
            }
        }

        public bool IsDestroyed
        {
            get
            {
                // check if all ships are destroyed... -1 for the none ship
                return _playerGrid.ShipsKilled == Enum.GetValues(typeof(ShipName)).Length - 1;
            }
        }

        // Returns the Player's ship with the given name.
        public Ship Ship(ShipName name)
        {
            return name == ShipName.None ? null : _Ships[name];
        }

        // The number of shots the player has made
        public int Shots { get; private set; }

        // Total number of shots that hit
        public int Hits { get; private set; }


        // Total number of shots that missed
        public int Missed { get; private set; }

        // calculate and return score integer
        public int Score
        {
            get
            {
                if (IsDestroyed)
                {
                    return 0;
                }
                else
                {
                    return (Hits * 12) - Shots - (PlayerGrid.ShipsKilled * 20);
                }
            }
        }

        //
        // Conversion error?
        //
        // Makes it possible to enumerate over the ships the player has.
        public IEnumerator<Ship> GetShipEnumerator()
        {
            var result = new Ship[_Ships.Values.Count + 1];
            _Ships.Values.CopyTo(result, 0);
            var lst = new List<Ship>();
            lst.AddRange(result);

            return lst.GetEnumerator();
        }
        // Makes it possible to enumerate over the ships the player has.
        IEnumerator<Ship> IEnumerable<Ship>.GetEnumerator()
        {
            // Potential Error?
            throw new NotImplementedException();
        }
        // Makes it possible to enumerate over the ships the player has.
        public IEnumerator GetEnumerator()
        {
            var result = new Ship[_Ships.Values.Count + 1];
            _Ships.Values.CopyTo(result, 0);
            var lst = new List<Ship>();
            lst.AddRange(result);

            return lst.GetEnumerator();
        }
        //
        // Conversion Error?
        //

        // Virtual Attack allows the player to shoot
        public virtual AttackResult Attack()
        {
            // human does nothing here...
            /* TODO Change to default(_) if this is not a reference type */
            return null;
        }

        // Shoot at a given row/column
        internal AttackResult Shoot(int row, int col)
        {
            Shots += 1;
            var result = EnemyGrid.HitTile(row, col);

            switch (result.Value)
            {
                case ResultOfAttack.Destroyed:
                case ResultOfAttack.Hit:
                    {
                        Hits += 1;
                        break;
                    }
                case ResultOfAttack.Miss:
                    {
                        Missed += 1;
                        break;
                    }
            }
            return result;
        }

        // randomise placement of ships
        public void RandomizeDeployment()
        {
            // for each ship to deploy in ship list
            foreach (ShipName shipToPlace in Enum.GetValues(typeof(ShipName)))
            {
                if (shipToPlace == ShipName.None)
                {
                    continue;
                }
                var placementSuccessful = false;

                // generate random position until the ship can be placed
                do
                {
                    var dir = _random.Next(2);
                    var x = _random.Next(0, 11);
                    var y = _random.Next(0, 11);
                    var heading = dir == 0 ? Direction.UpDown : Direction.LeftRight;

                    // try to place ship, if position unplaceable, generate new coordinates
                    try
                    {
                        PlayerGrid.MoveShip(x, y, shipToPlace, heading);
                        placementSuccessful = true;
                    }
                    catch
                    {
                        placementSuccessful = false;
                    }
                }
                while (!placementSuccessful);
            }
        }
    }
}