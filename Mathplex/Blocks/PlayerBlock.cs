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
    public class PlayerBlock : Block
    {
        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public PlayerBlock( Grid grid, double width, double height, Shape shape )
            : base( grid, width, height, shape )
        {
            this.CollisionIgnoreGroup = 100;

            this.Collided += PlayerBlockCollision;

            this.IsHard = true;
            this.Color = Color.White;
            this.Tag = "player";
        }

        public void PlayerBlockCollision(IPhysicsObject player, IPhysicsObject target)
        {
            Block block = target as Block;

            if (block != null && block.IsEdible)
            {
                if (block is NumberBlock)
                {
                    NumberBlock numblock = block as NumberBlock;

                    Mathplex.Peli g = Peli.Instance as Mathplex.Peli;
                    g.Collected.Add(numblock.Value);
                    g.CollectedUpdated = true;
                }

                block.Destroy();
            }
        }

        /// <summary>
        /// Siirtää oliota.
        /// </summary>
        /// <param name="movement">Vektori, joka määrittää kuinka paljon siirretään.</param>
        public override void Move(Vector movement)
        {
            if (this.IsMoving == true) return;

            Vector to = this.Position + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y));

            // TODO: should moving DroppingBlocks should be added back...?
            //// player?
            //if (this is PlayerBlock)
            //{
            //    GameObject atLoc = Game.Instance.GetObjectAt(to);
            //    if (atLoc != null && atLoc is DroppingBlock)
            //    {
            //        DroppingBlock atLocx = atLoc as DroppingBlock;
            //        if (atLocx.CanMoveTo(to + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y))))
            //            atLocx.Move(movement);
            //        else
            //            return;
            //    }
            //    else if (atLoc != null && atLoc is Block)
            //    {
            //        Block atLocx = atLoc as Block;
            //        if (atLocx.Eval == true)
            //            ((Mathplex.Peli)Game.Instance).CheckCond();
            //    }
            //}

            GameObject atLoc = Game.Instance.GetObjectAt(to);
            if (atLoc != null && atLoc is EvalBlock)
            {
                (Game.Instance as Mathplex.Peli).CheckCond();
            }

            // voidaanko liikkua...
            if (this.CanMoveTo(to) == false) return;

            // liikutaan
            this.MoveTo(to, this.MaxVelocity * 10);
            this.IsMoving = true;
        }
    }
}
