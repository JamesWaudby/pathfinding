using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.GridRepresentation
{
    public class Cell
    {
        public bool Walkable { get; set; }

        // Has the cell been visited already
        public int Visited { get; set; }

        public Vector2 Parent { get; set; }

        public Cell(bool walkable)
        {
            Walkable = walkable;
            Visited = 0;
        }

        protected bool Equals(Cell other)
        {
            return Walkable.Equals(other.Walkable);
        }

        public override int GetHashCode()
        {
            return Walkable.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
