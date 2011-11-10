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
    class ExplodingBlock : DroppingBlock
    {
        public ExplodingBlock(Grid grid, double width, double height, Shape shape)
            : base(grid, width, height, shape)
        {
            this.Collided -= DroppingBlockCollided;
            this.Collided += ExplodingDroppingBlockCollided;

            this.StopOnlyWhenColliding = true;
        }

        public void ExplodingDroppingBlockCollided(IPhysicsObject self, IPhysicsObject target)
        {
            Debug.Print("ExplodingBlock at " + self.Position + " collided with obj at " + target.Position);

            Explosion expl = new Explosion(80);
            //rajahdys.Sound = LoadSoundEffect("implosionSound"); // todo: fix sounds
            expl.Force = 0;
            expl.ShockwaveColor = Color.LightBlue;
            expl.Position = Vector.Average(new List<Vector> { self.Position, target.Position });

            List<GameObject> list = new List<GameObject>();
            if (target is PlayerBlock)
            {
                list = GetImmediateSurroundings(self as Block, target as Block);
            }
            else
            {
                list = GetImmediateSurroundings(self as Block);
            }
            list.ForEach(x => { Block y = x as Block; y.TryExplode(); });

            Game.Instance.Add(expl);
        }
    }
}
