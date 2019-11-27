using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartsAI
{
    public class SmartPlayer 
    {
        INodeWeighter _weighter;

        Node _tree;
        Dictionary<NodeKey, Node> _map;

        public SmartPlayer( INodeWeighter weighter )
        {
            _weighter = weighter;
            _tree = Pool<Node>.Obtain();
            _map = new Dictionary<NodeKey, Node>();
        }

        public void GenerateTree(IEnumerable<Card> hand)
        {
            foreach(var elem in _map.Keys)
            {
                Pool<NodeKey>.Free( elem );
            }
            _map.Clear();
            _tree.Reset();
            //_tree = Pool<Node>.Obtain();
            _tree.SetHandState( hand );
            _tree.GenerateBranches(_map);
            Console.WriteLine( "LUL" );
        }
    }
}
