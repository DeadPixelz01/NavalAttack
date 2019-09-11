
// A Ship has all the details about itself. For example the ship name, size, number of hits taken and the location.
// Its able to add tiles, remove, hits taken and if its deployed and destroyed.
// Deployment information is supplied to allow ships to be drawn.

using System.Collections.Generic;

namespace MyGame.Model
{
    public class Ship
    {
        private readonly ShipName _shipName;
        private readonly List<Tile> _tiles;

        // The type of ship
        public string Name => _shipName == ShipName.AircraftCarrier ? "Aircraft Carrier" : _shipName.ToString();

        // The number of cells that this ship occupies.
        public int Size { get; }

        // The number of hits that the ship has taken.
        public int Hits { get; private set; } = 0;

        // The row location of the ship
        public int Row { get; private set; }

        // The Column location of the ship
        public int Column { get; private set; }

        // The Direction of the ship
        public Direction Direction { get; private set; }

        // Constructor for the ship
        public Ship(ShipName ship)
        {
            _shipName = ship;
            _tiles = new List<Tile>();

            // gets the ship size from the enumarator
            Size = (int) _shipName;
        }

        // Add tile adds the ship tile
        public void AddTile(Tile tile)
        {
            _tiles.Add(tile);
        }

        // Remove clears the tile back to a sea tile
        public void Remove()
        {
            foreach (var tile in _tiles)
            {
                tile.ClearShip();
            }
            _tiles.Clear();
        }

        // adds to the Hits counter integer
        public void Hit()
        {
            Hits += 1;
        }

        /// IsDeployed returns if the ships is deployed, if its deployed it has more than 0 tiles
        public bool IsDeployed => _tiles.Count > 0;

        // isDestroyed returns if the ship has been hit enough to be detroyed
        public bool IsDestroyed => Hits == Size;

        // Record that the ship is now deployed.
        internal void Deployed(Direction direction, int row, int col)
        {
            Row = row;
            Column = col;
            Direction = direction;
        }
    }
}
