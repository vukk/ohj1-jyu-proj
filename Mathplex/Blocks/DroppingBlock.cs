using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

namespace Mathplex
{
    /// @author Un​to Ku​ur​an​ne
    /// @version 26.11.2011
    /// <summary>
    /// Luokka tippuvalle blockille
    /// </summary>
    public class DroppingBlock : Block
    {
        /// <summary>
        /// Stoppaako block vain törmätessään
        /// </summary>
        public bool StopOnlyWhenColliding = false;
        /// <summary>
        /// Pakotetaanko block liikkumaan
        /// </summary>
        public bool forceMove = false;


        /// <summary>
        /// Luodaan uusi tippuva block
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        public DroppingBlock( Grid grid, double width, double height)
            : base( grid, width, height, Shape.Rectangle)
        {
            this.MaxVelocity = Grid.Size * 0.2;

            this.IsHard = true;

            this.CollisionIgnoreGroup = 0;
            this.Collided += DroppingBlockCollided;

            this.Arrived += DroppingBlockArrived;

            Predicate<Block> isHardCheck = delegate(Block x) { return x.IsHard; };
            Predicate<Block> isNotPlayerCheck = delegate(Block x) { return !(x is PlayerBlock); };
            Predicate<Block> isNotExplodingCheck = delegate(Block x) { return !(x is ExplodingBlock); };
            Predicate<Block> isNotForceMoveCheck = delegate(Block x) { return !forceMove; };

            limitFactors.Add(isHardCheck);
            limitFactors.Add(isNotPlayerCheck);
            limitFactors.Add(isNotExplodingCheck);
            limitFactors.Add(isNotForceMoveCheck);
        }


        /// <summary>
        /// Tippuvan blockin saapumis-event
        /// </summary>
        /// <param name="self">DroppingBlock joka saapui</param>
        /// <param name="location">Saapumispaikka</param>
        protected void DroppingBlockArrived(IGameObject self, Vector location)
        {
            int row = (int)this.GridLocation.Y;
            int col = (int)this.GridLocation.X;

            GridLogic lvl = Peli.Instance.GridLogic;

            bool moveDown = true;

            if (lvl.IsInsideLevel(row + 1, col))
            {
                GameObject obj = lvl.Get(row + 1, col);

                if (obj != null && !obj.IsDestroyed)
                {
                    // skip PlayerBlock and ExplodingBlock if we are moving already
                    // ... so we can collide with them.
                    if (!(obj is PlayerBlock) && !(obj is ExplodingBlock))
                        moveDown = false;
                }
            }

            if (moveDown || (self as DroppingBlock).forceMove)
                this.Drop();
        }


        /// <summary>
        /// Tippuvan blockin törmäys-event
        /// </summary>
        /// <param name="self">DroppingBlock joka törmäsi</param>
        /// <param name="target">Block johon törmättiin</param>
        public static void DroppingBlockCollided(IPhysicsObject self, IPhysicsObject target)
        {
            if ((self as DroppingBlock).IsMoving)
            {
                if (target is PlayerBlock)// || target is ExplodingBlock)
                {
                    List<GameObject> list = GetImmediateSurroundings(self as Block, target as Block);
                    list.ForEach(x => { Block y = x as Block; y.TryExplode(); });
                }
                else if (target is ExplodingBlock)
                {
                    (target as ExplodingBlock).ExplodingDroppingBlockCollided(target, self);
                }
            }
        }


        /// <summary>
        /// Tiputetaan blockia
        /// </summary>
        public void Drop()
        {
            //Debug.Print("No objects downwards, moving");
            if (this.StopOnlyWhenColliding) this.forceMove = true;
            this.Move(-Vector.UnitY);
        }
    }
}
