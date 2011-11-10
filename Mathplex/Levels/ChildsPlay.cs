using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathplex.Levels
{
    public class ChildsPlay : MathLevel
    {
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
