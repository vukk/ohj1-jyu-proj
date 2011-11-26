using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jypeli;

namespace Mathplex
{
    /// @author Un​to Ku​ur​an​ne
    /// @version 26.11.2011
    /// <summary>
    /// Implementoi pelin palikoiden liikkumislogiikkaa
    /// </summary>
    public class GridLogic
    {
        /// <summary>
        /// Grid
        /// </summary>
        protected Grid grid;
        /// <summary>
        /// Grid-matriisi, sisältää kentän tämänhetkeisen tilan
        /// matriisimuodossa
        /// </summary>
        protected GameObject[,] level;

        /// <summary>
        /// Uusi GridLogic olio
        /// </summary>
        /// <param name="grid">Grid</param>
        /// <param name="width">Gridin leveys</param>
        /// <param name="height">Gridin korkeus</param>
        public GridLogic(Grid grid, int width, int height)
        {
            this.grid = grid;
            level = new GameObject[height, width];
        }


        /// <summary>
        /// Lisää objektin grid-matriisiin
        /// </summary>
        /// <param name="row">Rivi</param>
        /// <param name="col">Sarake</param>
        /// <param name="o">Objekti</param>
        public void Add(int row, int col, GameObject o)
        {
            level[row, col] = o;
        }


        /// <summary>
        /// Palauttaa objektin grid-matriisista
        /// </summary>
        /// <param name="row">Rivi</param>
        /// <param name="col">Sarake</param>
        /// <returns>Objekti</returns>
        public GameObject Get(int row, int col)
        {
            return level[row, col];
        }


        /// <summary>
        /// Poistaa objektin grid-matriisista
        /// (jos sellainen annetussa paikassa on)
        /// </summary>
        /// <param name="row">Rivi</param>
        /// <param name="col">Sarake</param>
        public void Remove(int row, int col)
        {
            level[row, col] = null;
        }


        /// <summary>
        /// Saapumisilmoitus, jokin objekti gridissä on liikkunut
        /// paikasta toiseen ja saapunut perille.
        /// </summary>
        /// <param name="o">Liikkunut objekti</param>
        /// <param name="from">Paikka gridissä mistä liikuttiin</param>
        /// <param name="to">Paikka gridissä mihin liikuttiin</param>
        public void Arrived(GameObject o, Vector from, Vector to)
        {
            this.Remove((int)from.Y, (int)from.X);
            this.Add((int)to.Y, (int)to.X, o);
        }


        /// <summary>
        /// Pelaaja syö jonkin objektin, tällöin pelaaja asetetaan
        /// kahteen paikkaan samanaikaisesti, lähtöpisteeseensä
        /// ja siihen pisteeseen josta objekti syötiin.
        /// </summary>
        /// <param name="p">Pelaaja</param>
        /// <param name="gridLocation">Paikka gridissä josta objekti syötiin</param>
        public void PlayerBlockEats(PlayerBlock p, Vector gridLocation)
        {
            this.Add((int)gridLocation.Y, (int)gridLocation.X, p);
        }


        /// <summary>
        /// Hakee gridin paikan perusteella ympäröivät objektit.
        /// Ei palauta tuhottuja objekteja tai tyhjien paikkojen nulleja.
        /// </summary>
        /// <param name="gridLocation">Paikka gridissä</param>
        /// <returns>Lista objekteista annetun paikan ympärillä</returns>
        public List<GameObject> GetImmediateSurroundings(Vector gridLocation)
        {
            int col = (int)gridLocation.X;
            int row = (int)gridLocation.Y;

            List<GameObject> res = new List<GameObject>();

            int startRow = row - 1;
            int stopRow = row + 1;
            int startCol = col - 1;
            int stopCol = col + 1;

            if (startRow < 0) startRow = 0;
            if (stopRow >= level.GetLength(0)) stopRow = level.GetLength(0) - 1;
            if (startCol < 0) startCol = 0;
            if (stopCol >= level.GetLength(1)) stopCol = level.GetLength(1) - 1;

            for (int iy = startRow; iy <= stopRow; iy++)
                for (int ix = startCol; ix <= stopCol; ix++)
                    if (level[iy, ix] != null && !level[iy, ix].IsDestroyed)
                        res.Add(level[iy, ix]);

            return res;
        }


