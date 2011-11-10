using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;
using AdvanceMath;

namespace Mathplex
{
    class NumberBlock : Block
    {
        public int Value = 0;

        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public NumberBlock( Grid grid, double width, double height, int num )
            : base( grid, width, height, Shape.Rectangle )
        {
            this.Image = Game.LoadImage("green" + num);
            this.Value = num;
            this.IsEdible = true;

            // labels can not be put as child objects :(
            //this.Color = Color.LightGreen;
            //Label label = new Label(Grid.CellSize.Y, Grid.CellSize.Y, num.ToString());
            //label.TextColor = Color.White;
            ////label.TextSize = new Vector(Grid.CellSize.Y, Grid.CellSize.Y);
            //label.TextScale = new Vector(3, 3);
            //this.
        }
    }
}
