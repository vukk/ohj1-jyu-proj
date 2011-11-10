using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mathplex.Levels
{
    public class Pythagoras : MathLevel
    {
        public Pythagoras(Grid grid, double defaultSize)
            : base(grid, defaultSize)
        {
            this.LevelAsset = "lvl_pythagoras";
            this.LevelEquation = "[{0}]^2 + [{1}]^2 = [{2}]^2";
            this.LevelNumReq = 3;
            this.LevelSolver = x => (Math.Pow(x[0], 2) + Math.Pow(x[1], 2) - Math.Pow(x[2], 2) < Double.Epsilon);
        }
    }
}

