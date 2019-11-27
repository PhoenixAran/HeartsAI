using Hearts.Core;
using Hearts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartsAI
{
    public class RandomPlayer : Player
    {

        private Random _random;

        public RandomPlayer()
        {
            _random = new Random();
        }


        public override Card GetPlayCard( int trickNumber, Trick currentTrick )
        {
            
            if ( this.ShouldLead() && trickNumber == 1)
            {
                return ( Hand.First( c => c.CardRank == 2 && c.Suit == Suit.Clubs ) );
            }

           
            if ( currentTrick.Count == 0 )
            {
                if ( CanLeadHearts  || HandIsAllHeartsAndQueenOfSpades() ) //|| Hand.All( c => c.Suit == Suit.Hearts ) )
                {
                    //Just play any card
                    return Hand[_random.Next( 0, Hand.Count )];
                }
                else //Play a random card that is NOT a heart
                {
                    var tempList = ListPool<Card>.Obtain();

                    foreach ( Card card in Hand )
                    {
                        if ( card.Suit != Suit.Hearts )
                        {
                            tempList.Add( card );
                        }
                    }

                    var returnCard = tempList[_random.Next( 0, tempList.Count )];
                    ListPool<Card>.Free( tempList );
                    return returnCard;
                }
            }

            if ( this.HasSuit( currentTrick.LeadSuit ) )
            {
                var tempList = ListPool<Card>.Obtain();

                foreach ( Card card in Hand )
                {
                    if ( card.Suit == currentTrick.LeadSuit )
                    {
                        tempList.Add( card );
                    }
                }

                var returnCard = tempList[ _random.Next( 0, tempList.Count ) ];
                ListPool<Card>.Free( tempList );
                return returnCard;
            }
            else
            {
                return Hand[_random.Next( 0, Hand.Count )];
            }
           

            
        }

        public override void PassCards( int roundNumber, Player otherPlayer )
        {
            var tempList = ListPool<Card>.Obtain();
            
            for ( int i = 0; i < 3; ++i )
            {
                var randomIdx = _random.Next( 0, Hand.Count - 1 );
                var randomCard = Hand[randomIdx];
                Hand.RemoveAt( randomIdx );
                tempList.Add( randomCard );
            }

            otherPlayer.QueueRecieveCards( tempList );

            ListPool<Card>.Free( tempList );
        }


    }
}
