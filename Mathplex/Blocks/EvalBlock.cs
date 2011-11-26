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
    /// @author Un​to Ku​ur​an​ne
    /// @version 26.11.2011
    /// <summary>
    /// Luokka evaluaatioblockille
    /// </summary>
    public class EvalBlock : Block 
    {
        /// <summary>
        /// Luo uuden Eval/Exit blockin
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        public EvalBlock(Grid grid, double width, double height)
            : base(grid, width, height, Shape.Rectangle)
        {
            this.IsSolid = true;
            this.Color = Color.Orange;
        }
    }
}
