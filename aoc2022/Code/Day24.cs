namespace aoc2022.Code;

internal class Day24 : BaseDay
{
    record XY(int X, int Y)
    {
        public static XY operator +(XY left, XY right) => new(left.X + right.X, left.Y + right.Y);

        public override string ToString() => $"{X},{Y}";
    }

    class Blizzard
    {
        public Blizzard(int X, int Y, char w)
        {
            Where = new XY(X, Y);
            Dir = w switch
            {
                '>' => new(1, 0),
                '<' => new(-1, 0),
                '^' => new(0, -1),
                'v' => new(0, 1),
                _ => throw new NotImplementedException()
            };
        }

        public Blizzard(Blizzard other) => (Where, Dir) = (other.Where, other.Dir);

        public XY Where { get; set; }

        public XY Dir { get; set; }

        public void Move(int width, int height)
        {
            var np = Where + Dir;

            if (np.X == 0)
            {
                Where = new(width - 2, np.Y);
            }
            else if (np.X == width - 1)
            {
                Where = new(1, np.Y);
            }
            else if (np.Y == 0)
            {
                Where = new(np.X, height - 2);
            }
            else if (np.Y == height - 1)
            {
                Where = new(np.X, 1);
            }
            else
            {
                Where = np;
            }
        }

        public override string ToString() => $"{Where}|{Dir}";
    }

    (XY Start, XY End, int Width, int Height, List<Blizzard> Storm) GetInput()
    {
        var input = ReadAllLines(true);

        var width = input.First().Length;
        var height = input.Length;

        var storm = input.SelectMany((row, y) => row.Select((c, x) => new { x, y, c }))
            .Where(e => e.c != '#' && e.c != '.')
            .Select(e => new Blizzard(e.x, e.y, e.c))
            .ToList();

        var start = new XY(1, 0);
        var end = new XY(width - 2, height - 1);

        return (start, end, width, height, storm);
    }

    static (int Time, List<Blizzard> Storm) DodgeTheBlizzard(XY start, XY end, int startMinute, List<Blizzard> currentStorm, int width, int height)
    {
        var visited = new HashSet<string>();

        var fringe = new Queue<(int Minute, XY Current, List<Blizzard> Storm)>();
        fringe.Enqueue((startMinute, start, currentStorm));

        while (fringe.TryDequeue(out var data))
        {
            var (minute, current, storm) = data;

            storm.ForEach(x => x.Move(width, height));
            var hash = new HashSet<XY>(storm.Select(x => x.Where));

            foreach (var next in GetNextMoves(current, hash, width, height, end))
            {
                if (next == end)
                {
                    return (minute + 1, storm);
                }
                EnqueueIfNew(minute + 1, next, CopyStorm(storm));
            }

            if (!hash.Contains(current))
            {
                EnqueueIfNew(minute + 1, current, storm);
            }
        }

        void EnqueueIfNew(int _minute, XY _pos, List<Blizzard> _storm)
        {
            var item = (_minute, _pos, _storm);
            var nh = GetHash(item);

            if (!visited.Contains(nh))
            {
                fringe.Enqueue(item);
                visited.Add(nh);
            }
        }

        throw new InvalidDataException();
    }

    static string GetHash((int Minute, XY Current, List<Blizzard> Storm) data) => $"{data.Minute},{data.Current},{string.Join(",", data.Storm)}";

    static List<Blizzard> CopyStorm(List<Blizzard> storm) => storm.Select(x => new Blizzard(x)).ToList();

    static IEnumerable<XY> GetNextMoves(XY current, HashSet<XY> storm, int width, int height, XY end)
    {
        foreach (var move in new XY[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) })
        {
            var np = current + move;
            if (np == end || (np.X > 0 && np.X < width - 1 && np.Y > 0 && np.Y < height - 1))
            {
                if (!storm.Contains(np))
                {
                    yield return np;
                }
            }
        }
    }

    protected override object Part1()
    {
        var (Start, End, Width, Height, Storm) = GetInput();

        return DodgeTheBlizzard(Start, End, 0, Storm, Width, Height).Time;
    }

    protected override object Part2()
    {
        var (Start, End, Width, Height, Storm) = GetInput();
        var time = 0;

        for (int i = 0; i < 3; i++)
        {
            (time, Storm) = DodgeTheBlizzard(Start, End, time, Storm, Width, Height);
            (Start, End) = (End, Start);
        }

        return time;
    }
}