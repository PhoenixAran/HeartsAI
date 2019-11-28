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
        #region Suit Trackers
        readonly Suit[] _suits = new[] {Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades};
        readonly int[] _pSuitCounts = new int[4];
        readonly double[] _pSuitScores = new double[4];

        private int GetSuitIndex( Suit suit )
        {
            int idx = -1;
            for ( int i = 0; i < _suits.Length; ++i )
            {
                if ( _suits[i] == suit )
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        private void IncrementSuitCount( Suit suit )
        {
            _pSuitCounts[GetSuitIndex( suit )] += 1;
        }

        private void IncrementSuitScore(Suit suit, int score )
        {
            _pSuitScores[GetSuitIndex( suit )] += score;
        }

        private void ResetSuitTrackers()
        {
            for ( int i = 0; i < _suits.Length; ++i )
            {
                _pSuitScores[i] = 0;
                _pSuitCounts[i] = 0;
            }
        }

        private void UpdateSuitTrackersFromHand()
        {
            ResetSuitTrackers();
            foreach ( var card in Hand )
            {
                IncrementSuitCount( card.Suit );
                IncrementSuitScore( card.Suit, card.CardRank );
            }
        }

        private Suit GetMaxScoreSuit()
        {
            double min = -1;
            int idx = -1;
            for ( int i = 0; i < _suits.Length; ++i )
            {
                if ( _pSuitScores[i] > min )
                {
                    idx = i;
                    min = _pSuitScores[i];
                }
            }

            return _suits[idx];
        }

        private Suit GetMaxCountSuit()
        {
            int min = -1;
            int idx = -1;
            for ( int i = 0; i < _suits.Length; ++i )
            {
                if ( _pSuitCounts[i] > min )
                {
                    idx = i;
                    min = _pSuitCounts[i];
                }
            }

            return _suits[idx];
        }

        private void AverageSuitScoresByCounts()
        {
            for ( int i = 0; i < _suits.Length; ++i )
            {
                _pSuitScores[i] = _pSuitScores[i] / _pSuitCounts[i];
            }
        }
        #endregion


        public override void PassCards( int roundNumber, Player otherPlayer )
        {
            var mod = roundNumber % 4;
            bool hasLeadCard = false;
            ResetSuitTrackers();
            foreach ( var card in Hand )
            {
                IncrementSuitCount( Suit.Clubs );
                if ( card.CardRank == 2 && card.Suit == Suit.Clubs )
                {
                    hasLeadCard = true;
       
                    IncrementSuitScore( Suit.Clubs, 5 );
                }
                else if ( card.CardRank == Card.QUEEN && card.Suit == Suit.Spades)
                {
                    var queenScore = mod == 1 ? 1 : Card.QUEEN;
                    IncrementSuitScore( Suit.Clubs, queenScore );
                }
                else
                {
                    IncrementSuitScore( card.Suit, card.CardRank );
                }

            }

            var maxSuit = GetMaxScoreSuit();

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
        }

        public override void WeighNodeTree( int turnNumber, Trick currentTrick )
        {
            UpdateSuitTrackersFromHand();
            var children = _current.Children;

            //Weigh nodes to try to win the first trick since there are no risks in winning the first one
            if ( turnNumber == 1 )
            {
                foreach ( var child in children )
                {
                    var cardPlayed = child.CardPlayed;
                    if ( cardPlayed.Suit == Suit.Spades && cardPlayed.CardRank == Card.QUEEN )
                    {
                        child.Weight = -50;
                    }
                    else if ( cardPlayed.Suit != currentTrick.LeadSuit )
                    {
                        child.Weight = -100;
                    }
                    else
                    {
                        child.Weight = cardPlayed.CardRank;
                    }
                }
                return;
            }

            throw new Exception( "Unexpected Condition in Voider::GetPlayCard" );
        }

        private void WeighNodeTreeWhereLeadSuitIsVoid( int turnNumber, Trick currentTrick )
        {
            var children = _current.Children;
        }
    }
}
