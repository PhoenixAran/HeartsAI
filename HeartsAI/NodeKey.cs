using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartsAI
{
    public class NodeKey : IEquatable<NodeKey>, IPoolable
    {
        private List<Card> _cards = new List<Card>();

        public void SetCards(IEnumerable<Card> cards)
        {
            _cards.AddRange( cards );
        }

        public void ClearCards()
        {
            _cards.Clear();
        }

        public void Reset()
        {
            ClearCards();
        }

        public override bool Equals( object obj )
        {
            return obj is NodeKey other && this.Equals( other );
        }

        public bool Equals( NodeKey other )
        {
            foreach ( var elem in other._cards)
            {
                if ( !ContainsCard( elem ) )
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return 1217012356 + EqualityComparer<List<Card>>.Default.GetHashCode( _cards );
        }

        private bool ContainsCard( Card card )
        {
            foreach( var elem in _cards)
            {
                if (elem == card)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
