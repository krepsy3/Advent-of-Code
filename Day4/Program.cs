using System.Text.RegularExpressions;

public class Program
{
    static void Main(string[] args)
    {
        Task1();
        Task2();
    }

    static void Task1()
    {
        using var sr = new StreamReader("input.txt");
        string? input = null;

        int result = 0;

        while ((input = sr.ReadLine()) is not null)
        {
            var c = ScratchCard.FromString(input);
            int cresult = c.GetWincount();

            if (cresult > 0)
            {
                result += 1 << (cresult - 1);
            }
        }

        Console.WriteLine(result);
    }

    static void Task2()
    {
        using var sr = new StreamReader("input.txt");
        string? input = null;

        List<ScratchCard> cards = new List<ScratchCard>();

        while ((input = sr.ReadLine()) is not null)
        {
            cards.Add(ScratchCard.FromString(input));
        }

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
    public static readonly Regex CardRegex = new Regex("Card *(?<id>[0-9]+): (?<winnings>( |[0-9])+)\\| (?<containing>( |[0-9])+)");

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