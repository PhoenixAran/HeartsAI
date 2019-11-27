using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;

namespace HeartsAI
{
    public class Node : IPoolable, IEquatable<Node>
    {

        /// <summary>
        /// The path weight to this node
        /// </summary>
        public int Weight { get; set; } = 0;
        public List<Node> Children { get; private set; } = new List<Node>();
        public List<Card> HandState { get; private set; } = new List<Card>();
        
        public void Reset()
        {
            Weight = 0;
            HandState.Clear();
            for( int i = Children.Count - 1; i >= 0; --i )
            {
                var childNode = Children[i];
                childNode.Reset();
                Pool<Node>.Free( childNode );
            }
        }

        public void SetHandState(IEnumerable<Card> cards)
        {
            HandState.AddRange( cards );
        }

        public void GenerateBranches(Dictionary<NodeKey, Node> map)
        {
            if ( HandState.Count > 1)
            {
                var tempList = ListPool<Card>.Obtain();

                for ( int i = 0; i < HandState.Count; ++i )
                {
                    for ( int j = 0; j < HandState.Count; ++j )
                    {
                        if ( i != j )
                        {
                            tempList.Add( HandState[j] );
                        }
                    }

                    var nodeKey = Pool<NodeKey>.Obtain();
                    nodeKey.SetCards( tempList );
                    if ( map.TryGetValue(nodeKey, out var child) )
                    {
                        Children.Add( child );
                        Pool<NodeKey>.Free( nodeKey );
                    }
                    else
                    {
                        var childNode = Pool<Node>.Obtain();
                        childNode.SetHandState( tempList );
                        map.Add( nodeKey, childNode );
                        childNode.GenerateBranches(map);
                        Children.Add( childNode );
                    }

                    tempList.Clear();

                }

                ListPool<Card>.Free( tempList );
            }
        }

        #region Object Overrides
        public bool Equals( Node other )
        {
            foreach ( var card in other.HandState )
            {
                if ( !HandContainsCard( card ) )
                {
                    return false;
                }
            }

            return true;
        }
        public override int GetHashCode()
        {
            return 1519321917 + EqualityComparer<List<Card>>.Default.GetHashCode( HandState );
        }
        #endregion

        #region IEquatable
        public override bool Equals( object other )
        {
            return other is Node otherNode && this.Equals( otherNode );
        }

        public bool HandContainsCard( Card card )
        {
            foreach ( var elem in HandState )
            {
                if ( !elem.Equals( card ) )
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
