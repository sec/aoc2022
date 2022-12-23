using System.Collections;
using System.Numerics;

namespace aoc2022.Code;

internal class Day23 : BaseDay
{
    const int C = 0;
    const int N = -1;
    const int S = 1;
    const int W = -1;
    const int E = 1;

    record XY(int X, int Y)
    {
        public static XY operator +(XY left, XY right) => new(left.X + right.X, left.Y + right.Y);
    }

    class Elf
    {
        public Elf(int x, int y) => (Coord) = new(x, y);

        public XY Coord { get; private set; }

        public int X => Coord.X;

        public int Y => Coord.Y;

        public void Move(XY p) => Coord = p;
    }

    static IEnumerable<XY> GetAdjacent(XY start)
    {
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (y == 0 && x == 0)
                {
                    continue;
                }

                yield return start + new XY(x, y);
            }
        }
    }

    int MoveOut(bool stopOnNoMove)
    {
        var elfs = ReadAllLines(true).SelectMany((row, i) => row.Select((col, j) => new { i, j, col }).Where(x => x.col == '#').Select(x => new Elf(x.j, x.i))).ToList();

        var considered = new Queue<XY[]>(new[]
        {
            new XY[] { new(C, N), new(W, N), new(E, N) },
            new XY[] { new(C, S), new(W, S), new(E, S) },
            new XY[] { new(W, C), new(W, N), new(W, S) },
            new XY[] { new(E, C), new(E, N), new(E, S) }
        });

        var rounds = stopOnNoMove ? int.MaxValue : 10;
        for (var round = 1; round <= rounds; round++)
        {
            var hash = new HashSet<XY>(elfs.Select(x => x.Coord));
            var propose = new Dictionary<XY, List<Elf>>();

            foreach (var elf in elfs)
            {
                if (!GetAdjacent(elf.Coord).Any(x => hash.Contains(x)))
                {
                    continue;
                }

                foreach (var option in considered)
                {
                    if (option.Select(o => elf.Coord + o).Any(p => hash.Contains(p)))
                    {
                        continue;
                    }

                    var newpos = option[0] + elf.Coord;
                    if (!propose.TryGetValue(newpos, out var list))
                    {
                        propose[newpos] = list = new();
                    }
                    list.Add(elf);

                    break;
                }
            }

            if (stopOnNoMove && !propose.Any(kv => kv.Value.Count > 0))
            {
                return round;
            }

            foreach (var (newpos, list) in propose.Where(kv => kv.Value.Count == 1))
            {
                list[0].Move(newpos);
            }

            considered.Enqueue(considered.Dequeue());
        }

        var minX = elfs.MinBy(e => e.X)!.X;
        var maxX = elfs.MaxBy(e => e.X)!.X;
        var minY = elfs.MinBy(e => e.Y)!.Y;
        var maxY = elfs.MaxBy(e => e.Y)!.Y;

        return Math.Abs(maxX - minX + 1) * Math.Abs(maxY - minY + 1) - elfs.Count;
    }

    protected override object Part1() => MoveOut(false);

    protected override object Part2() => MoveOut(true);
}