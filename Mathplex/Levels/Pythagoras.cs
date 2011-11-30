using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathplex.Levels
{
    /// @author Unto Kuuranne
    /// @version 26.11.2011
    /// <summary>
    /// "Pythagoras" kenttä
    /// </summary>
    public class Pythagoras : MathLevel
    {
        /// <summary>
        /// Luodaan kenttä
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="defaultSize">Blockien oletuskoko</param>
        public Pythagoras(Grid grid, double defaultSize)
            : base(grid, defaultSize)
        {
            this.levelAsset = "lvl_pythagoras2";
            this.levelEquation = "[{0}]^2 + [{1}]^2 = [{2}]^2";
            this.LevelNumReq = 3;
            this.levelSolver = x => (Math.Pow(x[0], 2) + Math.Pow(x[1], 2) - Math.Pow(x[2], 2) < Double.Epsilon);
        }
    }
}

