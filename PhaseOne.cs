namespace CardGame.PhaseOne
{
    public enum SuitType { Diamonds, Spades, Hearts, Clubs }
    public record struct Card(SuitType Suit, int Rank);
    public record class Player(string name, IReadOnlyList<Card> Hand);

    public class Game
    {
        public Player DrawCard(Player player, Card card)
        {
            var newHand = player.Hand.Append(card).ToList();
            return player with {Hand = newHand};
        }

        public int CardValue(Card card) => card.Rank switch
        {
            1 => 11,
            > 9 and < 14 => 10,
            _ => card.Rank
        };

        public string HandResult(IReadOnlyList<Card> hand)
        {
            var total = hand.Sum(CardValue);
            return total switch {
                > 21 => "Bust",
                21 => "Blackjack!",
                >= 17 => "Stand",
                _ => "Hit"
            };
        }

        public string RankName(int rank) => rank switch
        {
            1 or 14 => "Ace",
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            _ => $"{rank}"
        };

        public string Describe(Card card) => card switch
        {
            { Suit: var suit, Rank: var rank } => $"{RankName(rank)} of {suit}"
        };
    }
}