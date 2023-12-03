namespace Day1;

class Task1
{
    static void Main(string[] args)
    {
        using StreamReader sr = new StreamReader("input.txt");
        string line = "";

        int total = 0;

        while ((line = sr.ReadLine()!) is not null)
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

            total += first * 10 + last;
        }

        Console.WriteLine(total);
    }
}