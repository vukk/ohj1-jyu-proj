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
    /// Luokka pelin kentälle
    /// </summary>
    public class MathLevel
    {
        /// <summary>
        /// Kentän grid
        /// </summary>
        protected Grid grid;
        /// <summary>
        /// Olioiden oletuskoko
        /// </summary>
        protected double defaultSize;

        /// <summary>
        /// Tekstitiedosto jonka perusteella kenttä luodaan
        /// </summary>
        protected string levelAsset;
        /// <summary>
        /// Kentän ratkaistava yhtälö
        /// </summary>
        protected string levelEquation;
        /// <summary>
        /// Kuinka monta numeroa kentässä tulee kerätä
        /// </summary>
        public int LevelNumReq;
        /// <summary>
        /// Kentän yhtälön solveri
        /// </summary>
        protected Predicate<List<int>> levelSolver;


        /// <summary>
        /// Olio joka edustaa kenttää
        /// </summary>
        /// <param name="grid">Grid</param>
        /// <param name="defaultSize">Olioiden oletuskoko</param>
        public MathLevel(Grid grid, double defaultSize)
        {
            this.grid = grid;
            this.defaultSize = defaultSize;
        }


        /// <summary>
        /// Tämä metodi palauttaa Label olion jossa on ajantasainen
        /// informaatio kentän ratkaisun edistymisestä
        /// </summary>
        /// <param name="collected">Lista kerätyistä numeroista</param>
        /// <returns>Päivitetty Label</returns>
        public Label UpdateEquationLabel(List<int> collected)
        {
            Label label;

            string[] format = Enumerable.Repeat(" ", LevelNumReq).ToArray();

            int max = Math.Min(LevelNumReq, collected.Count);

            for (int i = 0; i < max; i++)
                format[i] = collected[i].ToString();

            if (collected.Count > LevelNumReq)
                label = new Label("You collected too many numbers");
            else
                label = new Label(String.Format(levelEquation, format));

            label.TextColor = Color.LightGray;

            return label;
        }


        /// <summary>
        /// Tarkistaa onko kenttä ratkottu oikein
        /// </summary>
        /// <param name="collected">Lista kerätyistä numeroista</param>
        /// <returns>Onko kenttä ratkottu oikein vai ei</returns>
        public virtual bool CheckCond(List<int> collected)
        {
            return (collected.Count >= LevelNumReq && levelSolver(collected));
        }


        /// <summary>
        /// Lisää Block-objektin peliin. Kätevämpi näin.
        /// </summary>
        /// <param name="block">Block joka lisätään peliin</param>
        public void Add(Block block)
        {
            Peli.Instance.Add(block);
        }


        /// <summary>
        /// Lataa kentän
        /// </summary>
        public GridLogic Load()
        {
            return this.LoadLevel(levelAsset);
        }


        /// <summary>
        /// Lataa kentän tekstitiedostosta ja luo objektit
        /// </summary>
        /// <param name="levelFile">Tekstitiedosto josta kenttä ladataan</param>
        public GridLogic LoadLevel(string levelFile)
        {
            TileMap tmap = TileMap.FromLevelAsset(levelFile);

            // Asetetaan merkeille merkityksiä objekteina
            tmap.SetTileMethod('P', CreatePlayer);
            tmap.SetTileMethod('E', CreateEval);
            tmap.SetTileMethod('R', CreateRed);
            tmap.SetTileMethod('O', CreateOrange);
            tmap.SetTileMethod('.', CreateNormal);
            tmap.SetTileMethod('#', CreateSolid);

            for (int i = 0; i <= 9; i++)
                tmap.SetTileMethod(i.ToString().ToCharArray()[0], CreateNumber, i);

            tmap.SetTileMethod('a', CreateOrangeNumber, 1);
            tmap.SetTileMethod('b', CreateOrangeNumber, 2);
            tmap.SetTileMethod('c', CreateOrangeNumber, 3);
            tmap.SetTileMethod('d', CreateOrangeNumber, 4);
            tmap.SetTileMethod('e', CreateOrangeNumber, 5);
            tmap.SetTileMethod('f', CreateOrangeNumber, 6);
            tmap.SetTileMethod('g', CreateOrangeNumber, 7);
            tmap.SetTileMethod('h', CreateOrangeNumber, 8);
            tmap.SetTileMethod('i', CreateOrangeNumber, 9);

            return tmap.Execute(grid);
        }


        /// <summary>
        /// Luo pelaajablockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreatePlayer(Vector position, double width, double height, object args)
        {
            PlayerBlock player = new PlayerBlock(grid, defaultSize, defaultSize, Shape.Rectangle);
            player.Position = position;
            Peli.Instance.Player = player;
            this.Add(player);
            return player;
        }


        /// <summary>
        /// Luo Evaluate/Exit-blockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateEval(Vector position, double width, double height, object args)
        {
            EvalBlock o = new EvalBlock(grid, defaultSize, defaultSize);
            o.Position = position;
            o.Image = Game.LoadImage("e");
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo punaisen räjähtävän blockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateRed(Vector position, double width, double height, object args)
        {
            ExplodingBlock o = new ExplodingBlock(grid, defaultSize, defaultSize);
            o.Position = position;
            o.IsHard = true;
            o.Image = Game.LoadImage("red");
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo oranssin tippuvan blockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateOrange(Vector position, double width, double height, object args)
        {
            DroppingBlock o = new DroppingBlock(grid, defaultSize, defaultSize);
            o.Position = position;
            o.IsHard = true;
            o.Image = Game.LoadImage("orange");
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo tavallisen blockin joka ei tee mitään
        /// Blockille arvotaan väri
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateNormal(Vector position, double width, double height, object args)
        {
            Block o = new Block(grid, defaultSize, defaultSize, Shape.Rectangle);
            o.Position = position;
            o.IsEdible = true;
            o.Color = RandomGen.NextColor();
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo kovan blockin jota käytetään myös kentän seininä
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateSolid(Vector position, double width, double height, object args)
        {
            Block o = new Block(grid, defaultSize, defaultSize, Shape.Rectangle);
            o.Position = position;
            o.IsSolid = true;
            o.Image = Game.LoadImage("solid");
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo tavallisen numeroblockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Numero joka blockilla on (int 0-9)</param>
        public GameObject CreateNumber(Vector position, double width, double height, object args)
        {
            NumberBlock o = new NumberBlock(grid, defaultSize, defaultSize, (int)args);
            o.Position = position;
            this.Add(o);
            return o;
        }


        /// <summary>
        /// Luo oranssin tippuvan numeroblockin
        /// </summary>
        /// <param name="position">Positio kentällä</param>
        /// <param name="width">Olion leveys</param>
        /// <param name="height">Olion korkeus</param>
        /// <param name="args">Muut argumentit (jätetään huomiotta)</param>
        public GameObject CreateOrangeNumber(Vector position, double width, double height, object args)
        {
            OrangeNumberBlock o = new OrangeNumberBlock(grid, defaultSize, defaultSize, (int)args);
            o.Position = position;
            this.Add(o);
            return o;
        }
    }
}
