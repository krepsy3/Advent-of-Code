using System.Text.RegularExpressions;

namespace Day1;

class Task2
{
    static void Main(string[] args)
    {
        using StreamReader sr = new StreamReader("input.txt");
        string line = "";

        int total = 0;
        Regex r = new Regex("(0|1|2|3|4|5|6|7|8|9|one|two|three|four|five|six|seven|eight|nine)", RegexOptions.Compiled);
        
        while ((line = sr.ReadLine()!) is not null)
        {
            Match matchObj = r.Match(line);
            int first = Map(matchObj.Value);
            int last = first;

            while (matchObj.Success)
            {
                last = Map(matchObj.Value);
                matchObj = r.Match(line, matchObj.Index + 1);
            }

            total += first * 10 + last;
        }

        Console.WriteLine(total);
    }

    static int Map(string s)
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
            "nine"  => 9
        };
    }
}