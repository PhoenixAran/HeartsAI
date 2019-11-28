using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;

namespace HeartsAI
{
    public class Node : IPoolable, IEquatable<Node>, IComparable<Node>
    {

        /// <summary>
        /// The path weight to this node
        /// </summary>
        public int Weight { get; set; } = 0;
        public List<Node> Children { get; private set; } = new List<Node>();
        public List<Card> HandState { get; private set; } = new List<Card>();
        public Card CardPlayed { get; set; }
        public void Reset()
        {
            Weight = 0;
            HandState.Clear();
            CardPlayed = null;
            for ( int i = Children.Count - 1; i >= 0; --i )
            {
                var childNode = Children[i];
                Pool<Node>.Free( childNode );
                Children.RemoveAt( i );
            }
        }

        public void SetHandState(IEnumerable<Card> cards)
        {
            HandState.AddRange( cards );
        }

        /// <summary>
        /// Generates layer of nodes.
        /// </summary>
        public void GenerateBranches()
        {
            if ( HandState.Count > 0 )
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


                    var childNode = Pool<Node>.Obtain();
                    childNode.SetHandState( tempList );
                    childNode.CardPlayed = HandState[i];
                    Children.Add( childNode );
                    tempList.Clear();

                }
                ListPool<Card>.Free( tempList );
            }
        }

        #region Object Overrides
        public override bool Equals( object other )
        {
            return other is Node otherNode && this.Equals( otherNode );
        }

        public override int GetHashCode()
        {
            return 1519321917 + EqualityComparer<List<Card>>.Default.GetHashCode( HandState );
        }
        #endregion

        #region IEquatable
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

        #region IComparable
        int IComparable<Node>.CompareTo( Node other ) => other.Weight.CompareTo( Weight );
        #endregion
    }
}
