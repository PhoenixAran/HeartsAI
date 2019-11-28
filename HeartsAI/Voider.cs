using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hearts.Core;
using Hearts.Utils;

namespace HeartsAI
{
    public class Voider : SmartPlayer
    {

        /// <summary>
        /// Dictionary used for passing cards
        /// </summary>
        Dictionary<Suit, decimal> _suitScore = new Dictionary<Suit, decimal>();
        Dictionary<Suit, decimal> _suitCounter = new Dictionary<Suit, decimal>();
        readonly Suit[] _suits = new[] {Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades};
        public override void PassCards( int roundNumber, Player otherPlayer )
        {
            var mod = roundNumber % 4;
            bool hasLeadCard = false;
            foreach ( var card in Hand )
            {
                if ( card.CardRank == 2 && card.Suit == Suit.Clubs )
                {
                    hasLeadCard = true;
                    if ( _suitScore.ContainsKey( Suit.Clubs ) )
                    {
                        _suitScore[Suit.Clubs] += 5;
                        _suitCounter[Suit.Clubs] += 1;
                    }
                    else
                    {
                        _suitScore[Suit.Clubs] = 5;
                        _suitCounter[Suit.Clubs] = 1;
                    }
                }
                else if ( card.CardRank == Card.QUEEN && card.Suit == Suit.Spades)
                {
                    var queenScore = mod == 1 ? 1 : Card.QUEEN;
                    if ( _suitScore.ContainsKey( Suit.Clubs ) )
                    {
                        _suitScore[Suit.Clubs] += queenScore;
                        _suitCounter[Suit.Clubs] += 1;
                    }
                    else
                    {
                        _suitScore[Suit.Clubs] = queenScore;
                        _suitCounter[Suit.Clubs] = 1;
                    }
                }
                else
                {
                    if ( _suitScore.ContainsKey( card.Suit ) )
                    {
                        _suitScore[card.Suit] += card.CardRank;
                        _suitCounter[card.Suit] += 1;
                    }
                    else
                    {
                        _suitScore[card.Suit] = card.CardRank;
                        _suitCounter[card.Suit] = 1;
                    }
                }

            }

            foreach ( Suit suit in _suits )
            {
                _suitScore[suit] = _suitScore[suit] / _suitCounter[suit];
            }

            var maxSuit = _suitScore.Select( kv => kv )
                      .OrderBy( kv => kv.Value )
                      .Last().Key;

            var tempList = ListPool<Card>.Obtain();
            Hand.Sort();

            int jLimit = 3;
            if ( hasLeadCard )
            {
                jLimit = 2;
                var leadCard = Hand.First( c => c.CardRank == 2 && c.Suit == Suit.Clubs );
                tempList.Add( leadCard );
                Hand.Remove( leadCard );
            }

            for ( int i = Hand.Count - 1, j = 0; i >= 0 || j == jLimit; --i)
            {
                var card = Hand[i];
                if ( card.Suit == maxSuit )
                {
                    tempList.Add( card );
                    ++jLimit;
                    Hand.RemoveAt( i );
                }
            }

            if ( tempList.Count < 3 )
            {
                for ( int i = Hand.Count - 1; tempList.Count < 3; --i )
                {
                    tempList.Add( Hand[i] );
                    Hand.RemoveAt( i );
                }
            }


            otherPlayer.QueueRecieveCards( tempList );
            ListPool<Card>.Free( tempList );
            _suitScore.Clear();
            _suitCounter.Clear();
        }

        public override void WeighNodeTree( int turnNumber, Trick currentTrick )
        {
            throw new NotImplementedException();
        }
    }
}
