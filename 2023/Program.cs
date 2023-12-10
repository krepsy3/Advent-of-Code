using Common;
using System.Reflection;

namespace AoC2023;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Put the day you'd like to execute as the first argument.");
            return;
        }

        bool parsed = int.TryParse(args[0], out int day);

        if (!parsed)
        {
            Console.WriteLine("Put the day you'd like to execute as the first argument.");
            return;
        }

        var task = GetTask(day);
        if (task is null)
        {
            Console.WriteLine("Day not impemented.");
            return;
        }

        var loader = new InputLoader(day);
        if (loader.BeginLoading())
        {
            task.DoTask(loader);
        }

        else
        {
            Console.WriteLine("Input file for the day was not found!");
        }
    }

    static AdventTask? GetTask(int day)
    {
        var type = typeof(Program).Assembly.GetType($"AoC2023.Day{day}.AdvTask");
        if (type is null)
        {
            return null;
        }

        return Activator.CreateInstance(type) as AdventTask;
    }
}