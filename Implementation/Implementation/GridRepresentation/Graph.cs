using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Tutorial: http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp

namespace Implementation.GridRepresentation
{
    public class Graph
    {
        //public static readonly Vector2[] Dirs =
        //{
        //    new Vector2(1, 0),
        //    new Vector2(0, -1),
        //    new Vector2(-1, 0),
        //    new Vector2(0, 1)
        //};

        public static readonly Vector2[] Dirs =
        {
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(1, 0)
        };

        public int Height { get; set; }
        public int Width { get; set; }
        public int Resolution { get; set; }
        public Cell[,] Cells { get; set; } 

        public Graph(int width, int height)
        {
            Cells = new Cell[width, height];
            Width = width;
            Height = height;
            Resolution = 32;
        }

        public bool InBounds(Vector2 position)
        {
            return position.X >= 0 && position.X < Width
                   && position.Y >= 0 && position.Y < Height;
        }

        public bool Walkable(Vector2 position)
        {
            return Cells[(int)position.X, (int)position.Y].Walkable;
        }

        public IEnumerable<Vector2> Neighbours(Vector2 position)
        {
            return Dirs.Select(dir => position + dir).Where(InBounds);
        }

        public IEnumerable<Vector2> WalkableNeighbours(Vector2 position)
        {
            return Dirs.Select(dir => position + dir)
                       .Where(InBounds)
                       .Where(next => Cells[(int) next.X, (int) next.Y] != null)
                       .Where(Walkable);
        }

        /// <summary>
        /// Get all of the unvisited neighbours of a position.
        /// </summary>
        /// <param name="position">The position to check from.</param>
        /// <returns>An IEnumerable of unvisited neighbours.</returns>
        public IEnumerable<Vector2> UnvisitedNeighbours(Vector2 position)
        {
            // Search for the unvisited neighbours.
            return WalkableNeighbours(position).Where(n => Cells[(int)n.X, (int)n.Y].Visited == 0);
        }

        public IEnumerable<Vector2> VisitedNeighbours(Vector2 position)
        {
            // Search for the unvisited neighbours.
            return WalkableNeighbours(position).Where(n => Cells[(int)n.X, (int)n.Y].Visited > 0);
        } 

        public IEnumerable<Vector2> GetPath(Vector2 from)
        {
            List<Vector2> path = new List<Vector2>();

            while (from != null)
            {
                Vector2 nonNull = (Vector2) from;
                path.Add(nonNull);

                Vector2? parent = Cells[(int)nonNull.X, (int)nonNull.Y].Parent;

                if (parent != null)
                    from = (Vector2) parent;
                else
                    break;
            }

            return path;
        } 
    }
}
