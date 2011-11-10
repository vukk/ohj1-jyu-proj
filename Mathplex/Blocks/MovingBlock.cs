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
    public class MovingBlock : Block
    {
        public new double Acceleration = 80;
        public new double MaxVelocity = 80;

        public bool IsMoving = false;

        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public MovingBlock( Grid grid, double width, double height, Shape shape )
            : base( grid, width, height, shape )
        {
            this.Acceleration = Grid.CellSize.Y;
            this.MaxVelocity = Grid.CellSize.Y;

            this.IsUpdated = true;
            this.CollisionIgnoreGroup = 2;
            this.Arrived += MovingBlockArrived;
        }

        protected void MovingBlockArrived(IGameObject obj, Vector location)
        {
            Debug.Print("Moving Block Arrived at: " + location);
            MovingBlock x = obj as MovingBlock;
            StopMoving(x, location);
        }

        public bool CanMoveTo(Vector to)
        {
            Level lvl = Game.Instance.Level;

            // rajataan alue
            if (to.X > lvl.Right || to.X < lvl.Left || to.Y > lvl.Top || to.Y < lvl.Bottom)
            {
                Debug.Print("MovingBlock was limited by level boundaries");
                return false;
            }

            // tsekataan ettei ole kovia palikoita liikkumiskohdassa
            GameObject atLoc = Game.Instance.GetObjectAt(to);
            if (atLoc != null && (string)atLoc.Tag == "hard" && !(this is ExplodingBlock))
            {
                Debug.Print("MovingBlock was prevented from moving by a hard block");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Siirtää oliota.
        /// </summary>
        /// <param name="movement">Vektori, joka määrittää kuinka paljon siirretään.</param>
        public override void Move(Vector movement)
        {
            if (this.IsMoving == true) return;

            Vector pos = this.Position;
            //Vector to = pos + (movement * Grid.CellSize.X); 
            Vector to = pos + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y));

            // player?
            if (this is PlayerBlock)
            {
                GameObject atLoc = Game.Instance.GetObjectAt(to);
                if (atLoc != null && atLoc is DroppingBlock)
                {
                    DroppingBlock atLocx = atLoc as DroppingBlock;
                    if (atLocx.CanMoveTo(to + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y))))
                        atLocx.Move(movement);
                    else
                        return;
                }
                else if (atLoc != null && atLoc is EvalBlock)
                {
                    (Game.Instance as Mathplex.Peli).CheckCond();
                }
            }

            // voidaanko liikkua...
            if (this.CanMoveTo(to) == false) return;

            // liikutaan
            this.MoveTo(to, this.MaxVelocity * 10);
            this.IsMoving = true;
        }

        protected static void StopMoving(MovingBlock obj, Vector mov)
        {
            obj.Stop();
            obj.Position = mov; // not trusting the code ololololo
            obj.IsMoving = false;
        }
    }
}
