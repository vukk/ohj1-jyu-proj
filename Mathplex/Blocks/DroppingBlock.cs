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
    class DroppingBlock : Block
    {
        public new double MaxVelocity = 20; // TODO: fix

        public bool StopOnlyWhenColliding = false;
        protected bool forceMove = false;

        public DroppingBlock( Grid grid, double width, double height, Shape shape )
            : base( grid, width, height, shape )
        {
            this.IsUpdated = true;

            this.CollisionIgnoreGroup = 0;
            this.Collided += DroppingBlockCollided;

            //this.Arrived -= BlockArrived;
            //this.Arrived += DroppingBlockArrived;

            this.Arrived += DroppingBlockArrived;
        }

        protected void DroppingBlockArrived(IGameObject self, Vector location)
        {
            // should we continue moving?
            if (this.ShouldMoveDown(true))
                this.Drop();
        }

        public void DroppingBlockCollided(IPhysicsObject self, IPhysicsObject target)
        {
            if (IsMoving && target is PlayerBlock) // TODO: || target is ExplodingBlock
            {
                List<GameObject> list = GetImmediateSurroundings(self as Block, target as Block);
                list.ForEach(x => { Block y = x as Block; y.TryExplode(); });
            }
        }

        public override bool CanMoveTo(Vector to)
        {
            Predicate<Block> isHardCheck = delegate(Block x) { return x.IsHard; };
            Predicate<Block> isNotPlayerCheck = delegate(Block x) { return !(x is PlayerBlock); };
            Predicate<Block> isNotExplodingCheck = delegate(Block x) { return !(x is ExplodingBlock); };
            Predicate<Block> isNotForceMoveCheck = delegate(Block x) { return !forceMove; };

            return CanMoveTo(to, new List<Predicate<Block>> { isHardCheck, isNotPlayerCheck, isNotForceMoveCheck });
        }

        public bool ShouldMoveDown(bool movingRightNow = false)
        {
            bool MoveDown = true;

            List<GameObject> list = Game.Instance.GetObjectsAt(
                    this.Position - (Grid.CellSize.Y * Vector.UnitY), (Grid.CellSize.Y / 2) - 1
                );

            foreach (var obj in list)
            {
                //Debug.Print("found obj at pos: " + obj.Position);
                if (!obj.IsDestroyed)
                {
                    if (obj == this) continue; // skip self if in list

                    // skip player if moving already (so we can collide)
                    if (movingRightNow && obj is PlayerBlock) continue;

                    //Debug.Print("and obj is not destroyed");
                    MoveDown = false;
                    break;
                }
            }

            //Debug.Print("Obj count downwards " + list.Count);

            return MoveDown;
        }

        public void Drop()
        {
            //Debug.Print("No objects downwards, moving");
            if (this.StopOnlyWhenColliding) this.forceMove = true;
            this.Move(-Vector.UnitY);
        }

        public override void Update(Time time)
        {
            if (!IsMoving)
            {
                if (this.forceMove)
                    Drop();
                else if (ShouldMoveDown())
                    Drop();
            }

            base.Update(time);
        }
    }
}
