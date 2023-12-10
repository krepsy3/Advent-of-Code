using Common;
using System.Text.RegularExpressions;

namespace AoC2023.Day1;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        DoTask(loader, ProcessLine1);
        loader.Reset();
        DoTask(loader, ProcessLine2);
        loader.EndLoading();
    }

    void DoTask(InputLoader loader, Func<string, int> lineProcessor)
    {
        string? line;
        int total = 0;
        while ((line = loader.GetNextLine()) is not null)
        {
            total += lineProcessor.Invoke(line);
        }

        Console.WriteLine(total);
    }


    int ProcessLine1(string line)
    {
        int first = -1, last = 0;
        foreach (char c in line)
        {
            if ("0123456789".IndexOf(c) >= 0)
            {
                var dig = c - '0';
                if (first < 0)
                {
                    first = dig;
                }

                last = dig;
            }
        }
        return first * 10 + last;
    }
        

    static readonly Regex line2Pattern = new Regex("(0|1|2|3|4|5|6|7|8|9|one|two|three|four|five|six|seven|eight|nine)");

    static int MapNumOrWord(string s)
    {
        if (s.Length == 1)
        {
            return int.Parse(s);
        }

        return s switch
        {
            "one"   => 1,
            "two"   => 2,
            "three" => 3,
            "four"  => 4,
            "five"  => 5,
            "six"   => 6,
            "seven" => 7,
            "eight" => 8,
            "nine"  => 9,
            _       => 0
        };
    }

    int ProcessLine2(string line)
    {
        Match matchObj = line2Pattern.Match(line);
        int first = MapNumOrWord(matchObj.Value);
        int last = first;

        while (matchObj.Success)
        {
            last = MapNumOrWord(matchObj.Value);
            matchObj = line2Pattern.Match(line, matchObj.Index + 1);
        }

        return first * 10 + last;
    }
}
