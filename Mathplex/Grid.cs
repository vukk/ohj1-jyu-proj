using System;
using Jypeli;

namespace Mathplex
{
    /// @author Jypelin tekijät
    /// @author Un​to Ku​ur​an​ne
    /// @version 26.11.2011
    public class Grid : Jypeli.Grid
    {
        private int _Size;
        /// <summary>
        /// Gridin koko, käytetään sekä pituutena että leveytenä
        /// </summary>
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


        /// <summary>
        /// Palauttaa gridin solun keskelle snappaavan vektorin
        /// </summary>
        /// <param name="v">Vektori jonka läheistä keskikohtaa etsitään</param>
        /// <returns>Lähimmän gridin solun keskikohdan vektori</returns>
        public Vector SnapToCenter(Vector v)
        {
            Vector result;

            // Mathplex.TileMap.Execute() places objects in such way that the +- CellSize / 2 are necessary
            result.X = (Math.Round((v.X - (this.CellSize.X / 2)) / this.CellSize.X) * this.CellSize.X) + (this.CellSize.X / 2);
            result.Y = (Math.Round((v.Y + (this.CellSize.Y / 2)) / this.CellSize.Y) * this.CellSize.Y) - (this.CellSize.Y / 2);

            return result;
        }
    }
}
