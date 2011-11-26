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
    /// Luokka räjähtävälle blockille
    /// </summary>
    public class ExplodingBlock : DroppingBlock
    {
        /// <summary>
        /// Luodaan uusi räjähtävä block
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        public ExplodingBlock(Grid grid, double width, double height)
            : base(grid, width, height)
        {
            this.MaxVelocity = Grid.Size * 0.2;

            this.Collided -= DroppingBlockCollided;
            this.Collided += ExplodingDroppingBlockCollided;

            // stopataan vasta kun törmätään
            this.StopOnlyWhenColliding = true;
        }


        /// <summary>
        /// Räjähtävän blockin törmäys-event
        /// </summary>
        /// <param name="self">ExplodingBlock joka törmäsi</param>
        /// <param name="target">Block johon törmättiin</param>
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

            Peli.Instance.Add(expl);
        }


        /// <summary>
        /// Koittaa räjäyttää tämä block
        /// </summary>
        public override void TryExplode()
        {
            if (this.IsDestroyed) return;

            base.TryExplode();

            List<GameObject> list = GetImmediateSurroundings(this).Where(x => x != null && x.IsDestroyed == false).ToList();
            list.ForEach(x => { Block y = x as Block; y.TryExplode(); });
        }
    }
}
