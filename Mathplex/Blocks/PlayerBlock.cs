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
    /// Luokka pelaajablockille
    /// </summary>
    public class PlayerBlock : Block
    {
        /// <summary>
        /// Luo uuden pelaajablockin
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        /// <param name="shape">Muoto<param>
        public PlayerBlock( Grid grid, double width, double height, Shape shape )
            : base( grid, width, height, shape )
        {
            this.MaxVelocity = Grid.Size * 1.2;

            this.CollisionIgnoreGroup = 100;

            this.Collided += PlayerBlockCollision;

            this.IsHard = true;
            this.Color = Color.White;

            Predicate<Block> isHardAndNotEdible = delegate(Block x) { return x.IsHard && !x.IsEdible; };
            limitFactors.Add(isHardAndNotEdible);
        }


        /// <summary>
        /// Pelaajablockin törmäys-event
        /// </summary>
        /// <param name="player">PlayerBlock joka törmäsi</param>
        /// <param name="target">Block johon törmättiin</param>
        public void PlayerBlockCollision(IPhysicsObject player, IPhysicsObject target)
        {
            Block block = target as Block;
            PlayerBlock p = player as PlayerBlock;

            // can we eat it
            if (block != null && block.IsEdible)
            {
                if (block is NumberBlock)
                {
                    NumberBlock numblock = block as NumberBlock;

                    // add to collected numbers
                    Peli.Instance.Collected.Add(numblock.Value);
                    Peli.Instance.CollectedUpdatedFlag = true;
                }
                else if (block is OrangeNumberBlock)
                {
                    OrangeNumberBlock orangenumblock = block as OrangeNumberBlock;

                    // add to collected numbers
                    Peli.Instance.Collected.Add(orangenumblock.Value);
                    Peli.Instance.CollectedUpdatedFlag = true;
                }

                Peli.Instance.GridLogic.PlayerBlockEats(p, p.MovingTo);
                block.Destroy();
            }
        }


        /// <summary>
        /// Siirtää pelaajaa
        /// </summary>
        /// <param name="movement">Vektori, joka määrittää kuinka paljon siirretään.</param>
        public override void Move(Vector movement)
        {
            base.Move(movement);

            Vector to = this.Position + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y));

            // jos liikutaan Eval-blockiin, tsekataan levelin cond
            GameObject atLoc = Peli.Instance.GetObjectAt(to);
            if (atLoc != null && atLoc is EvalBlock)
            {
                Peli.Instance.CheckCond();
            }
        }
    }
}
