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
    /// @version 30.11.2011
    /// <summary>
    /// Mathplex peli
    /// </summary>
    public class Peli : PhysicsGame
    {
        /// <summary>
        /// Instanssi
        /// Koska ei aina jaksa kirjoittaa (Game.Instance as Peli).Foobar()
        /// </summary>
        public static new Peli Instance { get; private set; }
        /// <summary>
        /// Pelin grid
        /// </summary>
        private static Grid grid = new Grid(80);
        /// <summary>
        /// Kuinka paljon pienempiä blockit ovat kuin grid
        /// </summary>
        private static double blocksizeGridsizeDiff = 0.8;
        /// <summary>
        /// Blockien oletuskoko
        /// </summary>
        private static double blockDefaultSize = grid.Size - blocksizeGridsizeDiff;

        /// <summary>
        /// Lista kentistä
        /// </summary>
        private List<MathLevel> levels = new List<MathLevel>();
        /// <summary>
        /// Tällä hetkellä pelattava kenttä
        /// </summary>
        public MathLevel CurrentLevel;
        /// <summary>
        /// Tällä hetkellä pelattavan kentän liikkumislogiikkaa
        /// </summary>
        public GridLogic GridLogic;
        /// <summary>
        /// Pelaajablock (ohjainten asettamista varten kätevästi saatavilla)
        /// </summary>
        public PlayerBlock Player;
        /// <summary>
        /// Timeri jolla välillä tarkistetaan onko pelaaja vielä elossa
        /// </summary>
        protected Timer playerAliveCheckTimer;
        /// <summary>
        /// Pelin tilannenäyttö
        /// </summary>
        private Widget display;

        /// <summary>
        /// Kerätyt luvut
        /// </summary>
        public List<int> Collected = new List<int>();
        /// <summary>
        /// Onko kerättyjen lukujen listaa päivitetty
        /// </summary>
        public bool CollectedUpdatedFlag = false;


        /// <summary>
        /// Pelin aloitusmetodi
        /// </summary>
        public override void Begin()
        {
            Instance = this;

            // mute
            SoundEffect.MasterVolume = 0.0;

            // display grid at layer -3
            this.GetLayer(-3).Grid = grid;

            //ScreenView sw = Game.Screen;
            //this.SetWindowSize((int)sw.Width - 200, (int)sw.Height - 200);
            //this.SetWindowSize(800, 600);

            Level.BackgroundColor = Color.DarkGreen;

            // add levels
            levels.Add(new Levels.ChildsPlay(grid, blockDefaultSize));
            levels.Add(new Levels.Pythagoras(grid, blockDefaultSize));
            levels.Add(new Levels.ShowOff(grid, blockDefaultSize));

            // load first level
            LoadNextLevel();
        }


        /// <summary>
        /// Tarkistaa onko kenttä ratkaistu oikein;
        /// vaihtaa seuraavaan kenttään jos on,
        /// viestii tarvittavista lisänumeroista tai
        /// reloadaa nykyisen levelin jos ei ole ratkottu
        /// oikein.
        /// </summary>
        public void CheckCond()
        {
            Debug.Print("Checking condition");

            if (CurrentLevel.CheckCond(Collected))
            {
                // poistetaan tämänhetkinen lvl
                levels.RemoveAt(0);

                // lisää leveleitä vai ei
                if (levels.Count > 0)
                {
                    LoadNextLevel();
                    MessageDisplay.Add("Nicely done!");
                }
                else
                {
                    this.Exit();
                }
            }
            else if (Collected.Count >= CurrentLevel.LevelNumReq)
            {
                LoadNextLevel(); // reload
                MessageDisplay.Add("Wrong answer, try again!");
            }
            else
            {
                MessageDisplay.Add("You need to collect more...");
            }
        }


        /// <summary>
        /// Lataa seuraavan levelin
        /// </summary>
        protected void LoadNextLevel()
        {
            // putsataan aivan kaikki, myös näppäimet
            ClearAll();

            // tyhjätään collected
            Collected = new List<int>();
            CollectedUpdatedFlag = false;

            // luodaan display
            CreateDisplay();

            // ladataan kenttä
            CurrentLevel = levels[0];
            GridLogic = CurrentLevel.Load();

            // päivitetään display
            UpdateDisplay();

            // asetetaan ohjaimet
            SetControls();

            // elossaolo-timer
            Timer timer = new Timer();
            timer.Interval = 4;
            timer.Timeout += PelaajaElossa;
            playerAliveCheckTimer = timer;
            timer.Start();

            // kamera
            Camera.StayInLevel = true;
            Camera.Zoom(1);
            Camera.Follow(Player);
        }


        /// <summary>
        /// Asettaa ohjaimet
        /// </summary>
        protected void SetControls()
        {
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Exit, "Poistu");
            Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä näppäinohjeet");

            // Moving player
            Keyboard.Listen(Key.Up, ButtonState.Down, MoveBlock, "Liikuta pelaajaa ylös", Player, Vector.UnitY);
            Keyboard.Listen(Key.Down, ButtonState.Down, MoveBlock, "Liikuta pelaajaa ylös alas", Player, -Vector.UnitY);
            Keyboard.Listen(Key.Left, ButtonState.Down, MoveBlock, "Liikuta pelaajaa ylös vasemmalle", Player, -Vector.UnitX);
            Keyboard.Listen(Key.Right, ButtonState.Down, MoveBlock, "Liikuta pelaajaa ylös oikealle", Player, Vector.UnitX);

#if DEBUG
            // Mouse
            Mouse.IsCursorVisible = true;
            Mouse.Listen(MouseButton.Left, ButtonState.Pressed, ShowDebugCoord, "Näytä alla olevan objektin koordinaatit");
#endif
        }


