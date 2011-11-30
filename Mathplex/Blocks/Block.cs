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
    /// Mathplexin perusblockin luokka
    /// </summary>
    public class Block : Jypeli.PhysicsObject
    {
        /// <summary>
        /// Pelin grid
        /// </summary>
        public Grid Grid;

        /// <summary>
        /// Sijainti gridissä
        /// </summary>
        public Vector GridLocation = new Vector();

        // Solid block can not be moved or destroyed
        private bool _isSolid = false;
        /// <summary>
        /// Onko block kiinteä
        /// Kiinteää blockia ei voi syödä, liikuttaa tai räjäyttää
        /// </summary>
        public bool IsSolid {
            get { return this._isSolid; }
            set
            {
                this._isSolid = value;
                if (value == true) this.IsHard = true;
                if (value == true) this.IsEdible = false;
            }
        }

        private bool _isHard = false;
        /// <summary>
        /// Onko block kova (tuhoaa pelaajan tippuessa päälle)
        /// </summary>
        public bool IsHard
        {
            get { return this._isHard; }
            set
            {
                this._isHard = value;
            }
        }

        // Edible block is edible by the player block
        private bool _isEdible = false;
        /// <summary>
        /// Onko block pelaajan syötävä vai ei
        /// </summary>
        public bool IsEdible
        {
            get { return this._isEdible; }
            set
            {
                this._isEdible = value;
            }
        }

        public double MaxVelocity = 100;

        /// <summary>
        /// Onko block liikkeessä
        /// </summary>
        public bool IsMoving = false;

        /// <summary>
        /// Liikkumisen aloituspaikka
        /// </summary>
        public Vector MovingFrom = new Vector();
        /// <summary>
        /// Paikka johon ollaan kenties liikkumassa (jonka kertoo IsMoving)
        /// </summary>
        public Vector MovingTo = new Vector();

        /// <summary>
        /// Mitkä tekijät määräävät blockin liikkumisen johonkin ruutuun
        /// </summary>
        public List<Predicate<Block>> limitFactors = new List<Predicate<Block>>();


        /// <summary>
        /// Luo uuden blockin
        /// </summary>
        /// <param name="grid">Pelin grid</param>
        /// <param name="width">Leveys</param>
        /// <param name="height">Korkeus</param>
        /// <param name="shape">Muoto</param>
        public Block( Grid grid, double width, double height, Shape shape )
            : base( width, height, shape )
        {
            this.Grid = grid;
            this.CanRotate = false;
            this.IgnoresExplosions = true;
            this.IgnoresCollisionResponse = true;
            this.IgnoresGravity = true;
            this.CollisionIgnoreGroup = 1;

            this.MaxVelocity = Grid.Size;

            this.Arrived += BlockArrived;
        }


        /// <summary>
        /// Blockin saapumis-event
        /// </summary>
        /// <param name="self">Block joka saapui</param>
        /// <param name="location">Saapumispaikka</param>
        protected void BlockArrived(IGameObject self, Vector location)
        {
            Debug.Print("Moving Block Arrived at: " + location);

            Block o = self as Block;

            o.Stop();
            o.Position = location; // not trusting the code ololololo
            o.GridLocation = MovingTo;
            o.IsMoving = false;

            Peli.Instance.GridLogic.Arrived(this, MovingFrom, MovingTo);
        }


        /// <summary>
        /// Voiko block liikkua paikkaan <c>to</c> vai ei.
        /// </summary>
        /// <param name="to">Paikka johon liikkua</param>
        /// <returns>Voidaanko paikkaan liikkua</returns>
        public virtual bool CanMoveTo(Vector to)
        {
            return Peli.Instance.GridLogic.CanMoveTo(to, limitFactors);
        }


        /// <summary>
        /// Siirtää blockia
        /// </summary>
        /// <param name="movement">Vektori, joka määrittää kuinka paljon siirretään.</param>
        public override void Move(Vector movement)
        {
            if (this.IsMoving == true) return;

            Vector to = this.Position + (new Vector(movement.X * Grid.CellSize.X, movement.Y * Grid.CellSize.Y));
            Vector toLoc = new Vector(this.GridLocation.X + movement.X, this.GridLocation.Y - movement.Y);

            // voidaanko liikkua...
            if (this.CanMoveTo(toLoc) == false) return;
            
            // liikutaan
            this.MovingFrom = this.GridLocation;
            // GridLocation ja GameObj.Position Y-liikkeet ovat vastakkaisia...
            this.MovingTo = toLoc;
            this.MoveTo(to, this.MaxVelocity * 10);
            this.IsMoving = true;
        }


        /// <summary>
        /// Palauttaa blockin välittömät naapurit
        /// </summary>
        /// <param name="obj">Block jonka naapurit halutaan</param>
        /// <returns>Blockin naapurit listana</returns>
        public static List<GameObject> GetImmediateSurroundings(Block obj)
        {
            return Peli.Instance.GridLogic.GetImmediateSurroundings(obj.GridLocation);
        }


        /// <summary>
        /// Palauttaa kahden eri blockin välittömät naapurit yhdessä listassa
        /// </summary>
        /// <param name="one">Ensimmäinen block jonka naapurit halutaan</param>
        /// <param name="two">Toinen block jonka naapurit halutaan</param>
        /// <returns>Blockien naapurit listana</returns>
        public static List<GameObject> GetImmediateSurroundings(Block one, Block two)
        {
            List<GameObject> list = GetImmediateSurroundings(one);
            list.Concat<GameObject>(GetImmediateSurroundings(two));

            return list;
        }


        /// <summary>
        /// Koittaa räjäyttää tämä block
        /// Ei räjäytä jo tuhottuja tai kiinteitä blockeja
        /// </summary>
        public virtual void TryExplode()
        {
            if (this.IsDestroyed) return;
            if (this.IsSolid) return;

            Explosion expl = new Explosion(Grid.Size);
            //expl.Sound = LoadSoundEffect("implosionSound"); // TODO: fix sounds
            expl.Force = 0;
            expl.ShockwaveColor = Color.LightPink;
            expl.Position = this.Position;
            Peli.Instance.Add(expl);
            this.Destroy();
        }


        /// <summary>
        /// Tuhoa block jos se ei ole kova
        /// </summary>
        public void DestroyIfNotHard()
        {
            if (!this.IsHard) this.Destroy();
        }
    }
}
