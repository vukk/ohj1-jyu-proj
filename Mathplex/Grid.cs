using System;
using Jypeli;

namespace Mathplex
{
    public class Grid : Jypeli.Grid
    {
        private int _Size;
        public int Size
        {
            get { return this._Size; }
            set
            {
                this.CellSize = new Vector(value, value);
                this._Size = value;
            }
        }

        public Grid()
        {
            Color = Color.Black;
            Size = 10;
            CellSize = new Vector(Size, Size);
        }

        public Grid(int size)
        {
            Color = Color.Black;
            Size = size;
            CellSize = new Vector(Size, Size);
        }

        public new Vector SnapToLines(Vector v)
        {
            Vector result;
            // TileMap.Execute() places objects in such way that the +- CellSize / 2 are necessary
            result.X = (Math.Round(v.X / this.CellSize.X) * this.CellSize.X) - (this.CellSize.X / 2);
            result.Y = (Math.Round(v.Y / this.CellSize.Y) * this.CellSize.Y) + (this.CellSize.Y / 2);
            return result;
        }
    }
}
