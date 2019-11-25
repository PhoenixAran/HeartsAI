using Hearts.Core;
using Hearts.Utils;
using HeartsAI;
using System;

namespace HeartsConsole
{
    class Program
    {
        static void Main( string[] args )
        {
            //Set up object pools
            Pool<Node>.WarmCache( 100 );
            Pool<Trick>.WarmCache( 100 );
            ListPool<Trick>.WarmCache( 100 );
            ListPool<Card>.WarmCache( 100 );

            //Set up random players
            var player1 = new RandomPlayer();
            var player2 = new RandomPlayer();
            var player3 = new RandomPlayer();
            var player4 = new RandomPlayer();

            //Set up game
            var game = new HeartsGame();

            //Add the players
            game.Players.AddRange( new[] { player1, player2, player3, player4 } );
            

            for ( int i = 0; i < 13; ++i )
            {
                game.PlayRound();
            }

            Console.WriteLine( $"Player1: {player1.Points}" );
            Console.WriteLine( $"Player2: {player2.Points}" );
            Console.WriteLine( $"Player3: {player3.Points}" );
            Console.WriteLine( $"Player4: {player4.Points}" );
            Console.ReadLine();
        }
    }
}
