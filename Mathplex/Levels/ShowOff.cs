using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathplex.Levels
{
    /// @author Unto Kuuranne
    /// @version 26.11.2011
    /// <summary>
    /// "Show Off" kenttä
    /// </summary>
    public class ShowOff : MathLevel
    {
        /// <summary>
        /// Luodaan kenttä
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="defaultSize">Blockien oletuskoko</param>
        public ShowOff(Grid grid, double defaultSize)
            : base(grid, defaultSize)
        {
            this.levelAsset = "lvl_showoff";
            this.levelEquation = "[{0}] + [{1}] + [{2}] + [{3}] - [{4}] = [{5}] + [{6}]";
            this.LevelNumReq = 7;
            this.levelSolver = x => x[0] + x[1] + x[2] + x[3] - x[4] - x[5] - x[6] == 0;
        }
    }
}
