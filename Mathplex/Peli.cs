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
    public class Peli : PhysicsGame
    {
        private static Grid Grid = new Grid(80);
        public static double hackdiff = 0.2;
        public static double DefaultSize = Grid.Size - hackdiff;

        public List<MathLevel> Levels = new List<MathLevel>();
        public MathLevel CurrentLevel;
        public PlayerBlock Player;
        public Widget Display;

        public List<int> Collected = new List<int>();
        public bool CollectedUpdated = false;

        public void CheckCond()
        {
            Debug.Print("Checking condition");
            if (CurrentLevel.CheckCond(Collected))
            {
                Levels.RemoveAt(0);

                if (Levels.Count > 0)
                {
                    LoadNextLevel();
                }
                else
                {
                    this.Exit();
                }
            }
        }

        public void LoadNextLevel()
        {
            ClearAll();

            Collected = new List<int>();
            CollectedUpdated = false;

            // create display box
            CreateDisplay();

            CurrentLevel = Levels[0];
            CurrentLevel.Load();

            UpdateDisplay();

            SetControls();

            Camera.Follow(Player);
            Camera.Zoom(1);
        }

        public void SetControls()
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

        public void UpdateDisplay()
        {
            Display.Clear();
            Display.Add(CurrentLevel.UpdateEquationLabel(Collected));
            CollectedUpdated = false;
        }

        public override void Begin()
        {
            // display grid at layer -3
            this.GetLayer(-3).Grid = Grid;

            ScreenView sw = Game.Screen;
            this.SetWindowSize((int)sw.Width - 200, (int)sw.Height - 200);
            //this.SetWindowSize(800, 600);

            Level.BackgroundColor = Color.DarkGreen;
            //Level.BackgroundColor = Color.DarkViolet;

            // add levels
            Levels.Add(new Levels.ChildsPlay(Grid, DefaultSize));
            Levels.Add(new Levels.Pythagoras(Grid, DefaultSize));

            // load first level
            LoadNextLevel();
        }

#if DEBUG
        public static Label DebugPositionLabel;

        public void ShowDebugCoord()
        {
            GameObject o = this.GetObjectAt(Mouse.PositionOnWorld);
            if (o != null)
                MessageDisplay.Add("Clicked object at position: " + o.Position);
        }
#endif

        /// <summary>
        /// Liikuta blockia annetun vektorin mukaisesti.
        /// </summary>
        /// <param name="block">Block jota siirretään</param>
        /// <param name="suunta">Vektori jonka mukaan siirretään</param>
        private void MoveBlock(Block block, Vector suunta)
        {
            block.Move(suunta);
        }

        void CreateDisplay()
        {
            Widget laatikko = new Widget(Screen.WidthSafe, Screen.HeightSafe / 10);
            laatikko.Y = Screen.Bottom + laatikko.Height / 2;
            laatikko.X = Screen.Left + laatikko.Width / 2;
            laatikko.Color = Color.DarkBlue;

            Label label = new Label("Mathplex is loading...");
            label.TextColor = Color.LightGray;
            laatikko.Add(label);

            Display = laatikko;

            this.Add(laatikko);
        }

        protected override void Update(Time time)
        {
            if (CollectedUpdated)
            {
                UpdateDisplay();
            }
            base.Update(time);
        }
    }
}
