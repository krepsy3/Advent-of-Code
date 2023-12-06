using System.Text;
using System.Text.RegularExpressions;

namespace Day6;

public class Program
{
    static void Main(string[] args)
    {
        Task1();
        Task2();
    }

    static void Task1()
    {
        var races = ParseInput1();
        long result = 1;

        foreach(var r in races)
        {
            long wait = 0;
            while ((wait * (r.limit - wait)) <= r.record)
                wait++;

            result *= r.limit + 1 - 2 * wait;
        }

        Console.WriteLine(result);
    }

    static void Task2()
    {
        var race = ParseInput2();
        long wait = 0;
        while ((wait * (race.limit - wait)) <= race.record)
            wait++;

        long result = race.limit + 1 - 2 * wait;

        Console.WriteLine(result);
    }

    static readonly Regex TimeLineRegex = new Regex("Time:(?<times>.+)");
    static readonly Regex DistLineRegex = new Regex("Distance:(?<distances>.+)");
    static readonly Regex LineNumberRegex = new Regex(" *(?<number>[0-9]+)");

    static List<Race> ParseInput1()
    {
        List<Race> result = new List<Race>();

        using var sr = new StreamReader("input.txt");
        string line1 = sr.ReadLine()!;
        string line2 = sr.ReadLine()!;

        line1 = TimeLineRegex.Match(line1).Groups["times"].Value;
        line2 = DistLineRegex.Match(line2).Groups["distances"].Value;

        Match m1 = LineNumberRegex.Match(line1);
        Match m2 = LineNumberRegex.Match(line2);

        while(m1.Success && m2.Success)
        {
            result.Add(new(
                Convert.ToInt32(m1.Groups["number"].Value),
                Convert.ToInt32(m2.Groups["number"].Value)
            ));

            m1 = m1.NextMatch();
            m2 = m2.NextMatch();
        }

        return result;
    }

    static Race ParseInput2()
    {
        using var sr = new StreamReader("input.txt");
        string line1 = sr.ReadLine()!;
        string line2 = sr.ReadLine()!;

        line1 = TimeLineRegex.Match(line1).Groups["times"].Value;
        line2 = DistLineRegex.Match(line2).Groups["distances"].Value;

        Match m1 = LineNumberRegex.Match(line1);
        Match m2 = LineNumberRegex.Match(line2);

        StringBuilder n1 = new StringBuilder();
        StringBuilder n2 = new StringBuilder();

        while (m1.Success && m2.Success)
        {
            n1.Append(m1.Groups["number"].Value);
            n2.Append(m2.Groups["number"].Value);
            m1 = m1.NextMatch();
            m2 = m2.NextMatch();
        }

        return new(
            Convert.ToInt64(n1.ToString()),
            Convert.ToInt64(n2.ToString())
        );
    }
}

public record class Race(long limit, long record) { }