using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CardGame.PhaseOne
{
    public enum SuitType { Diamonds, Spades, Hearts, Clubs }
    public record struct Card(SuitType Suit, int Rank);
    public record class Player(string name, IReadOnlyList<Card> Hand);
    public record class Deck(IReadOnlyList<Card> Cards)
    {
        public Card? Draw()
        {
            return Cards.FirstOrDefault();
        }

        public Deck WithoutTop()
        {
            return this with { Cards = Cards.Skip(1).ToList()};
        }
    }
    public record class GameState(Player Player, Deck Deck, string? LastMessage);

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

        public string BriefDescribe(Card card) => card switch
        {
            { Suit: var suit, Rank: var rank } => $"{suit}:{rank}"
        };

        public GameState DealerTurn(GameState game)
        {
            var newState = game with { LastMessage = null};
            while(newState.Player.Hand.Sum(CardValue) < 17)
            {
                Card? drawnCard = newState.Deck.Draw();
                if(drawnCard == null) break;
                newState = newState with {
                    Deck = newState.Deck.WithoutTop(), 
                    Player = newState.Player with { 
                        Hand = [..newState.Player.Hand, drawnCard.Value]
                    }
                };
            }
            return newState with { LastMessage = HandResult(newState.Player.Hand)};
        }

        public async IAsyncEnumerable<Card> DealCards(Deck deck, int count, [EnumeratorCancellation] CancellationToken ct )
        {
            for(int i = 0; i < count; i++)
            {
                var card = deck.Draw();
                deck = deck.WithoutTop();
                if (card == null)
                    yield break;
                ct.ThrowIfCancellationRequested();
                await Task.Delay(200, ct);
                yield return card.Value;
            }
        }

        public async Task<IReadOnlyList<Card>> DrawHandAsync(Deck deck, int count, CancellationToken ct = default)
        {
            var hand = new List<Card>();
            await foreach(var card in DealCards(deck, count, ct))
            {
                hand.Add(card);
            }
            return hand;
        }

        public int SerializeCard(Card card, Span<char> buffer)
        {
            var suit = card.Suit.ToString();
            var rank = card.Rank.ToString();
            int returnLength = suit.Length;
            suit.AsSpan().CopyTo(buffer);
            buffer[returnLength++] = ':';
            rank.AsSpan().CopyTo(buffer[returnLength..]);
            returnLength += rank.Length;
            return returnLength;
        }

        public Card ParseCard(ReadOnlySpan<char> data)
        {
            var separatorIndex = data.IndexOf(':');
            var suit = data[..separatorIndex];
            var rank = int.Parse(data[(separatorIndex+1)..]);
            return new Card(Enum.Parse<SuitType>(suit), rank);
        }

        public List<Card> BuildDeck()
        {
            List<Card> deck = new List<Card>(capacity: 52);
            int[] ranks = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
            Span<Card> deckSpan = CollectionsMarshal.AsSpan(deck);
            for(byte i = 0; i < 52; i++)
            {
                deckSpan[i] = new Card((SuitType)(i / 13), ranks[i % 13]);
            }
            return deck;
        }

        public List<Card> Shuffle(List<Card> currentDeck)
        {
            List<Card> newDeck = new List<Card>(capacity: currentDeck.Count);
            Span<Card> deckSpan = CollectionsMarshal.AsSpan(newDeck);
            var rand = new Random();
            for(int i = currentDeck.Count - 1; i > 0; i--)
            {
                var selectedIdx = rand.NextInt64(0, i + 1);
                deckSpan[i] = currentDeck[(int)selectedIdx];
            }
            return newDeck;
        }

        public List<IReadOnlyList<Card>> DealHands(List<Card> deck, int playerCount, int cardsEach)
        {
            var hands = new List<IReadOnlyList<Card>>(capacity: playerCount);
            Span<IReadOnlyList<Card>> handsSpan = CollectionsMarshal.AsSpan(hands);
            var currentIdx = 0;
            for(int i = 0; i < playerCount; i++)
            {
                handsSpan[i] = deck[currentIdx..(currentIdx + cardsEach)];
                currentIdx += cardsEach;
            }
            return hands;
        }
    }
}