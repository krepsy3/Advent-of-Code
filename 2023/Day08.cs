using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AoC2023.Day08;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        (var path, var nodes) = ParseInput(loader);
        loader.EndLoading();
        DoTask1(path, nodes);
        DoTask2(path, nodes);
    }


    public void DoTask1(BitArray path, Dictionary<int, (int l, int r)> nodes)
    {
        int next = GetNodeIdentifier("AAA");
        int target = GetNodeIdentifier("ZZZ");
        int steps = 0;
        int pathIndex = 0;

        while (next != target)
        {
            bool right = path[pathIndex];
            if (++pathIndex >= path.Length)
            {
                pathIndex = 0;
            }

            next = right ? nodes[next].r : nodes[next].l;
            steps++;
        }

        Console.WriteLine(steps);
    }


    public void DoTask2Naive(BitArray path, Dictionary<int, (int l, int r)> nodes)
    {
        int[] nexts = nodes.Where(n => (n.Key % 26) == 0).Select(n => n.Key).ToArray();
        long steps = 0;
        int pathIndex = 0;

        while (!nexts.All(i => (i % 26) == 25))
        {
            bool right = path[pathIndex];
            if (++pathIndex >= path.Length)
            {
                pathIndex = 0;
            }

            for (int i = 0; i < nexts.Length; i++)
            {
                nexts[i] = right ? nodes[nexts[i]].r : nodes[nexts[i]].l;
            }

            steps++;
        }

        Console.WriteLine(steps);
    }

    public void DoTask2General(BitArray path, Dictionary<int, (int l, int r)> nodes)
    {
        int[] starts = nodes.Where(n => (n.Key % 26) == 0).Select(n => n.Key).ToArray();
        (int init, int[] sequence)[] stepRecords = new (int init, int[] sequence)[starts.Length];

        for (int i = 0; i < starts.Length; i++)
        {
            int pathIndex = 0;
            int next = starts[i];
            int steps = 0;

            while ((next % 26) != 25)
            {
                bool right = path[pathIndex];
                if (++pathIndex >= path.Length)
                {
                    pathIndex = 0;
                }

                next = right ? nodes[next].r : nodes[next].l;
                steps++;
            }

            stepRecords[i].init = steps;
            int loopstart = next;
            int loopstartstep = pathIndex;

            List<int> seq = new List<int>();

            do
            {
                steps = 0;
                do
                {
                    bool right = path[pathIndex];
                    if (++pathIndex >= path.Length)
                    {
                        pathIndex = 0;
                    }

                    next = right ? nodes[next].r : nodes[next].l;
                    steps++;
                }
                while ((next % 26) != 25);
                seq.Add(steps);
            }

            while ((next != loopstart) || (loopstartstep != pathIndex));

            stepRecords[i].sequence = seq.ToArray();
        }

        //TODO
    }


    public void DoTask2(BitArray path, Dictionary<int, (int l, int r)> nodes)
    {
        int[] starts = nodes.Where(n => (n.Key % 26) == 0).Select(n => n.Key).ToArray();
        int[] ssteps = new int[starts.Length];

        for (int i = 0; i < starts.Length; i++)
        {
            int pathIndex = 0;
            int next = starts[i];
            int steps = 0;

            while ((next % 26) != 25)
            {
                bool right = path[pathIndex];
                if (++pathIndex >= path.Length)
                {
                    pathIndex = 0;
                }

                next = right ? nodes[next].r : nodes[next].l;
                steps++;
            }

            ssteps[i] = steps;
        }

        long result = ssteps[0];

        for (int i = 1; i < ssteps.Length; i++)
        {
            result = Euclid.LCM(result, ssteps[i]);
        }

        Console.WriteLine(result);
    }


    static readonly Regex PathLineRegex = new Regex("^[LR]+$");
    static readonly Regex NodeLineRegex = new Regex("(?<idf>[A-Z]{3}) = \\((?<left>[A-Z]{3}), (?<right>[A-Z]{3})\\)");

    (BitArray path, Dictionary<int, (int l, int r)> nodes) ParseInput(InputLoader loader)
    {
        string? line;

        BitArray path = null!;

        while ((line = loader.GetNextLine()) is not null)
        {
            if (!PathLineRegex.IsMatch(line))
            {
                continue;
            }

            path = new BitArray(line.Length);
            for (int i = 0; i < line.Length; i++)
            {
                path[i] = line[i] == 'R';
            }

            break;
        }

        Dictionary<int, (int, int)> nodes = new Dictionary<int, (int, int)>();
        
        while ((line = loader.GetNextLine()) is not null)
        {
            Match m = NodeLineRegex.Match(line);
            if (!m.Success)
            {
                continue;
            }

            int idf   = GetNodeIdentifier(m.Groups["idf"].Value);
            int left  = GetNodeIdentifier(m.Groups["left"].Value);
            int right = GetNodeIdentifier(m.Groups["right"].Value);

            nodes.Add(idf, (left, right));
        }

        return (path, nodes);
    }

    static int GetNodeIdentifier(string s)
    {
        int result = 0;

        for (int i = 0; i < 3; i++)
        {
            result = result * 26 + (s[i] - 'A');
        }

        return result;
    }
}

public static class Euclid
{
    public static long GCD(long a, long b)
    {
        while (a != b)
        {
            if (b > a)
            {
                b -= a;
            }
            else
            {
                a -= b;
            }
        }

        return a;
    }

    public static long LCM(long a, long b)
    {
        var gcd = GCD(a, b);
        return a * b / gcd;
    }
}