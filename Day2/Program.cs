using System.Text.RegularExpressions;

namespace Day2;

public class Program
{
    static void Main(string[] args)
    {
        Task1();
        Task2();
    }

    static void Task1()
    {
        var games = ParseInput();
        int total = 0;

        foreach (Game game in games)
        {
            int
                maxred   = game.sequence.Max(t => t.red),
                maxgreen = game.sequence.Max(t => t.green),
                maxblue  = game.sequence.Max(t => t.blue);

            if ((maxred <= 12) && (maxgreen <= 13) && (maxblue <= 14))
            {
                total += game.id;
            }
        }

        Console.WriteLine(total);
    }
    
    static void Task2()
    {
        var games = ParseInput();
        int total = 0;

        foreach (Game game in games)
        {
            int
                maxred = game.sequence.Max(t => t.red),
                maxgreen = game.sequence.Max(t => t.green),
                maxblue = game.sequence.Max(t => t.blue);

            total += (maxred * maxgreen * maxblue);
        }

        Console.WriteLine(total);
    }
    static List<Game> ParseInput()
    {
        using var sr = new StreamReader("input.txt");
        string? line = null;

        var games = new List<Game>();

        while ((line = sr.ReadLine()) is not null)
        {
            games.Add(Game.FromString(line));
        }

        return games;
    }
}

public record class Game(int id, List<(int red, int green, int blue)> sequence)
{
    static readonly Regex idRegex  = new Regex("Game (?<id>[0-9]+)");
    static readonly Regex seqRegex = new Regex("(?<count>[0-9]+) (?<color>red|green|blue)");

    public static Game FromString(string input)
    {
        var s1 = input.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var s2 = s1[1].Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int id = Convert.ToInt32(idRegex.Match(s1[0]).Groups["id"].Value);
        var sequence = new List<(int red, int green, int blue)>();

        foreach (var sseq in s2)
        {
            var seqparts = sseq.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int red = 0, green = 0, blue = 0;

            foreach (var seqpart in seqparts)
            {
                var m = seqRegex.Match(seqpart);
                var val = Convert.ToInt32(m.Groups["count"].Value);
                switch(m.Groups["color"].Value)
                {
                    case "red":     red = val; break;
                    case "green": green = val; break;
                    case "blue":   blue = val; break;
                }
            }

            sequence.Add((red, green, blue));
        }

        return new Game(id, sequence);
    }
}