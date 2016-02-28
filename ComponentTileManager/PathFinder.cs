using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{
    public static class PathFinder
    {
        public static Stack<Tile> StackPath(TileManager _tileManager, Tile Start, Tile Finish)
        {
            List<Tile> path = Path(_tileManager, Start, Finish);
            path.Reverse();
            Stack<Tile> stack = new Stack<Tile>();
            foreach (Tile t in path)
                stack.Push(t);
            return stack;

            }

        public static List<Tile> Path(TileManager _tileManager, Tile Start, Tile Finish)
        {
            if (Start == Finish)
                return new List<Tile> { Start, Finish };
            TileComparer compare = new TileComparer();
            Tile Current = Start;
            List<Tile> passable = _tileManager.ActiveLayer.Passable;
            List<Tile> visited = new List<Tile>();
            Stack<Tile> frontier = new Stack<Tile>();
            frontier.Push(Start);
            while (Current != null && Current != Finish)
            {
                visited.Add(Current);
                // get Neighbours
                var Neighbours = _tileManager.ActiveLayer.adjacentPassable(Current);
                // discount those already visited
                var NewNeighbours = Neighbours.Where(n => !visited.Contains(n, compare));
                // Get best neighbours in decreasing order as they are about to 
                // pushed onto the frontier
                var NextNearestNeighbour = NewNeighbours
                    .Select(nn => new { nn.X, nn.Y, Distance = ManhattanDistance(nn, Finish) })
                    .OrderByDescending(d => d.Distance)
                    .ToList();
                // get each candidate and add to the frontier in order of Heuristic distance 
                // furthest gets pushed first
                foreach (var node in NextNearestNeighbour)
                {
                    Tile Nextnode = (from n in NewNeighbours
                                     where n.X == node.X && n.Y == node.Y
                                     select n)
                             .FirstOrDefault();
                    if (Nextnode != null)
                        frontier.Push(Nextnode);
                }
                // Choose the next Neighbour as the shortest distance at the head of the stack
                if (frontier.Count > 0)
                    Current = frontier.Pop();
                else return null;

            }
            visited.Add(Finish);
            return visited;
        }
        public static int ManhattanDistance(Tile first, Tile second)
        {
            int abs_X = Math.Abs(first.X - second.X);
            int abs_Y = Math.Abs(first.Y - second.Y);
            return Math.Abs(abs_X + abs_Y);
        }
        public static int euclideanDistance(Tile first, Tile second)
        {
            if (first == second)
                return 0;

            Vector2 vfirst = new Vector2(first.X, first.Y);
            Vector2 vsecond = new Vector2(second.X, second.Y);
            int abs_X = Math.Abs(first.X - second.X);
            int abs_Y = Math.Abs(first.Y - second.Y);
            int distance = (int)Vector2.DistanceSquared(vfirst, vsecond);
            return distance;
        }

    }
}
