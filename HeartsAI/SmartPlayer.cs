using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartsAI
{
    public abstract class SmartPlayer : Player
    {
        protected Node _tree;
        protected Node _current;
        Node _previous;
        public SmartPlayer( )
        {
            _tree = Pool<Node>.Obtain();
        }
        
        public override void NotifyHandFinalized()
        {
            _tree.Reset();
            _tree.SetHandState( this.Hand );
            _tree.GenerateBranches();
            _current = _tree;
        }

        public override void NotifyInitialLeadCardRemoved()
        {
            foreach ( var elem in _current.Children )
            {
                if ( elem.CardPlayed.Suit == Suit.Clubs && elem.CardPlayed.CardRank == 2 )
                {
                    _current = elem;
                    _current.GenerateBranches();
                    break;
                }
            }
        }

        public abstract void WeighNodeTree( int turnNumber, Trick currentTrick );

        public override Card GetPlayCard( int trickNumber, Trick currentTrick )
        {
            WeighNodeTree( trickNumber, currentTrick );

            var children = _current.Children;
            int idx = 0;
            for ( int i = 1; i < children.Count; ++i )
            {
                if ( children[i].Weight > children[idx].Weight )
                {
                    idx = i;
                }
            }

            _previous = _current;
            _current = children[idx];
            _current.GenerateBranches();
            return _current.CardPlayed;
        }



        protected bool TrickContainsPenaltyCards( Trick trick )
        {
            if ( trick.LeadSuit == Suit.Hearts )
            {
                return true;
            }

            foreach ( var card in trick.OrderedCards )
            {
                if ( card.Suit == Suit.Hearts )
                {
                    return true;
                }

                if ( card.CardRank == Card.QUEEN && card.Suit == Suit.Hearts )
                {
                    return true;
                }
            }

            return false;
        }

        protected Card GetMinCardInTrick( Trick trick )
        {
            var suit = trick.LeadSuit;
            var idx = -1;
            var minCardRank = 1;
            for ( int i = 0; i < trick.OrderedCards.Count; ++i )
            {
                var card = trick.OrderedCards[i];
                if ( card.Suit == suit && minCardRank < card.CardRank)
                {
                    idx = i;
                    minCardRank = card.CardRank;
                }
            }

            if ( idx == -1 )
            {
                throw new Exception( "SmartPlayer::GetMinCard failed due to void suit" );
            }

            return trick.OrderedCards[idx];
        }

    }
}
