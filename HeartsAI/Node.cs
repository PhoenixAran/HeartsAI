using System;
using System.Collections.Generic;

namespace HeartsAI
{
    public class Node
    {
        public List<Node> Children { get; set; }

        /// <summary>
        /// The path weight to this node
        /// </summary>
        public int Weight { get; set; }
    }
}
