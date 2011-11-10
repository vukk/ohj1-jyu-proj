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
    public class EvalBlock : Block 
    {
        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public EvalBlock(Grid grid, double width, double height, Shape shape)
            : base(grid, width, height, shape)
        {
            this.IsSolid = true;
            this.Color = Color.Orange;
        }
    }
}
