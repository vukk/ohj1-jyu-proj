using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

// copypastaa koska kaikki rakastaa privatea...

namespace Mathplex
{
    public delegate GameObject TileMethod(Vector position, double width, double height, object arguments);

    /// @author Jypelin tekijät
    /// @author Un​to Ku​ur​an​ne
    /// @version 26.11.2011
    class TileMap
    {
        protected Dictionary<char, TileMethod> legend = new Dictionary<char, TileMethod>();
        protected Dictionary<char, object> args = new Dictionary<char, object>();

        private char[,] tiles;

        /// <summary>
        /// Määrittää, että tietyn ruutukentän merkin (<c>tileSymbol</c>) kohdalla
        /// kutsutaan aliohjelmaa <c>f</c>. Huom! Käytä tämän aliohjelman kanssa metodia
        /// <c>Execute</c> eikä <c>Insert</c>.
        /// </summary>
        public void SetTileMethod(char tileSymbol, TileMethod f, object arguments = null)
        {
            legend[tileSymbol] = f;
            args[tileSymbol] = arguments;
        }

        /// <summary>
        /// Luo uuden ruutukartan.
        /// </summary>
        /// <param name="tiles">Kaksiulotteinen taulukko merkeistä.</param>
        public TileMap( char[,] tiles )
        {
            if ( tiles.GetLength( 0 ) == 0 || tiles.GetLength( 1 ) == 0 )
                throw new ArgumentException( "All dimensions of tiles must be at least 1" );
            this.tiles = tiles;
        }

        /// <summary>
        /// Lukee ruutukentän tiedostosta.
        /// </summary>
        /// <param name="path">Tiedoston polku.</param>
        public static TileMap FromFile( string path )
        {
            char[,] tiles = ReadFromFile( path );
            return new TileMap( tiles );
        }

        /// <summary>
        /// Lukee ruutukentän merkkijonotaulukosta.
        /// </summary>
        /// <param name="lines">Merkkijonotaulukko</param>        
        public static TileMap FromStringArray(string[] lines)
        {
            char[,] tiles = ReadFromStringArray(lines);
            return new TileMap(tiles);
        }

        /// <summary>
        /// Lukee ruutukentän Content-projektin tekstitiedostosta.
        /// </summary>
        /// <param name="assetName">Tiedoston nimi</param>        
        public static TileMap FromLevelAsset(string assetName)
        {
            string[] lines = Peli.Instance.Content.Load<string[]>(assetName);
            char[,] tiles = ReadFromStringArray(lines);
            return new TileMap(tiles);
        }

        /// <summary>
        /// Käy kentän kaikki merkit läpi ja kutsuu <c>SetTileMethod</c>-metodilla annettuja
        /// aliohjelmia kunkin merkin kohdalla. Palauttaa täytetyn GridLogic-olion.
        /// </summary>
        /// <remarks>
        /// Aliohjelmassa voi esimerkiksi luoda olion ruudun kohdalle.
        /// </remarks>
        /// <param name="tileWidth">Yhden ruudun leveys.</param>
        /// <param name="tileHeight">Yhden ruudun korkeus.</param>
        /// <returns>Olioilla täytetty GridLogic-olio</returns>
        public GridLogic Execute(Grid grid)
        {
            Game game = Peli.Instance;
            int width = tiles.GetLength(1);
            int height = tiles.GetLength(0);

            GridLogic gridLogic = new GridLogic(grid, width, height);

            double tileWidth = grid.CellSize.X;
            double tileHeight = grid.CellSize.Y;

            // Tehdään kenttä sen kokoiseksi että luodut oliot ladotaan gridiin nätisti
            // ... ja Grid.SnapToCenter toimii fiksusti
            // Aina parillinen määrä kertoimena.
            if (width % 2 == 0)
                game.Level.Width = width * tileWidth + (2 * tileWidth);
            else
                game.Level.Width = (width * tileWidth) + (3 * tileWidth);

            if (height % 2 == 0)
                game.Level.Height = height * tileHeight + (2 * tileHeight);
            else
                game.Level.Height = (height * tileHeight) + (3 * tileHeight);


            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = tiles[y, x];
                    if (legend.ContainsKey(symbol))
                    {
                        TileMethod f = legend[symbol];
                        double realX = game.Level.Left + (x * tileWidth) + (tileWidth / 2);
                        double realY = game.Level.Top - (y * tileHeight) - (tileHeight / 2);
                        GameObject o = f(new Vector(realX, realY), tileWidth, tileHeight, args[symbol]);
                        (o as Block).GridLocation = new Vector(x, y);
                        gridLogic.Add(y, x, o);
                    }
                }
            }

            return gridLogic;
        }

        /// <summary>
        /// Lukee kentän ruudut tiedostosta.
        /// </summary>
        /// <param name="path">Tiedoston polku</param>
        /// <returns>Kentän ruudut kaksiulotteisessa taulukossa</returns>
        internal static char[,] ReadFromFile( string path )
        {
            var tileBuffer = new List<char[]>();            

            using (StreamReader input = File.OpenText(path))
            {
                string line;
                while ( ( line = input.ReadLine() ) != null )
                {
                    tileBuffer.Add( line.ToCharArray() );
                }
            }

            return ListToArray(tileBuffer);
        }

        internal static char[,] ReadFromStringArray(string[] lines)
        {
            var tileBuffer = new List<char[]>();

            for (int i = 0; i < lines.Length; i++ )
            {

                tileBuffer.Add(lines[i].ToCharArray());

            }

            return ListToArray(tileBuffer);
        }   

        private static char[,] ListToArray(List<char[]> list)
        {
            int finalWidth = list.Max(cs => cs.Length);

            char[,] tiles = new char[list.Count, finalWidth];

            for (int y = 0; y < list.Count; y++)
            {
                char[] row = list.ElementAt(y);

                for (int x = 0; x < row.Length; x++)
                {
                    tiles[y, x] = row[x];
                }
            }

            return tiles;
        }
    }
}
