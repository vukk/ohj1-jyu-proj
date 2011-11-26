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
    /// Luokka oranssille tippuvalle numeroblockille
    /// </summary>
    public class OrangeNumberBlock : DroppingBlock
    {
        /// <summary>
        /// Blockin arvo
        /// </summary>
        public int Value = 0;

        /// <summary>
        /// Luo uuden numeroblockin
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        /// <param name="num">Numeroarvo</param>
        public OrangeNumberBlock(Grid grid, double width, double height, int num)
            : base(grid, width, height)
        {
            this.Image = Game.LoadImage("o" + num);
            this.Value = num;
            this.IsEdible = true;
        }
    }
}
