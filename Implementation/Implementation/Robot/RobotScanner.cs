using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;

namespace Implementation
{
    class RobotScanner
    {
        public int WorldX { get; set; }
        public int WorldY { get; set; }

        private Graph _graph;

        public RobotScanner(Graph graph)
        {
            _graph = graph;
        }

        //public Cell ScanCell()
        //{
        //    return _graph.Cells[WorldX / 32, WorldY / 32];
        //}
    }
}
