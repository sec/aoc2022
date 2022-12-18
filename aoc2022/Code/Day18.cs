namespace aoc2022.Code;

internal class Day18 : BaseDay
{
    record Cube(int X, int Y, int Z);

    protected override object Part1()
    {
        var cubes = ReadAllLinesSplit(",", true)
            .Select(x => x.Select(int.Parse).ToArray())
            .Select(x => new Cube(x[0], x[1], x[2]))
            .ToList();

        return cubes.Select(x => 6 - CountConnected(x, cubes)).Sum();
    }

    static int CountConnected(Cube cube, List<Cube> cubes)
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

    protected override object Part2()
    {
        return 0;
    }
}