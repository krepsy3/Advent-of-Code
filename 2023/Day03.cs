using Common;
using System.Text.RegularExpressions;

namespace AoC2023.Day03;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        DoTask(loader, ProcessLine1);
        loader.Reset();
        DoTask(loader, ProcessLine2);
        loader.EndLoading();
    }

    static readonly Regex numRegex = new Regex("[0-9]+");
    static readonly string nonPartChars = ".0123456789";
    static readonly string numChars = "0123456789";

    void DoTask(InputLoader loader, Func<string, string, string, int> lineProcessor)
    {
        string? input;
        string prevline;
        string nextline = loader.GetNextLine()!;
        string emptyline = new string('.', nextline.Length);
        string line = emptyline;

        int result = 0;

        while ((input = loader.GetNextLine()) is not null)
        {
            prevline = line;
            line = nextline;
            nextline = input;
            result += lineProcessor(line, prevline, nextline);
        }

        prevline = line;
        line = nextline;
        nextline = emptyline;
        result += lineProcessor(line, prevline, nextline);

        Console.WriteLine(result);
    }


    int ProcessLine1(string line, string prev, string next)
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
                left = Math.Max(match.Index - 1, 0),
                right = Math.Min(match.Index + match.Length, line.Length - 1);

            if (IsEnginePart(prev, line, next, left, right))
            {
                result += Convert.ToInt32(match.Value);
            }
        }

        return result;
    }

    bool IsEnginePart(string topline, string line, string botline, int leftIndex, int rightIndex)
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


    int ProcessLine2(string line, string prev, string next)
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

    IEnumerable<int> GetNumbersAround(string topline, string line, string botline, int index)
    {
        foreach (var n in GetNumbersAroundInLine(topline, index))
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

    IEnumerable<int> GetNumbersAroundInLine(string line, int index)
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

    int GetNumberToLeft(string text, int rightIndex)
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

    int GetNumberToRight(string text, int leftIndex)
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
