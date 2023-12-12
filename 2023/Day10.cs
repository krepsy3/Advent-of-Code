using Common;

namespace AoC2023.Day10;

using tile = (int x, int y);

enum TileType { None = 0, LoopBlock, Grouped, Enclosed, Out }

public class AdvTask : AdventTask
{
    public override void DoTask(InputLoader loader)
    {
        var pipeline = Pipeline.FromLines(ReadInput(loader));
        loader.EndLoading();

        var task2map = Task1(pipeline);
        
        Task2(task2map, pipeline);
    }


    TileType[,] Task1(Pipeline pipeline)
    {
        int[,] lengths = new int[pipeline.pipeMap.GetLength(0), pipeline.pipeMap.GetLength(1)];
        TileType[,] result  = new TileType[pipeline.pipeMap.GetLength(0), pipeline.pipeMap.GetLength(1)];

        (int sx, int sy) = pipeline.startTile;
        var initDir = pipeline.pipeMap[sx, sy];

        result[sx, sy] = TileType.LoopBlock;

        var steps = new List<(int, int, int, Pipeline.Direction)>();
        foreach (var cd in Pipeline.CardinalDirections)
        {
            if ((initDir & cd) > 0)
            {
                steps.Add((sx, sy, 0, cd));
            }
        }

        var targets = new List<tile>();

        do
        {
            for (int i = 0; i < steps.Count; i++)
            {
                (int x, int y, int last, Pipeline.Direction dir) = steps[i];
                (int nx, int ny) = dir switch
                {
                    Pipeline.Direction.Up => (x, y - 1),
                    Pipeline.Direction.Down => (x, y + 1),
                    Pipeline.Direction.Left => (x - 1, y),
                    Pipeline.Direction.Right => (x + 1, y),
                    _ => (x, y)
                };

                dir = ProcessField(nx, ny, pipeline, ref lengths, last, dir);
                result[nx, ny] = TileType.LoopBlock;

                last++;

                if (dir == 0)
                {
                    steps.RemoveAt(i);
                    i--;
                    targets.Add((x, y));
                }

                else
                {
                    steps[i] = (nx, ny, last, dir);
                }
            }
        }

        while (steps.Count > 0);

        Console.WriteLine(targets.Max((t) => lengths[t.x, t.y]));
        return result;
    }

    Pipeline.Direction ProcessField(int x, int y, Pipeline pipeline, ref int[,] lengths, int last, Pipeline.Direction from)
    {
        if (lengths[x, y] != 0)
        {
            return 0;
        }

        lengths[x, y] = last + 1;
        var fromReturn = Pipeline.Opposite(from);
        var exits = pipeline.pipeMap[x, y];

        foreach (var cd in Pipeline.CardinalDirections)
        {
            if (exits.HasFlag(cd) && cd != fromReturn)
            {
                return cd;
            }
        }

        return 0;
    }


    void Task2(TileType[,] map, Pipeline pipeline)
    {
        var tileGroups = new List<List<tile>>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == TileType.None)
                {
                    tileGroups.Add(GroupNeighbours(ref map, x, y));
                }
            }
        }

        foreach (var group in tileGroups)
        {

        }
    }

    List<tile> GroupNeighbours(ref TileType[,] map, int x, int y)
    {
        var result = new List<tile>();
        var tileQueue = new Queue<tile>();
        tileQueue.Enqueue((x, y));

        while (tileQueue.Count > 0)
        {
            (int tx, int ty) = tileQueue.Dequeue();
            if (map[tx, ty] != TileType.None)
            {
                continue;
            }

            map[tx, ty] = TileType.Grouped;
            result.Add((tx, ty));
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if ((dx == 0) ^ (dy == 0))
                    {
                        int nx = tx + dx;
                        int ny = ty + dy;

                        if ((nx >= 0) && (nx < map.GetLength(0)) && (ny >= 0) && (ny < map.GetLength(1)))
                        {
                            tileQueue.Enqueue((nx, ny));
                        }
                    }
                }
            }
        }

        return result;
    }

    tile GetTileClosestToEdge(List<tile> tiles, int width, int height)
    {
        var possibleResults = new (tile c, int r, bool s)[4];
        possibleResults[0] = (tiles.MinBy(t => t.x), 0, true);
        possibleResults[1] = (tiles.MinBy(t => t.y), 0, false);
        possibleResults[2] = (tiles.MaxBy(t => t.x), width, true);
        possibleResults[3] = (tiles.MaxBy(t => t.y), height, false);
        return possibleResults.MinBy(tr => tr.s ? tr.c.x : tr.c.y - tr.r).c;
    }


    IEnumerable<string> ReadInput(InputLoader loader)
    {
        string? line;
        while ((line = loader.GetNextLine()) is not null)
        {
            yield return line;
        }
    }
}

