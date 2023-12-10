using Common;
using System.Text.RegularExpressions;

namespace AoC2023.Day4;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        string? line;
        var cards = new List<ScratchCard>();
        while ((line = loader.GetNextLine()) is not null)
        {
            cards.Add(ScratchCard.FromString(line));
        }

        loader.EndLoading();

        Task1(cards);
        Task2(cards);
    }

    void Task1(IEnumerable<ScratchCard> cards)
    {
        int result = 0;
        foreach (ScratchCard c in cards)
        { 
            int cresult = c.GetWincount();

            if (cresult > 0)
            {
                result += 1 << (cresult - 1);
            }
        }

        Console.WriteLine(result);
    }

    void Task2(IList<ScratchCard> cards)
    {
        int[] cardCounts = Enumerable.Repeat(1, cards.Count).ToArray();

        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var wins = card.GetWincount();
            for (int j = 1; j <= wins; j++)
            {
                cardCounts[i + j] += cardCounts[i];
            }
        }

        Console.WriteLine(cardCounts.Sum());
    }
}

public record class ScratchCard(int id, List<int> winnings, List<int> containing)
{
    public static readonly Regex CardRegex = new Regex("Card *(?<id>[0-9]+): (?<winnings>[0-9 ]+)\\| (?<containing>[0-9 ]+)");

    public static ScratchCard FromString(string input)
    {
        var m = CardRegex.Match(input);
        return new ScratchCard(
            Convert.ToInt32(m.Groups["id"].Value),
            ProcessNums(m.Groups["winnings"].Value),
            ProcessNums(m.Groups["containing"].Value));
    }

    private static List<int> ProcessNums(string input)
    {
        var nums = input.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<int>(nums.Length);

        foreach (var num in nums)
        {
            result.Add(Convert.ToInt32(num));
        }

        return result;
    }

    public int GetWincount()
    {
        int result = 0;
        foreach (var cont in containing)
        {
            if (winnings.IndexOf(cont) >= 0)
            {
                result++;
            }
        }

        return result;
    }
}