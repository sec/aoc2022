namespace aoc2022.Code;

internal partial class Day15 : BaseDay
{
    [GeneratedRegex("x=(-?\\d+), y=(-?\\d+)")]
    private static partial Regex ExtractXY();

    record Point(int X, int Y)
    {
        public int ManhattanDistance(Point other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    record Sensor
    {
        public Point Location { get; init; }
        public Point Beacon { get; init; }
        public int Distance { get; init; }

        public Sensor(int x, int y, int bx, int by)
        {
            Location = new(x, y);
            Beacon = new(bx, by);
            Distance = Location.ManhattanDistance(Beacon);
        }
    }

    List<Sensor> ParseInput(out int magic)
    {
        magic = 0;
        var regexp = ExtractXY();
        var sensors = new List<Sensor>();

        foreach (var line in ReadAllLinesSplit(":"))
        {
            var s = regexp.Match(line[0]);
            var b = regexp.Match(line[1]);

            var ns = new Point(int.Parse(s.Groups[1].Value), int.Parse(s.Groups[2].Value));
            var nb = new Point(int.Parse(b.Groups[1].Value), int.Parse(b.Groups[2].Value));

            var sensor = new Sensor(ns.X, ns.Y, nb.X, nb.Y);
            sensors.Add(sensor);

            magic = Math.Max(Math.Abs(ns.X), magic);
            magic = Math.Max(Math.Abs(ns.Y), magic);
            magic = Math.Max(Math.Abs(nb.X), magic);
            magic = Math.Max(Math.Abs(nb.Y), magic);
        }

        magic *= 2;

        return sensors;
    }

    int CountPart1(int y)
    {
        var result = 0;
        var sensors = ParseInput(out var magic);

        Parallel.For(-magic, magic, x =>
        {
            var p = new Point(x, y);

            foreach (var sensor in sensors)
            {
                if (p != sensor.Location && p != sensor.Beacon && p.ManhattanDistance(sensor.Location) <= sensor.Distance)
                {
                    Interlocked.Increment(ref result);
                    break;
                }
            }
        });

        return result;
    }

    long CountPart2(int magic)
    {
        var result = 0L;
        var sensors = ParseInput(out var _);

        Parallel.For(0, magic, (y, loop) =>
        {
            for (var x = 0; x < magic; x++)
            {
                var p = new Point(x, y);

                var flag = true;
                foreach (var sensor in sensors)
                {
                    if (p == sensor.Location || p == sensor.Beacon || p.ManhattanDistance(sensor.Location) <= sensor.Distance)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    result = x * 4_000_000L + y;

                    loop.Stop();
                }
            }
        });

        return result;
    }

    protected override object Part1() => CountPart1(_testRun ? 10 : 2_000_000);

    protected override object Part2() => CountPart2(_testRun ? 20 : 4_000_000);
}