#if DEBUG
        protected void ShowDebugCoord()
        {
            GameObject o = this.GetObjectAt(Mouse.PositionOnWorld);
            if (o != null && o is Block)
                MessageDisplay.Add("Clicked object at position: " + o.Position + " location: " + (o as Block).GridLocation);
        }
#endif


        /// <summary>
        /// Tarkistaa onko pelaaja vielä elossa, jos ei niin
        /// restartataan level.
        /// </summary>
        public void PelaajaElossa()
        {
            if (Player.IsDestroyed)
            {
                LoadNextLevel(); // reload
                MessageDisplay.Add("You died, try again!");
            }
        }


        /// <summary>
        /// Liikuta blockia annetun vektorin mukaisesti, ohjainten asettamista varten
        /// </summary>
        /// <param name="block">Block jota siirretään</param>
        /// <param name="suunta">Vektori jonka mukaan siirretään</param>
        protected void MoveBlock(Block block, Vector suunta)
        {
            block.Move(suunta);
        }


        /// <summary>
        /// Luo tilannenäytön
        /// </summary>
        protected void CreateDisplay()
        {
            Widget laatikko = new Widget(Screen.WidthSafe, Screen.HeightSafe / 10);
            laatikko.Y = Screen.Bottom + laatikko.Height / 2;
            laatikko.X = Screen.Left + laatikko.Width / 2;
            laatikko.Color = Color.DarkBlue;

            Label label = new Label("Mathplex is loading...");
            label.TextColor = Color.LightGray;
            laatikko.Add(label);

            display = laatikko;

            this.Add(laatikko);
        }


        /// <summary>
        /// Päivittää tilannenäytön
        /// </summary>
        public void UpdateDisplay()
        {
            display.Clear();
            display.Add(CurrentLevel.UpdateEquationLabel(Collected));
            CollectedUpdatedFlag = false;
        }


        /// <summary>
        /// Päivitysmetodi pelille
        /// </summary>
        /// <param name="time">Aika</param>
        protected override void Update(Time time)
        {
            if (CollectedUpdatedFlag)
            {
                UpdateDisplay();
            }

            GridLogic.Update(time);

            base.Update(time);
        }
    }
}
