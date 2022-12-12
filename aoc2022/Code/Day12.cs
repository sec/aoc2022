namespace aoc2022.Code;

internal class Day12 : BaseDay
{
    record Point(int X, int Y);

    record Item(int Moves, Point Location);

    static readonly Point[] Moves = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, 1), new Point(0, -1), };

    int FindRoute(bool checkAll)
    {
        var data = ReadAllLines(true);
        var width = data[0].Length;
        var height = data.Length;

        var grid = new int[height, width];

        var end = new Point(0, 0);
        var starting = new List<Point>();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var c = data[y][x];
                if (c == 'S' || c == 'a')
                {
                    grid[y, x] = 'a';
                    if (checkAll || c == 'S')
                    {
                        starting.Add(new Point(x, y));
                    }
                }
                else if (c == 'E')
                {
                    end = new Point(x, y);
                    grid[y, x] = 'z';
                }
                else
                {
                    grid[y, x] = c;
                }
            }
        }

        var min = int.MaxValue;

        foreach (var start in starting)
        {
            var fringe = new Queue<Item>();
            var visited = new HashSet<Point>();

            fringe.Enqueue(new(0, start));
            visited.Add(start);

            while (fringe.TryDequeue(out var current))
            {
                if (current.Location == end)
                {
                    min = Math.Min(min, current.Moves);
                    break;
                }

                foreach (var move in Moves)
                {
                    var newMove = new Point(current.Location.X + move.X, current.Location.Y + move.Y);

                    if (newMove.X < 0 || newMove.X >= width || newMove.Y < 0 || newMove.Y >= height)
                    {
                        continue;
                    }
                    if (visited.Contains(newMove))
                    {
                        continue;
                    }

                    if (grid[newMove.Y, newMove.X] <= 1 + grid[current.Location.Y, current.Location.X])
                    {
                        fringe.Enqueue(new Item(current.Moves + 1, newMove));
                        visited.Add(newMove);
                    }
                }
            }
        }

        return min;
    }

    protected override object Part1() => FindRoute(false);

    protected override object Part2() => FindRoute(true);
}