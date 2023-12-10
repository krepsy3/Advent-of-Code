using Common;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023.Day6;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        DoTask1(loader);
        loader.Reset();
        DoTask2(loader);
        loader.EndLoading();
    }

    void DoTask1(InputLoader loader)
    {
        var races = ParseInput(
            loader,
            new List<Race>(),
            (l, t, d) =>
            {
                l.Add(new(t, d));
                return l;
            },
            l => l
        );

        Console.WriteLine(Compute(races));
    }

    void DoTask2(InputLoader loader)
    {
        var races = ParseInput(
            loader,
            (new StringBuilder(), new StringBuilder()),
            (bb, t, d) =>
            {
                bb.Item1.Append(t);
                bb.Item2.Append(d);
                return bb;
            },
            bb => Enumerable.Repeat(
                new Race(
                    Convert.ToInt64(bb.Item1.ToString()),
                    Convert.ToInt64(bb.Item2.ToString())),
                1
            )               
        );

        Console.WriteLine(Compute(races));
    }

    long Compute(IEnumerable<Race> races)
    {
        long result = 1;
        foreach (var r in races)
        {
            long wait = 0;
            while ((wait * (r.limit - wait)) <= r.record)
                wait++;

            result *= r.limit + 1 - 2 * wait;
        }

        return result;
    }

    static readonly Regex TimeLineRegex = new Regex("Time:(?<times>.+)");
    static readonly Regex DistLineRegex = new Regex("Distance:(?<distances>.+)");
    static readonly Regex LineNumberRegex = new Regex(" *(?<number>[0-9]+)");

    IEnumerable<Race> ParseInput<I>(InputLoader loader, I initialResult, Func<I, int, int, I> timeDistanceProcessor, Func<I, IEnumerable<Race>> outputGenerator)
    {
        string line1 = loader.GetNextLine()!;
        string line2 = loader.GetNextLine()!;

        line1 = TimeLineRegex.Match(line1).Groups["times"].Value;
        line2 = DistLineRegex.Match(line2).Groups["distances"].Value;

        Match m1 = LineNumberRegex.Match(line1);
        Match m2 = LineNumberRegex.Match(line2);

        I result = initialResult;

        while (m1.Success && m2.Success)
        {
            result = timeDistanceProcessor.Invoke(
                result,
                Convert.ToInt32(m1.Groups["number"].Value),
                Convert.ToInt32(m2.Groups["number"].Value)
            );

            m1 = m1.NextMatch();
            m2 = m2.NextMatch();
        }

        return outputGenerator.Invoke(result);
    }
}

public record class Race(long limit, long record) { }