using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    abstract class Search
    {
        protected Robot _robot;
        protected Queue<Vector2> Path;

        protected Search(Robot robot)
        {
            _robot = robot;

            Path = new Queue<Vector2>();
        }

        /// <summary>
        /// Provides a path that will explore the grid.
        /// </summary>
        /// <returns>A queue of vectors represent the path to take.</returns>
        public abstract Queue<Vector2> GetPath();
    }
}
