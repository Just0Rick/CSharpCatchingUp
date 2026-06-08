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

            Console.WriteLine($"Card value: {game.CardValue(sampleCard)}");
            Console.WriteLine($"Hand result: {game.HandResult(rick.Hand)}");
            Console.WriteLine($"Card described: {game.Describe(sampleCard)}");
        }
    }
}