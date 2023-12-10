using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023.Day09;

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        var sequences = ParseInput(loader);
        loader.EndLoading();

        DoTask(sequences, ExtrapolateSequence);
        DoTask(sequences, ExtrapolateSequenceFront);
    }

    void DoTask(List<int[]> sequences, Func<int[], int> extrapolator)
    {
        long result = 0;
        foreach (var sequence in sequences)
        {
            result += extrapolator.Invoke(sequence);
        }

        Console.WriteLine(result);
    }

    int ExtrapolateSequence(int[] seq)
    {
        List<int[]> seqs = new List<int[]>() { seq };
        while (!seqs[seqs.Count - 1].All(n => n == 0))
        {
            seqs.Add(GetNeigbourDiffs(seqs[seqs.Count - 1]));
        }

        int result = 0;
        for (int i = seqs.Count - 2; i >= 0; i--)
        {
            result += seqs[i][seqs[i].Length - 1];
        }

        return result;
    }

    int ExtrapolateSequenceFront(int[] seq)
    {
        List<int[]> seqs = new List<int[]>() { seq };
        while (!seqs[seqs.Count - 1].All(n => n == 0))
        {
            seqs.Add(GetNeigbourDiffs(seqs[seqs.Count - 1]));
        }

        int result = 0;
        for (int i = seqs.Count - 2; i >= 0; i--)
        {
            result = seqs[i][0] - result;
        }

        return result;
    }

    int[] GetNeigbourDiffs(int[] nums)
    {
        int[] result = new int[nums.Length - 1];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = nums[i + 1] - nums[i];
        }

        return result;
    }


    List<int[]> ParseInput(InputLoader loader)
    {
        string? line;
        List<int[]> result = new List<int[]>();

        while ((line = loader.GetNextLine()) is not null)
        {
            string[] entries = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (entries.Length == 0)
            {
                continue;
            }

            result.Add(entries.Select(s => Convert.ToInt32(s)).ToArray());
        }

        return result;
    }
}