public class Pipeline
{
    [Flags]
    public enum Direction { Up = 1, Down = 2, Left = 4, Right = 8, DVERT = 3, HYPHEN = 12, J = 5, L = 9, SEVEN = 6, F = 10 }

    public static readonly Direction[] CardinalDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    public static Direction Opposite(Direction direction)
        => direction switch
        {
            Direction.Up    => Direction.Down,
            Direction.Down  => Direction.Up,
            Direction.Left  => Direction.Right,
            Direction.Right => Direction.Left,
            _               => 0
        };

    public Direction[,] pipeMap;
    public tile startTile;

    public Pipeline()
    {
        startTile = (-1, -1);
        pipeMap = new Direction[0, 0];
    }


    public static Pipeline FromLines(IEnumerable<string> lines)
    {
        Pipeline pipeline = new Pipeline();

        var e = lines.GetEnumerator();
        e.MoveNext();
        var line = e.Current;

        int width = line.Length;
        var pipeMap = new List<Direction[]>();

        do
        {
            line = e.Current;
            var row = new Direction[width];

            for (int i = 0; i < width; i++)
            {
                row[i] = line[i] switch
                {
                    '|' => Direction.DVERT,
                    '-' => Direction.HYPHEN,
                    'J' => Direction.J,
                    'L' => Direction.L,
                    '7' => Direction.SEVEN,
                    'F' => Direction.F,
                    'S' => pipeline.PutStartTile(i, pipeMap.Count),
                    _ => 0,
                };
            }

            pipeMap.Add(row);
        }
        while (e.MoveNext());


        pipeline.pipeMap = new Direction[width, pipeMap.Count];
        for (int i = 0; i < pipeMap.Count; i++)
        {
            for (int j = 0; j < width; j++)
            {
                pipeline.pipeMap[j, i] = pipeMap[i][j];
            }
        }




        if ((pipeline.startTile.x >= 0) && (pipeline.startTile.y >= 0))
        {
            pipeline.pipeMap[pipeline.startTile.x, pipeline.startTile.y] = pipeline.GetExitableDirections(pipeline.startTile.x, pipeline.startTile.y);
        }

        return pipeline;
    }

    private Direction GetExitableDirections(int x, int y)
    {
        Direction result = 0;

        if ((x >= 1) && pipeMap[x - 1, y].HasFlag(Direction.Right))
            result |= Direction.Left;

        if ((x < (pipeMap.GetLength(0) - 1)) && pipeMap[x + 1, y].HasFlag(Direction.Left))
            result |= Direction.Right;

        if ((y >= 1) && pipeMap[x, y - 1].HasFlag(Direction.Down))
            result |= Direction.Up;

        if ((y < (pipeMap.GetLength(1) - 1)) && pipeMap[x, y + 1].HasFlag(Direction.Up))
            result |= Direction.Down;

        return result;
    }


    private Direction PutStartTile(int x, int y)
    {
        startTile = (x, y);
        return 0;
    }
}
