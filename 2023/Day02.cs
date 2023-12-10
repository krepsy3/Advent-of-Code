using Common;
using System.Text.RegularExpressions;

namespace AoC2023.Day02;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        string? line;
        var games = new List<Game>();
        while ((line = loader.GetNextLine()) is not null)
        {
            games.Add(Game.FromString(line));
        }

        loader.EndLoading();

        DoTask(games, ProcessGame1);
        DoTask(games, ProcessGame2);
    }

    void DoTask(List<Game> games, Func<Game, int> gameProcessor)
    {
        int total = 0;

        foreach (Game game in games)
        {
            total += gameProcessor.Invoke(game);
        }

        Console.WriteLine(total);
    }


    int ProcessGame1(Game game)
    {
        int
            maxred = game.sequence.Max(t => t.red),
            maxgreen = game.sequence.Max(t => t.green),
            maxblue = game.sequence.Max(t => t.blue);

        if ((maxred <= 12) && (maxgreen <= 13) && (maxblue <= 14))
        {
            return game.id;
        }

        return 0;
    }


    int ProcessGame2(Game game)
    {
        int
            maxred = game.sequence.Max(t => t.red),
            maxgreen = game.sequence.Max(t => t.green),
            maxblue = game.sequence.Max(t => t.blue);

        return (maxred * maxgreen * maxblue);
    }
}


public record class Game(int id, List<(int red, int green, int blue)> sequence)
{
    static readonly Regex idRegex = new Regex("Game (?<id>[0-9]+)");
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
                switch (m.Groups["color"].Value)
                {
                    case "red": red = val; break;
                    case "green": green = val; break;
                    case "blue": blue = val; break;
                }
            }

            sequence.Add((red, green, blue));
        }

        return new Game(id, sequence);
    }
}
