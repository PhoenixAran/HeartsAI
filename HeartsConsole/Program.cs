using Hearts.Core;
using Hearts.Utils;
using HeartsAI;
using System;

namespace HeartsConsole
{
    class Program
    {
        static void RandomPlayerTest()
        {
            
            //Set up random players
            var player1 = new RandomPlayer();
            var player2 = new RandomPlayer();
            var player3 = new RandomPlayer();
            var player4 = new RandomPlayer();

            var points = new int[4];

            //Set up game
            var game = new HeartsGame();

            //Add the players
            game.Players.AddRange( new[] { player1, player2, player3, player4 } );


            for ( int i = 0; i < 1000000; ++i )
            {
                game.PlayRound();
                for ( int j = 0; j < 4; ++j )
                {
                    points[j] = game.Players[j].Points;
                }
                game.Reset();

            }

            Console.WriteLine( $"Player1: {points[0]}" );
            Console.WriteLine( $"Player2: {points[1]}" );
            Console.WriteLine( $"Player3: {points[2]}" );
            Console.WriteLine( $"Player4: {points[3]}" );
            Console.ReadLine();
        }

        static void LowLayerTest()
        {

            //Set up random players
            var player1 = new LowLayer();
            var player2 = new RandomPlayer();
            var player3 = new RandomPlayer();
            var player4 = new RandomPlayer();

            var points = new int[4];

            //Set up game
            var game = new HeartsGame();

            //Add the players
            game.Players.AddRange( new Player[] { player1, player2, player3, player4 } );


            for ( int i = 0; i < 1000000; ++i )
            {
                game.PlayRound();
                for ( int j = 0; j < 4; ++j )
                {
                    points[j] = game.Players[j].Points;
                }
                game.Reset();

            }

            Console.WriteLine( $"LowLayer: {points[0]}" );
            Console.WriteLine( $"Player2: {points[1]}" );
            Console.WriteLine( $"Player3: {points[2]}" );
            Console.WriteLine( $"Player4: {points[3]}" );
            Console.ReadLine();
        }

        static void Voider()
        {

            //Set up random players
            var player1 = new Voider();
            var player2 = new RandomPlayer();
            var player3 = new RandomPlayer();
            var player4 = new RandomPlayer();

            var points = new int[4];

            //Set up game
            var game = new HeartsGame();

            //Add the players
            game.Players.AddRange( new Player[] { player1, player2, player3, player4 } );


            for ( int i = 0; i < 1000; ++i )
            {
                game.PlayRound();
                for ( int j = 0; j < 4; ++j )
                {
                    points[j] += game.Players[j].Points;
                }
                game.Reset();

            }

            Console.WriteLine( $"LowLayer: {points[0]}" );
            Console.WriteLine( $"Player2: {points[1]}" );
            Console.WriteLine( $"Player3: {points[2]}" );
            Console.WriteLine( $"Player4: {points[3]}" );
            Console.ReadLine();
        }

        static void SplitTest()
        {
            //Set up random players
            var player1 = new Voider();
            var player2 = new LowLayer();
            var player3 = new Voider();
            var player4 = new LowLayer();

            var points = new int[4];

            //Set up game
            var game = new HeartsGame();

            //Add the players
            game.Players.AddRange( new Player[] { player1, player2, player3, player4 } );


            for ( int i = 0; i < 1000; ++i )
            {
                game.PlayRound();
                for ( int j = 0; j < 4; ++j )
                {
                    points[j] += game.Players[j].Points;
                }
                game.Reset();

            }

            Console.WriteLine( $"LowLayer1: {points[0]}" );
            Console.WriteLine( $"Voider1: {points[1]}" );
            Console.WriteLine( $"LowLayer2: {points[2]}" );
            Console.WriteLine( $"Voider2: {points[3]}" );
            Console.ReadLine();
        }

        static void Main( string[] args )
        {
            //Set up object pools
            Pool<Node>.WarmCache( 10000 );
            Pool<Trick>.WarmCache( 100 );
            ListPool<Trick>.WarmCache( 100 );
            ListPool<Card>.WarmCache( 800 );
            SplitTest();
            
            
        }
    }
}
