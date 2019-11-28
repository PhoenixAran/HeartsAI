using System;
using System.Collections.Generic;
using System.Text;
using Hearts.Core;
using Hearts.Utils;

namespace HeartsAI
{
    public class LowLayer : SmartPlayer
    {
       
        public override void PassCards( int roundNumber, Player otherPlayer )
        {
            var tempList = ListPool<Card>.Obtain();
            Hand.Sort();

            for ( int i = Hand.Count - 1, j = 0; j < 3; ++j, --i )
            {
                tempList.Add( Hand[i] );
                Hand.RemoveAt( i );
            }

            otherPlayer.QueueRecieveCards( tempList );

            ListPool<Card>.Free( tempList );
        }

        public override void WeighNodeTree( int turnNumber, Trick currentTrick )
        {
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

            if ( currentTrick.Count > 0 && !HasSuit(currentTrick.LeadSuit) )
            {
                WeighNodeTreeWhereLeadSuitIsVoid( turnNumber, currentTrick );
                return;
            }

            if ( currentTrick.Count == 0 && CanLeadHearts )
            {
                foreach ( var child in children )
                {
                    child.Weight = child.CardPlayed.CardRank;
                }
                return;
            }


            //Can't lead with hearts
            if ( currentTrick.Count == 0 )
            {
                foreach ( var child in children )
                {
                    var cardPlayed = child.CardPlayed;
                    if ( cardPlayed.Suit == Suit.Hearts )
                    {
                        child.Weight =  -cardPlayed.CardRank;
                    }
                    else if ( cardPlayed.Suit == Suit.Spades && cardPlayed.CardRank == Card.QUEEN )
                    {
                        child.Weight = 0;
                    }
                    else
                    {
                        child.Weight = Card.MAX_CARD_RANK - cardPlayed.CardRank;
                    }
                }
                return;
            }

            if ( currentTrick.Count > 0 )
            {
                var leadSuit = currentTrick.LeadSuit;
                var minCard = GetMinCardInTrick( currentTrick );
                foreach ( var child in children )
                {
                    var cardPlayed = child.CardPlayed;
                    if ( cardPlayed.Suit != leadSuit )
                    {
                        child.Weight = -100;
                    }
                    else
                    {
                        if ( cardPlayed.CardRank < minCard.CardRank )
                        {
                            child.Weight = 100 + (14) - ( minCard.CardRank - cardPlayed.CardRank );
                        }
                        else
                        {
                            child.Weight = 100 - ( minCard.CardRank - cardPlayed.CardRank );
                        }
                    }
                }
                return;
            }
            throw new Exception( "Unexpected Condition in LowLayer::GetPlayCard" );
        }

        private void WeighNodeTreeWhereLeadSuitIsVoid( int turnNumber, Trick currentTrick )
        {
            var children = _current.Children;

            foreach ( var child in children )
            {
                var cardPlayed = child.CardPlayed;
                child.Weight = cardPlayed.CardRank;

                if ( cardPlayed.Suit == Suit.Hearts )
                {
                    child.Weight += 50;
                }
                else if (cardPlayed.Suit == Suit.Spades && cardPlayed.CardRank == Card.QUEEN )
                {
                    child.Weight += 150;
                }
            }
        }

    
    }
}
