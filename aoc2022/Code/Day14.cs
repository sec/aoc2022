namespace aoc2022.Code;

internal class Day14 : BaseDay
{
    record Point(int X, int Y);

    class Sandbox
    {
        readonly HashSet<Point> _sand = new();
        readonly HashSet<Point> _rock = new();
        readonly Point _start;
        readonly int _floorY;

        public Sandbox(string[] lines, Point start)
        {
            _start = start;

            foreach (var line in lines)
            {
                var points = line
                    .Split("->", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => new Point(int.Parse(x[0]), int.Parse(x[1]))).ToList();

                for (var i = 0; i < points.Count - 1; i++)
                {
                    var a = points[i];
                    var b = points[i + 1];

                    for (var y = Math.Min(a.Y, b.Y); y <= Math.Max(a.Y, b.Y); y++)
                    {
                        for (var x = Math.Min(a.X, b.X); x <= Math.Max(a.X, b.X); x++)
                        {
                            _rock.Add(new(x, y));
                        }
                    }
                }
            }
            _floorY = _rock.OrderByDescending(x => x.Y).First().Y + 2;
        }

        bool IsFree(Point where) => !_rock.Contains(where) && !_sand.Contains(where);

        public int Fill(bool checkFloor)
        {
            if (checkFloor)
            {
                var magic = _rock.OrderByDescending(x => x.X).First().X * 2;
                for (int x = -magic; x < magic; x++)
                {
                    _rock.Add(new(x, _floorY));
                }
            }

            while (true)
            {
                if (checkFloor && !IsFree(_start))
                {
                    // Part 2
                    return _sand.Count;
                }

                var sand = _start;
                while (true)
                {
                    var moves = new[] { new Point(0, 1), new Point(-1, 1), new Point(1, 1) };
                    var rest = true;
                    foreach (var move in moves)
                    {
                        var moved = sand with { X = sand.X + move.X, Y = sand.Y + move.Y };

                        if (!checkFloor && !_rock.Any(x => x.Y >= moved.Y) && !_sand.Any(x => x.Y >= moved.Y))
                        {
                            // Part 1
                            return _sand.Count;
                        }

                        if (IsFree(moved))
                        {
                            sand = moved;
                            rest = false;
                            break;
                        }
                    }

                    if (rest)
                    {
                        _sand.Add(sand);
                        break;
                    }
                }
            }
        }
    }

    int Simulate(bool checkFloor) => new Sandbox(ReadAllLines(true), new(500, 0)).Fill(checkFloor);

    protected override object Part1() => Simulate(false);

    protected override object Part2() => Simulate(true);
}