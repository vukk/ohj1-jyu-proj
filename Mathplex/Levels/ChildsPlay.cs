using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathplex.Levels
{
    /// @author Unto Kuuranne
    /// @version 26.11.2011
    /// <summary>
    /// "Child's play" kenttä
    /// </summary>
    public class ChildsPlay : MathLevel
    {
        /// <summary>
        /// Luodaan kenttä
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="defaultSize">Blockien oletuskoko</param>
        public ChildsPlay(Grid grid, double defaultSize)
            : base(grid, defaultSize)
        {
            this.LevelAsset = "lvl_childsplay";
            this.LevelEquation = "[{0}] + [{1}] = [{2}]";
            this.LevelNumReq = 3;
            this.LevelSolver = x => x[0] + x[1] - x[2] == 0;
        }
    }
}
