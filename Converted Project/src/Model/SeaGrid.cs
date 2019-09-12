
// The SeaGrid is the grid upon which the ships are deployed.
// The grid is viewable via the ISeaGrid interface as a read only
// grid. This can be used in conjuncture with the SeaGridAdapter to
// mask the position of the ships.

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
using MyGame.Model;

namespace MyGame.Model
{
    public class SeaGrid : ISeaGrid
    {
        private const int _WIDTH = 10;
        private const int _HEIGHT = 10;

        private readonly Tile[,] _GameTiles;
        private readonly Dictionary<ShipName, Ship> _Ships;
        private int _ShipsKilled = 0;

        int ISeaGrid.Height => Height;

        /// The sea grid has changed and should be redrawn.
        public event EventHandler Changed;

        /// The width of the sea grid.
        public int Width => _WIDTH;

        /// The height of the sea grid
        public int Height => _HEIGHT;

        // ShipsKilled returns the number of ships killed
        public int ShipsKilled { get; private set; } = 0;

        // Show the tile view
        public TileView Item(int x, int y)
        {
            return _GameTiles[x, y].View;
        }

        /// AllDeployed checks if all the ships are deployed
        public bool AllDeployed
        {
            get
            {
                foreach (var s in _Ships.Values)
                {
                    if (!s.IsDeployed)
                        return false;
                }
                return true;
            }
        }

        /// SeaGrid constructor, a sea grid has a number of tiles stored in an array
        public SeaGrid(Dictionary<ShipName, Ship> ships)
        {
            // fill array with empty Tiles
            _GameTiles = new Tile[Width, Height];

            //fill array with empty Tiles
            int i = 0;
            for (i = 0; i <= Width - 1; i++)
            {
                for (int j = 0; j <= Height - 1; j++)
                {
                    _GameTiles[i, j] = new Tile(i, j, null);
                }
            }

            _Ships = ships;
        }

        /// MoveShips allows for ships to be placed on the sea grid
        public void MoveShip(int row, int col, ShipName ship, Direction direction)
        {
            var newShip = _Ships[ship];
            newShip.Remove();
            AddShip(row, col, direction, newShip);
        }

        /// AddShip add a ship to the SeaGrid
        private void AddShip(int row, int col, Direction direction, Ship newShip)
        {
            try
            {
                var size = newShip.Size;
                var currentRow = row;
                var currentCol = col;

                var dRow = default(int);
                var dCol = default(int);
                if (direction == Direction.LeftRight)
                {
                    dRow = 0;
                    dCol = 1;
                }
                else
                {
                    dRow = 1;
                    dCol = 0;
                }

                // place ship's tiles in array and into ship object
                int i;
                var loopTo = size - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    if ((currentRow < 0) | (currentRow >= Width) | (currentCol < 0) | (currentCol >= Height))
                        throw new InvalidOperationException("Ship can't fit on the board");

                    _GameTiles[currentRow, currentCol].Ship = newShip;

                    currentCol += dCol;
                    currentRow += dRow;
                }

                newShip.Deployed(direction, row, col);
            }
            catch (Exception e)
            {
                newShip.Remove(); // if fails remove the ship
                throw new ApplicationException(e.Message);
            }

            finally
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public AttackResult HitTile(int row, int col)
        {
            try
            {
                // tile is already hit
                if (_GameTiles[row, col].Shot)
                    return new AttackResult(ResultOfAttack.ShotAlready, "have already attacked ["
                                                                        + Convert.ToString(col) + "," +
                                                                        Convert.ToString(row) + "]!",
                        row, col);

                _GameTiles[row, col].Shoot();

                // there is no ship on the tile
                if (_GameTiles[row, col].Ship == null)
                    return new AttackResult(ResultOfAttack.Miss, "missed", row, col);

                // all ship's tiles have been destroyed
                if (!_GameTiles[row, col].Ship.IsDestroyed)
                    return new AttackResult(ResultOfAttack.Hit, "hit something!", row, col);
                _GameTiles[row, col].Shot = true;
                ShipsKilled += 1;
                return new AttackResult(ResultOfAttack.Destroyed, _GameTiles[row, col].Ship,
                    "destroyed the enemy's", row, col);

                // else hit but not destroyed
                return new AttackResult(ResultOfAttack.Hit, "hit something!", row, col);
            }
            finally
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

