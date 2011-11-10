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
    public class MathLevel
    {
        protected Grid Grid;
        protected double DefaultSize;

        protected string LevelAsset;
        protected string LevelEquation;
        protected int LevelNumReq;
        protected Predicate<List<int>> LevelSolver;

        public MathLevel(Grid grid, double defaultSize)
        {
            Grid = grid;
            this.DefaultSize = defaultSize;
        }

        public virtual Label UpdateEquationLabel(List<int> collected)
        {
            Label label;

            string[] format = Enumerable.Repeat(" ", LevelNumReq).ToArray();

            for (int i = 0; i < collected.Count; i++)
                format[i] = collected[i].ToString();

            if (collected.Count > LevelNumReq)
                label = new Label("You collected too many numbers");
            else
                label = new Label(String.Format(LevelEquation, format));

            label.TextColor = Color.LightGray;

            return label;
        }

        public virtual bool CheckCond(List<int> collected)
        {
            return (collected.Count >= LevelNumReq && LevelSolver(collected));
        }

        public void Add(Block block)
        {
            Game.Instance.Add(block);
        }

        public void Load()
        {
            this.LoadLevel(LevelAsset);
        }

        public void LoadLevel(string levelFile)
        {
            TileMap tmap = TileMap.FromLevelAsset(levelFile);
            tmap.SetTileMethod('P', CreatePlayerBlock);
            tmap.SetTileMethod('E', CreateEvalBlock);
            tmap.SetTileMethod('X', CreateRedBlock);
            tmap.SetTileMethod('K', CreateOrangeBlock);
            tmap.SetTileMethod('.', CreateNormalBlock);
            tmap.SetTileMethod('#', CreateHardBlock);

            for (int i = 0; i <= 9; i++)
                tmap.SetTileMethod(i.ToString().ToCharArray()[0], CreateNumberBlock, i);

            tmap.Execute(Grid, Grid.Size, Grid.Size);
        }

        void CreatePlayerBlock(Vector position, double width, double height, object args)
        {
            PlayerBlock player = new PlayerBlock(Grid, DefaultSize, DefaultSize, Shape.Rectangle);
            player.Position = position;
            (Game.Instance as Mathplex.Peli).Player = player;
            this.Add(player);
        }

        void CreateEvalBlock(Vector position, double width, double height, object args)
        {
            EvalBlock o = new EvalBlock(Grid, DefaultSize, DefaultSize, Shape.Rectangle);
            o.Position = position;
            this.Add(o);
        }

        void CreateRedBlock(Vector position, double width, double height, object args)
        {
            ExplodingBlock o = new ExplodingBlock(Grid, DefaultSize, DefaultSize, Shape.Triangle);
            o.Position = position;
            o.IsHard = true;
            o.Color = Color.Red;
            this.Add(o);
        }

        void CreateOrangeBlock(Vector position, double width, double height, object args)
        {
            DroppingBlock o = new DroppingBlock(Grid, DefaultSize, DefaultSize, Shape.Triangle);
            o.Position = position;
            o.IsHard = true;
            o.Color = Color.Orange;
            this.Add(o);
        }

        void CreateNormalBlock(Vector position, double width, double height, object args)
        {
            Block o = new Block(Grid, DefaultSize, DefaultSize, Shape.Rectangle);
            o.Position = position;
            o.IsEdible = true;
            o.Color = RandomGen.NextColor();
            this.Add(o);
        }

        void CreateHardBlock(Vector position, double width, double height, object args)
        {
            Block o = new Block(Grid, DefaultSize, DefaultSize, Shape.Rectangle);
            o.Position = position;
            o.IsSolid = true;
            o.Color = Color.DarkGray;
            this.Add(o);
        }

        void CreateNumberBlock(Vector position, double width, double height, object args)
        {
            NumberBlock o = new NumberBlock(Grid, DefaultSize, DefaultSize, (int)args);
            o.Position = position;
            this.Add(o);
        }
    }
}
