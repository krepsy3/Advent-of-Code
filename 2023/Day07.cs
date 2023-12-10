using Common;

namespace AoC2023.Day07;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        DoTask(loader, false);
        loader.Reset();
        DoTask(loader, true);
        loader.EndLoading();
    }

    void DoTask(InputLoader loader, bool cardJIsJoker)
    {
        string? line;
        List<Hand> hands = new List<Hand>();

        while ((line = loader.GetNextLine()) is not null)
        {
            hands.Add(Hand.FromString(line, cardJIsJoker));
        }

        long result = 0;

        hands.Sort(Hand.CompareHands);
        for (int i = 0; i < hands.Count; i++)
        {
            result += hands[i].Bid * (i + 1);
        }

        Console.WriteLine(result);
    }
}

public record class Hand(char[] Cards, int[] CardVals, int Bid, Hand.EKind Kind)
{
    public enum EKind { High = 1, Pair, TwoPairs, Threetuple, FullHouse, Fourtuple, Fivetuple }

    public static Hand FromString(string s, bool cardJIsJoker = false)
    {
        var parts = s.Trim().Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        char[] cards = parts[0].ToCharArray();
        int bid = Convert.ToInt32(parts[1]);

        int jokers = cardJIsJoker ? cards.Where(c => c == 'J').Count() : 0;
        IEnumerable<char> cardsSelection = cardJIsJoker ? cards.Where(c => c != 'J') : cards;
        int[] groups = jokers >= 5 ? new int[] { 0 } : cardsSelection.GroupBy(c => c).Select(g => g.Count()).ToArray();
        Array.Sort(groups, (i1, i2) => i2 - i1);
        groups[0] += jokers;

        EKind kind = groups switch
        {
            [5, ..]    => EKind.Fivetuple,
            [4, ..]    => EKind.Fourtuple,
            [3, 2, ..] => EKind.FullHouse,
            [3, ..]    => EKind.Threetuple,
            [2, 2, ..] => EKind.TwoPairs,
            [2, ..]    => EKind.Pair,
            _          => EKind.High
        };

        int[] cardVals = new int[cards.Length];
        string cardOrder = cardJIsJoker ? "J23456789TQKA" : "23456789TJQKA";

        for (int i = 0; i < cards.Length; i++)
        {
            cardVals[i] = cardOrder.IndexOf(cards[i]);
        }

        return new(cards, cardVals, bid, kind);
    }

    public static int CompareHands(Hand hand1, Hand hand2)
    {
        int diff = hand1.Kind - hand2.Kind;
        if (diff != 0)
        {
            return diff;
        }

        int i = 0, max = Math.Min(hand1.Cards.Length, hand2.Cards.Length);
        while (i < max)
        {
            diff = hand1.CardVals[i] - hand2.CardVals[i];
            if (diff != 0)
            {
                return diff;
            }

            i++;
        }

        return 0;
    }
}