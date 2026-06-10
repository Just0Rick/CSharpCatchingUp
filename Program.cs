using System.Collections.Generic;
using System.Collections.Immutable;

// Current phase
using CardGame.PhaseOne;

namespace CardGame
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var game = new Game();
            var rick = new Player("Rick", new List<Card>());
            var sampleCard = new Card(SuitType.Spades, 1);

            rick = game.DrawCard(rick, new Card(SuitType.Spades, 1));
            rick = game.DrawCard(rick, new Card(SuitType.Clubs, 10));

            Console.WriteLine(rick);
            var rand = new Random();
            var deckList = (new byte[10]).Select((_) =>
            {
                var enums = Enum.GetValues<SuitType>();
                (int suit, int rank) = (rand.Next() % 4, (rand.Next() % 12) + 1);
               return new Card(enums[suit], rank); 
            });

            Array.ForEach(deckList.ToArray(), (c) => Console.Write(c) );
            Console.WriteLine();
            var gameState = new GameState(rick, new Deck(deckList.ToImmutableList()), null);
            Console.WriteLine($"Last message: {game.DealerTurn(gameState).LastMessage}");
        }
    }
}