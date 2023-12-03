using System.Text.RegularExpressions;

namespace Day3;

public class Program
{
    static void Main(string[] args)
    {
        Task1();
        Task2();
    }


    static readonly Regex numRegex = new Regex("[0-9]+");
    static readonly string nonPartChars = ".0123456789";
    static readonly string numChars = "0123456789";


    static void Task(Func<string, string, string, int> processingFunction)
    {
        using var sr = new StreamReader("input.txt");
        string? input = null;

        string prevline = "";
        string nextline = sr.ReadLine()!;
        string emptyline = new string('.', nextline.Length), line = emptyline;

        int result = 0;

        while ((input = sr.ReadLine()) is not null)
        {
            prevline = line;
            line = nextline;
            nextline = input;
            result += processingFunction(line, prevline, nextline);
        }

        prevline = line;
        line = nextline;
        nextline = emptyline;
        result += processingFunction(line, prevline, nextline);

        Console.WriteLine(result);
    }


    static void Task1()
        => Task(ProcessTask1Line);

    static int ProcessTask1Line(string line, string prev, string next)
    {
        int result = 0;
        var matches = numRegex.Matches(line);
        foreach (Match match in matches)
        {
            if (!match.Success)
            {
                continue;
            }

            int
                left  = Math.Max(match.Index - 1, 0),
                right = Math.Min(match.Index + match.Length, line.Length - 1);

            if (IsEnginePart(prev, line, next, left, right))
            {
                result += Convert.ToInt32(match.Value);
            }
        }

        return result;
    }

    static bool IsEnginePart(string topline, string line, string botline, int leftIndex, int rightIndex)
    {
        if (nonPartChars.IndexOf(line[leftIndex]) < 0)
        {
            return true;
        }

        if (nonPartChars.IndexOf(line[rightIndex]) < 0)
        {
            return true;
        }

        if (topline.Substring(leftIndex, rightIndex - leftIndex + 1).Any(c => nonPartChars.IndexOf(c) < 0))
        {
            return true;
        }

        if (botline.Substring(leftIndex, rightIndex - leftIndex + 1).Any(c => nonPartChars.IndexOf(c) < 0))
        {
            return true;
        }

        return false;
    }


    static void Task2()
        => Task(ProcessTask2Line);

    static int ProcessTask2Line(string line, string prev, string next)
    {
        int result = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != '*')
            {
                continue;
            }

            var nums = GetNumbersAround(next, line, prev, i).ToList();
            if (nums.Count == 2)
            {
                result += nums[0] * nums[1];
            }
        }

        return result;
    }

    static IEnumerable<int> GetNumbersAround(string topline, string line, string botline, int index)
    {
        foreach(var n in GetNumbersAroundInLine(topline, index))
        {
            yield return n;
        }

        int leftindex = Math.Max(index - 1, 0);
        if (numChars.IndexOf(line[leftindex]) >= 0)
        {
            yield return GetNumberToLeft(line, leftindex);
        }

        int rightindex = Math.Min(index + 1, line.Length - 1);
        if (numChars.IndexOf(line[rightindex]) >= 0)
        {
            yield return GetNumberToRight(line, rightindex);
        }

        foreach (var n in GetNumbersAroundInLine(botline, index))
        {
            yield return n;
        }
    }

    static IEnumerable<int> GetNumbersAroundInLine(string line, int index)
    {
        int leftindex = Math.Max(index - 1, 0);
        int rightindex = Math.Min(index + 1, line.Length - 1);

        if (numChars.IndexOf(line[index]) < 0)
        {
            if (numChars.IndexOf(line[rightindex]) >= 0)
            {
                yield return GetNumberToRight(line, rightindex);
            }

            if (numChars.IndexOf(line[leftindex]) >= 0)
            {
                yield return GetNumberToLeft(line, leftindex);
            }
        }

        else
        {
            int rightmost;
            for (rightmost = index + 1; rightmost < line.Length; rightmost++)
            {
                if (numChars.IndexOf(line[rightmost]) < 0)
                {
                    rightmost--;
                    break;
                }
            }

            yield return GetNumberToLeft(line, rightmost);
        }
    }

    static int GetNumberToLeft(string text, int rightIndex)
    {
        int result = text[rightIndex] - '0';
        int coeff = 10;

        for (int i = rightIndex - 1; i >= 0; i--)
        {
            if (numChars.IndexOf(text[i]) < 0)
            {
                return result;
            }

            result += (text[i] - '0') * coeff;
            coeff *= 10;
        }
        return result;
    }

    static int GetNumberToRight(string text, int leftIndex)
    {
        int result = text[leftIndex] - '0';
        for (int i = leftIndex + 1; i < text.Length; i++)
        {
            if (numChars.IndexOf(text[i]) < 0)
            {
                return result;
            }

            result = result * 10 + text[i] - '0';
        }
        return result;
    }
}