using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

// Current phase
using CardGame.PhaseOne;

namespace CardGame
{
    public static class Program
    {
        public static  async Task Main(string[] args)
        {
            var game = new Game();
            
            var deck = game.Shuffle(game.BuildDeck());
            var handsList = game.DealHands(deck, 4, 5);
            handsList.ForEach(hand =>
            {
               Console.WriteLine("Player hand:");
               foreach(var card in hand)
                {
                    Console.WriteLine(game.Describe(card));
                }
            });
        }
    }
}