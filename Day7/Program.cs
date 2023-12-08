using System.Text;
using System.Text.RegularExpressions;

namespace Day7;

public class Program
{
    static void Main(string[] args)
    {
        Task1();
        Task2();
    }

    static void Task1()
    {
        var hands = ParseInputTask1();
        long result = 0;

        hands.Sort(Hand.CompareHands);
        for (int i = 0; i < hands.Count; i++)
        {
            result += hands[i].Bid * (i + 1);
        }

        Console.WriteLine(result);
    }

    static void Task2()
    {
        var hands = ParseInputTask2();
        long result = 0;

        hands.Sort(Hand.CompareHands);
        for (int i = 0; i < hands.Count; i++)
        {
            result += hands[i].Bid * (i + 1);
        }

        Console.WriteLine(result);
    }

    static List<Hand> ParseInputTask1()
    {
        List<Hand> result = new List<Hand>();

        using var sr = new StreamReader("input.txt");
        string? input = null;

        while ((input = sr.ReadLine()) is not null)
        {
            result.Add(Hand.FromStringTask1(input));
        }

        return result;
    }

    static List<Hand> ParseInputTask2()
    {
        List<Hand> result = new List<Hand>();

        using var sr = new StreamReader("input.txt");
        string? input = null;

        while ((input = sr.ReadLine()) is not null)
        {
            result.Add(Hand.FromStringTask2(input));
        }

        return result;
    }
}

public record class Hand(char[] Cards, int[] CardVals, int Bid, Kind Kind)
{
    public static Hand FromStringTask1(string s)
    {
        var parts = s.Trim().Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        char[] cards = parts[0].ToCharArray();
        int bid = Convert.ToInt32(parts[1]);
        int[] groups = cards.GroupBy(c => c).Select(g => g.Count()).ToArray();
        Array.Sort(groups, (i1, i2) => i2 - i1);
        Kind kind = groups switch
        {
            [5, ..]    => Kind.Fivetuple,
            [4, ..]    => Kind.Fourtuple,
            [3, 2, ..] => Kind.FullHouse,
            [3, ..]    => Kind.Threetuple,
            [2, 2, ..] => Kind.TwoPairs,
            [2, ..]    => Kind.Pair,
            _          => Kind.High
        };

        int[] cardVals = new int[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            cardVals[i] = "23456789TJQKA".IndexOf(cards[i]);
        }

        return new(cards, cardVals, bid, kind);
    }

    public static Hand FromStringTask2(string s)
    {
        var parts = s.Trim().Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        char[] cards = parts[0].ToCharArray();
        int bid = Convert.ToInt32(parts[1]);
        int jokers = cards.Where(c => c == 'J').Count();
        int[] groups = jokers >= 5 ? new int[] { 0 } : cards.Where(c => c != 'J').GroupBy(c => c).Select(g => g.Count()).ToArray();
        Array.Sort(groups, (i1, i2) => i2 - i1);
        groups[0] += jokers;
        Kind kind = groups switch
        {
            [5, ..]    => Kind.Fivetuple,
            [4, ..]    => Kind.Fourtuple,
            [3, 2, ..] => Kind.FullHouse,
            [3, ..]    => Kind.Threetuple,
            [2, 2, ..] => Kind.TwoPairs,
            [2, ..]    => Kind.Pair,
            _          => Kind.High
        };

        int[] cardVals = new int[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            cardVals[i] = "J23456789TQKA".IndexOf(cards[i]);
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

public enum Kind { High = 1, Pair, TwoPairs, Threetuple, FullHouse, Fourtuple, Fivetuple }