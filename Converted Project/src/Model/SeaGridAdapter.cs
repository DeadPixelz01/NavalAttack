// The SeaGridAdapter allows for the change in a sea grid view. Whenever a ship is presented it
// changes the view into a sea tile instead of a ship tile.
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
    public class SeaGridAdapter : ISeaGrid
    {
        private readonly SeaGrid _MyGrid;

        // Create the SeaGridAdapter, with the grid, and it will allow it to be changed
        public SeaGridAdapter(SeaGrid grid)
        {
            _MyGrid = grid;
            _MyGrid.Changed += new EventHandler(MyGrid_Changed);
        }

        // MyGrid_Changed causes the grid to be redrawn by raising a changed event
        private void MyGrid_Changed(object sender, EventArgs e)
        {
            Changed?.Invoke(this, e);
        }


        // Changes the discovery grid. Where there is a ship we will sea water
        public TileView Item(int x, int y)
        {
            var result = _MyGrid.Item(x, y);
            return result == TileView.Ship ? TileView.Sea : result;
        }

        // Indicates that the grid has been changed
        public event EventHandler Changed;

        // Get the width of a tile
        public int Width => _MyGrid.Width;

        // Get the height of the tile
        public int Height => _MyGrid.Height;

        public AttackResult HitTile(int row, int col)
        {
            return _MyGrid.HitTile(row, col);
        }
    }
}

