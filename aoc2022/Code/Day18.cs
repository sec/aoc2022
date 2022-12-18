namespace aoc2022.Code;

internal class Day18 : BaseDay
{
    record Cube(int X, int Y, int Z);

    HashSet<Cube> Cubes => new HashSet<Cube>(
        ReadAllLinesSplit(",", true)
            .Select(x => x.Select(int.Parse).ToArray())
            .Select(x => new Cube(x[0], x[1], x[2]))
            .ToList()
        );

    static int CountSides(HashSet<Cube> cubes) => cubes.Select(x => 6 - CountConnected(x, cubes)).Sum();

    static int CountConnected(Cube cube, HashSet<Cube> cubes)
    {
        var count = 0;

        foreach (var step in new[] { -1, 1 })
        {
            if (cubes.Any(c => c.X + step == cube.X && c.Y == cube.Y && c.Z == cube.Z))
            {
                count++;
            }
            if (cubes.Any(c => c.X == cube.X && c.Y + step == cube.Y && c.Z == cube.Z))
            {
                count++;
            }
            if (cubes.Any(c => c.X == cube.X && c.Y == cube.Y && c.Z + step == cube.Z))
            {
                count++;
            }
        }

        return count;
    }

    static void Expand(HashSet<Cube> cubes, Cube start)
    {
        var fringe = new Queue<Cube>();
        var visited = new HashSet<Cube>();

        fringe.Enqueue(start);
        visited.Add(start);

        while (fringe.TryDequeue(out var current))
        {
            cubes.Add(current);

            foreach (var next in GetAdj(current))
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    if (!cubes.Contains(next))
                    {
                        cubes.Add(next);
                        fringe.Enqueue(next);
                    }
                }
            }
        }
    }

    static bool WayFrom(Cube start, Cube end, HashSet<Cube> cubes)
    {
        var fringe = new Queue<Cube>();
        var visited = new HashSet<Cube>();

        fringe.Enqueue(start);
        visited.Add(start);

        var maxx = cubes.OrderByDescending(c => c.X).First().X;
        var maxy = cubes.OrderByDescending(c => c.Y).First().Y;
        var maxz = cubes.OrderByDescending(c => c.Z).First().Z;

        while (fringe.TryDequeue(out var current))
        {
            if (current.X < 0 || current.Y < 0 || current.Z < 0)
            {
                continue;
            }
            if (current.X > maxx && current.Y > maxy && current.Z > maxz)
            {
                continue;
            }

            foreach (var next in GetAdj(current))
            {
                if (next == end)
                {
                    return true;
                }
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    if (!cubes.Contains(next))
                    {
                        fringe.Enqueue(next);
                    }
                }
            }
        }

        return false;
    }

    static IEnumerable<Cube> GetAdj(Cube cube)
    {
        foreach (var step in new[] { -1, 1 })
        {
            yield return cube with { X = cube.X + step };
            yield return cube with { Y = cube.Y + step };
            yield return cube with { Z = cube.Z + step };
        }
    }

    protected override object Part1() => CountSides(Cubes);

    protected override object Part2()
    {
        var cubes = Cubes;
        var maxx = cubes.OrderByDescending(c => c.X).First().X;
        var maxy = cubes.OrderByDescending(c => c.Y).First().Y;
        var maxz = cubes.OrderByDescending(c => c.Z).First().Z;
        var offcube = new Cube(maxx + 1, maxy + 1, maxz + 1);

        for (int x = 0; x <= maxx; x++)
        {
            for (int y = 0; y <= maxy; y++)
            {
                for (int z = 0; z <= maxz; z++)
                {
                    var c = new Cube(x, y, z);
                    if (!cubes.Contains(c) && !WayFrom(c, offcube, cubes))
                    {
                        Expand(cubes, c);
                    }
                }
            }
        }
        return CountSides(cubes);
    }
}