        /// <summary>
        /// Voiko block liikkua paikkaan <c>to</c> vai ei, kun
        /// rajoittavat tekijät on annettu listassa <c>limitFactors</c>
        /// </summary>
        /// <param name="to">Paikka gridissä johon liikkua</param>
        /// <param name="limitFactors">Rajoittavat tekijät</param>
        /// <returns>Voidaanko paikkaan liikkua</returns>
        public bool CanMoveTo(Vector to, List<Predicate<Block>> limitFactors)
        {
            int col = (int)to.X;
            int row = (int)to.Y;

            return CanMoveTo(row, col, limitFactors);
        }


        /// <summary>
        /// Voiko block liikkua paikkaan <c>row, col</c> vai ei, kun
        /// rajoittavat tekijät on annettu listassa <c>limitFactors</c>
        /// </summary>
        /// <param name="row">Rivi</param>
        /// <param name="col">Sarake</param>
        /// <param name="limitFactors">Rajoittavat tekijät</param>
        /// <returns>Voidaanko paikkaan liikkua</returns>
        public bool CanMoveTo(int row, int col, List<Predicate<Block>> limitFactors)
        {
            if (!IsInsideLevel(row, col))
            {
                Debug.Print("Moving Block was limited by level boundaries row: " + row + " col: " + col);
                //Debug.Print("MinRow: 0, MaxRow: " + level.GetUpperBound(0) + " MinCol: 0, MaxCol: " + level.GetUpperBound(1));
                return false;
            }

            // check that none of the limiting factors apply to the object at location to
            // ... if there is an object there
            // ... so: if all predicates yield true, then movement is limited
            Block atLoc = level[row, col] as Block;
            if (atLoc != null && limitFactors.All(predicate => predicate(atLoc) == true)) return false;

            return true;
        }


        /// <summary>
        /// Onko <c>row, column</c> "kentän sisällä", grid-matriisissa
        /// </summary>
        /// <param name="row">Rivi</param>
        /// <param name="col">Sarake</param>
        /// <returns>Onko paikka kentän sisällä vai ei</returns>
        public bool IsInsideLevel(int row, int col)
        {
            if (col > level.GetUpperBound(1) || col < level.GetLowerBound(1) || row > level.GetUpperBound(0) || row < level.GetLowerBound(0))
                return false;

            return true;
        }


        /// <summary>
        /// Päivitysmetodi
        /// Jokaisella päivityksellä tarkastetaan tiputettavat palikat.
        /// </summary>
        /// <param name="time">Aika</param>
        public void Update(Time time)
        {
            int startRow = level.GetLowerBound(0);
            int stopRow = level.GetUpperBound(0);
            int startCol = level.GetLowerBound(1);
            int stopCol = level.GetUpperBound(1);

            // lista blockeista jotka dropataan ruudun yläpuolelta jos
            // nykyisessä ruudussa on tyhjää tai siihen muuten voidaan pudota
            List<DroppingBlock> backlog = new List<DroppingBlock>();

            // run through column1 => rows, column2 => rows...
            for (int col = startCol; col < stopCol; col++)
            {
                for (int row = startRow; row < stopRow; row++)
                {
                    Block b = level[row, col] as Block;

                    // remove destroyed
                    if (b != null && b.IsDestroyed)
                    {
                        level[row, col] = null;
                        continue;
                    }

                    // droppaavat blockit
                    if (b != null && b is DroppingBlock)
                    {
                        DroppingBlock block = b as DroppingBlock;
                        if (block.IsMoving)
                        {
                            backlog.ForEach(x => x.Drop());
                            backlog.Clear();
                        }
                        else
                        {
                            backlog.Add(block);
                        }
                    } // tyhjät
                    else if (b == null || b.IsDestroyed)
                    {
                        backlog.ForEach(x => x.Drop());
                        backlog.Clear();
                    }
                    else
                    {
                        backlog.Clear();
                    }
                }

                // clear backlog between columns
                backlog.Clear();
            }
        }
    }
}
