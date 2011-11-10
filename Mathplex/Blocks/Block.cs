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
    public class Block : Jypeli.PhysicsObject
    {
        public Grid Grid;

        // Solid block can not be moved or destroyed
        private bool _IsSolid = false;
        public bool IsSolid {
            get { return this._IsSolid; }
            set
            {
                this._IsSolid = value;
                if (value == true) this.IsHard = true;
            }
        }

        // Hard block can not be eaten
        private bool _IsHard = false;
        public bool IsHard
        {
            get { return this._IsHard; }
            set
            {
                this._IsHard = value;
                if (value == true) this.IsEdible = false;
            }
        }

        private bool _IsEdible = false;
        public bool IsEdible
        {
            get { return this._IsEdible; }
            set
            {
                this._IsEdible = value;
                if (value == true) this.IsHard = false;
            }
        }

        public new double Acceleration = 100;
        public new double MaxVelocity = 100;

        public bool IsMoving = false;

        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public Block( Grid grid, double width, double height, Shape shape )
            : base( width, height, shape )
        {
            this.Grid = grid;
            this.CanRotate = false;
            this.IgnoresExplosions = true;
            this.IgnoresCollisionResponse = true;
            this.IgnoresGravity = true;
            this.CollisionIgnoreGroup = 1;

            this.Acceleration = Grid.Size;
            this.MaxVelocity = Grid.Size;

            this.Arrived += BlockArrived;
        }

        protected void BlockArrived(IGameObject obj, Vector location)
        {
            Debug.Print("Moving Block Arrived at: " + location);

            Block o = obj as Block;

            o.Stop();
            o.Position = location; // not trusting the code ololololo
            o.IsMoving = false;
        }

        public virtual bool CanMoveTo(Vector to)
        {
            Predicate<Block> isHardCheck = delegate(Block x) { return x.IsHard; };

            return CanMoveTo(to, new List<Predicate<Block>> { isHardCheck });
        }

        public bool CanMoveTo(Vector to, List<Predicate<Block>> limitFactors)
        {
            Level lvl = Peli.Instance.Level;

            // limit to level boundaries
            if (to.X > lvl.Right || to.X < lvl.Left || to.Y > lvl.Top || to.Y < lvl.Bottom)
            {
                Debug.Print("Moving Block was limited by level boundaries");
                return false;
            }

            // check that none of the limiting factors apply to the object at location to
            // ... if there is an object there
            // ... so: if all predicates yield true, then movement is limited
            Block atLoc = Peli.Instance.GetObjectAt(to) as Block;
            if (atLoc != null && limitFactors.All(predicate => predicate(atLoc) == true)) return false;

            return true;
        }

        /// <summary>
        /// Siirtää oliota.
        /// </summary>
        /// <param name="movement">Vektori, joka määrittää kuinka paljon siirretään.</param>
        public override void Move(Vector movement)
        {
            if (this.IsMoving == true) return;

            Vector to = this.Position + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y));

            // voidaanko liikkua...
            if (this.CanMoveTo(to) == false) return;
            
            // liikutaan
            this.MoveTo(to, this.MaxVelocity * 10);
            this.IsMoving = true;
        }

        public static List<GameObject> GetImmediateSurroundings(Block obj)
        {
            // TODO: remove?
            Game.Instance.GetLayer(0).Update(Game.Time); // synchronize main layer object list

            Vector position = obj.Grid.SnapToLines(obj.Position);
            Debug.Print("Grid.Size: " + obj.Grid.Size + " Grid.CellSize.X: " + obj.Grid.CellSize.X + " Grid.CellSize.Y: " + obj.Grid.CellSize.Y);
            Debug.Print("GetImmediateSurroundings: pos " + position + " orig.pos " + obj.Position);
            return GetSurroundings(position, obj.Grid.Size + 1);
        }

        public static List<GameObject> GetImmediateSurroundings(Block one, Block two)
        {
            Game.Instance.GetLayer(0).Update(Game.Time); // synchronize main layer object list

            List<GameObject> list = GetImmediateSurroundings(one);
            list.Concat<GameObject>(GetImmediateSurroundings(two));

            return list;
        }

        /// <summary>
        /// Gets surrounding objects of position by radius, returns those objects which have
        /// a center point in the radius. Radius is calculated from the position that is snapped to grid.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static List<GameObject> GetSurroundings(Vector position, double radius)
        {
            // remember to synchronize layer objects before calling this method
            // ... if needed
            // Game.Instance.GetLayer(0).Update(Game.Time); // synchronize main layer object list

            Predicate<GameObject> isCenterPointInsideRadius = delegate(GameObject obj)
            {
                bool res = (obj.Position.X < position.X + radius && obj.Position.X > position.X - radius &&
                        obj.Position.Y < position.Y + radius && obj.Position.Y > position.Y - radius);

                if (res)
                {
                    Debug.Print("Is inside");
                    Debug.Print("X: " + obj.Position.X + " < " + (position.X + radius) + ", " + obj.Position.X + " > " + (position.X - radius));
                    Debug.Print("Y: " + obj.Position.Y + " < " + (position.Y + radius) + ", " + obj.Position.Y + " > " + (position.Y - radius));
                }

                return res;
            };

            return Game.Instance.GetObjects(isCenterPointInsideRadius);
        }

        public void TryExplode()
        {
            if (this.IsDestroyed) return;
            if (this.IsSolid) return;

            Explosion expl = new Explosion(Grid.Size);
            //expl.Sound = LoadSoundEffect("implosionSound"); // TODO: fix sounds
            expl.Force = 0;
            expl.ShockwaveColor = Color.LightPink;
            expl.Position = this.Position;
            Game.Instance.Add(expl);
            this.Destroy();
        }

        public void DestroyInExplosion(IPhysicsObject obj)
        {
            if (obj.IsDestroyed) return;
            if ((string)obj.Tag == "hard") return;

            Explosion rajahdys = new Explosion(Grid.CellSize.Y);
            //rajahdys.Sound = LoadSoundEffect("implosionSound"); // todo: fix sounds
            rajahdys.Force = 0;
            rajahdys.ShockwaveColor = Color.LightPink;
            rajahdys.Position = obj.Position;
            Game.Instance.Add(rajahdys);
            obj.Destroy();
            //obj.Color = Color.Black;
        }

        public void DestroyIfNotHard()
        {
            if (!this.IsHard) this.Destroy();
        }
    }
}
