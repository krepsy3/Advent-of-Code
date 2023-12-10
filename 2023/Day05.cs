using Common;

namespace AoC2023.Day05;
public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        Console.WriteLine("TODO...");
    }
}

/*
public class Program
{
    static void Main(string[] args)
    {
        Almanach input = ParseInput();

        //Task1(input);
        //Task2(input);
        Task2_1(input);
    }

    static void Task1(Almanach almanach)
    {
        long result = long.MaxValue;

        foreach (var seed in almanach.Seeds)
        {
            var loc = SeedToLocation(almanach, seed);

            if (loc < result)
            {
                result = loc;
            }
        }

        Console.WriteLine(result);
    }

    static void Task2(Almanach almanach)
    {
        long result = long.MaxValue;
        long totalSeeds = 0;

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < almanach.Seeds.Count; i += 2)
        {
            Console.WriteLine("Processing seed batch - from {0:### ### ### ### ###}, count {1:### ### ### ### ###}.", almanach.Seeds[i], almanach.Seeds[i + 1]);

            for (long seed = almanach.Seeds[i]; seed < almanach.Seeds[i] + almanach.Seeds[i + 1]; seed++)
            {
                var loc = SeedToLocation(almanach, seed);

                if (loc < result)
                {
                    result = loc;
                }

                totalSeeds++;

                if (totalSeeds % 10_000_000 == 0)
                {
                    Console.WriteLine("Another 10 million seeds processed (total {0:### ### ### ### ###})  ->  {1:mm\\:ss\\.fff}.", totalSeeds, sw.Elapsed);
                }
            }
        }

        Console.WriteLine(result);
    }

    static void Task2_1(Almanach almanach)
    {
        long result = long.MaxValue;

        List<longrange> seeds = new List<longrange>();
        for (int i = 0; i < almanach.Seeds.Count; i += 2)
        {
            seeds.Add((almanach.Seeds[i], almanach.Seeds[i + 1]));
        }

        seeds.Sort((r1, r2) => r1.begin.CompareTo(r2.begin));



        Console.WriteLine(result);
    }

    static Almanach ParseInput()
    {
        return Almanach.FromLines(GetInput().GetEnumerator());
    }

    static IEnumerable<string> GetInput()
    {
        using var sr = new StreamReader("input.txt");
        string? input = null;

        while ((input = sr.ReadLine()) is not null)
        {
            yield return input;
        }
    }

    static long SeedToLocation(Almanach a, long seed)
    {
        var loc = a.Se_So.Conv(seed);
        loc = a.So_F.Conv(loc);
        loc = a.F_W.Conv(loc);
        loc = a.W_Li.Conv(loc);
        loc = a.Li_T.Conv(loc);
        loc = a.T_H.Conv(loc);
        loc = a.H_Lo.Conv(loc);
        return loc;
    }
}

public record class Almanach(List<long> Seeds, Conversion Se_So, Conversion So_F, Conversion F_W, Conversion W_Li, Conversion Li_T, Conversion T_H, Conversion H_Lo)
{
    public static readonly Regex SeedLine = new Regex("seeds:(?<seeds>( [0-9]+)+)");
    public static readonly Regex PrologLine = new Regex("(?<type>[a-zA-Z\\-]+) map:");

    public static Almanach FromLines(IEnumerator<string> lines)
    {
        Match m;
        do
        {
            lines.MoveNext();
            m = SeedLine.Match(lines.Current);
        }

        while (!m.Success);
        List<long> seeds = m.Groups["seeds"].Value
            .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Convert.ToInt64(s))
            .ToList();


        Conversion? seso = null, sof = null, fw = null, wli = null, lit = null, th = null, hlo = null;

        bool goon = true;
        while (goon)
        {
            do
            {
                goon = lines.MoveNext();
                m = PrologLine.Match(lines.Current);
            }

            while (goon && !m.Success);

            if (!goon) break;

            switch (m.Groups["type"].Value)
            {
                case "seed-to-soil":
                    seso = Conversion.FromLines(lines);
                    break;
                case "soil-to-fertilizer":
                    sof = Conversion.FromLines(lines);
                    break;
                case "fertilizer-to-water":
                    fw = Conversion.FromLines(lines);
                    break;
                case "water-to-light":
                    wli = Conversion.FromLines(lines);
                    break;
                case "light-to-temperature":
                    lit = Conversion.FromLines(lines);
                    break;
                case "temperature-to-humidity":
                    th = Conversion.FromLines(lines);
                    break;
                case "humidity-to-location":
                    hlo = Conversion.FromLines(lines);
                    break;
                default:
                    goon = false;
                    break;
            }
        }

        return new Almanach(seeds, seso, sof, fw, wli, lit, th, hlo);
    }
}

public class Conversion
{
    public List<(long source, long destination, long width)> Ranges { get; protected set; } = new List<(long source, long destination, long width)>();

    public static readonly Regex LineRegex = new Regex(" *(?<dest>[0-9]+) *(?<source>[0-9]+) *(?<width>[0-9]+) *");

    public static Conversion FromLines(IEnumerator<string> lines)
    {
        Conversion result = new Conversion();
        lines.MoveNext();
        var m = LineRegex.Match(lines.Current);

        bool nonEnd = true;
        while (m.Success && nonEnd)
        {
            result.Ranges.Add(new(
                Convert.ToInt64(m.Groups["source"].Value),
                Convert.ToInt64(m.Groups["dest"].Value),
                Convert.ToInt64(m.Groups["width"].Value)
            ));

            nonEnd = lines.MoveNext();
            m = LineRegex.Match(lines.Current);
        }

        result.Ranges.Sort((r1, r2) => r1.source.CompareTo(r2.source));
        return result;
    }

    public long Conv(long num)
    {
        foreach (var range in Ranges)
        {
            if ((num >= range.source) && (num < (range.source + range.width)))
            {
                return range.destination + num - range.source;
            }
        }

        return num;
    }

    public List<longrange> Conv(IList<longrange> sortedValues)
    {
        var result = new List<longrange>();
        foreach (var value in sortedValues)
        {

        }

        return result;
    }
}
*/