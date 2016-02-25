using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{
    public class TileComparer : IEqualityComparer<Tile>

    {
        public int GetHashCode(Tile t)
        {
            return t.GetHashCode();
        }

        public bool Equals(Tile tileA, Tile tileB)
        {
            if (tileA.X == tileB.X && tileA.Y == tileB.Y)
                return true;
            else return false;

        }
    }
}
