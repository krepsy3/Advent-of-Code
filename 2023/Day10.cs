using Common;

namespace AoC2023.Day10;

using tile = (int x, int y);
using Dir = Pipeline.Direction;

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

        var steps = new List<(int, int, int, Dir)>();
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
                (int x, int y, int last, Dir dir) = steps[i];
                (int nx, int ny) = dir switch
                {
                    Dir.Up => (x, y - 1),
                    Dir.Down => (x, y + 1),
                    Dir.Left => (x - 1, y),
                    Dir.Right => (x + 1, y),
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

    Dir ProcessField(int x, int y, Pipeline pipeline, ref int[,] lengths, int last, Dir from)
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
            tile target = GetTileClosestToEdge(group, map.GetLength(0), map.GetLength(1));
            var filler = FindOddEvenFillState(target, map, pipeline.pipeMap) ? TileType.Enclosed : TileType.Out;
          
            foreach (var t in group)
            {
                map[t.x, t.y] = filler;
            }
        }

        int total = 0;

        for (int y = 0; y < map.GetLength(1) ;y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                char c = map[x, y] switch
                {
                    TileType.Enclosed => '■',
                    TileType.Out => ' ',
                    _ => 'X',
                };

                if (map[x, y] == TileType.Enclosed)
                {
                    total++;
                }

                if (map[x, y] == TileType.LoopBlock)
                {
                    c = pipeline.pipeMap[x, y] switch
                    {
                        Dir.HYPHEN => '─',
                        Dir.DVERT => '│',
                        Dir.F => '┌',
                        Dir.SEVEN => '┐',
                        Dir.L => '└',
                        Dir.J => '┘',
                    };
                }

                Console.Write(c);
            }

            Console.WriteLine();
        }

        Console.WriteLine(total);
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

    bool FindOddEvenFillState(tile target, TileType[,] map, Dir[,] pipemap)
    {
        Func<tile, int> selector;
        Func<tile, tile> advancer;
        (Dir d, bool seen) cross1, cross2;
        bool state = false;
        tile curr;

        if (target.x <= target.y)
        {
            selector = t => t.x;
            advancer = t => (t.x + 1, t.y);
            cross1 = (Dir.Up, false);
            cross2 = (Dir.Down, false);
            curr = (0, target.y);
        }

        else
        {
            selector = t => t.y;
            advancer = t => (t.x, t.y + 1);
            cross1 = (Dir.Left, false);
            cross2 = (Dir.Right, false);
            curr = (target.x, 0);
        }

        while (selector(curr) < selector(target))
        {
            if (map[curr.x, curr.y] == TileType.LoopBlock)
            {
                Dir block = pipemap[curr.x, curr.y];
                if (block.HasFlag(cross1.d))
                {
                    cross1.seen = !cross1.seen;
                }

                if (block.HasFlag(cross2.d))
                {
                    cross2.seen = !cross2.seen;
                }

                if (cross1.seen && cross2.seen)
                {
                    state = !state;
                    cross1.seen = false;
                    cross2.seen = false;
                }
            }

            curr = advancer(curr);
        }

        return state;
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
