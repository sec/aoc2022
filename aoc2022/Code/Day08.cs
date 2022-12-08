namespace aoc2022.Code;

internal class Day08 : BaseDay
{
    void GetMap(out int[,] map, out int width, out int height)
    {
        var data = ReadAllLines(true);

        width = data.First().Length;
        height = data.Length;
        map = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = data[y][x] - '0';
            }
        }
    }

    protected override object Part1()
    {
        GetMap(out var map, out var width, out var height);

        var cnt = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
                {
                    cnt++;
                    continue;
                }

                var check = true;
                for (var cx = x + 1; cx < width; cx++)
                {
                    if (map[y, cx] >= map[y, x])
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    cnt++;
                    continue;
                }

                check = true;
                for (var cx = 0; cx < x; cx++)
                {
                    if (map[y, cx] >= map[y, x])
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    cnt++;
                    continue;
                }

                check = true;
                for (var cy = y + 1; cy < height; cy++)
                {
                    if (map[cy, x] >= map[y, x])
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    cnt++;
                    continue;
                }

                check = true;
                for (var cy = 0; cy < y; cy++)
                {
                    if (map[cy, x] >= map[y, x])
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    cnt++;
                    continue;
                }
            }
        }

        return cnt;
    }

    protected override object Part2()
    {
        GetMap(out var map, out var width, out var height);

        var max = 0;

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int a = 1, b = 1, c = 1, d = 1;

                for (var cx = x + 1; cx < width - 1 && map[y, cx] < map[y, x]; cx++, a++)
                {
                }

                for (var cx = x - 1; cx > 0 && map[y, cx] < map[y, x]; cx--, b++)
                {
                }

                for (var cy = y + 1; cy < height - 1 && map[cy, x] < map[y, x]; cy++, c++)
                {
                }

                for (var cy = y - 1; cy > 0 && map[cy, x] < map[y, x]; cy--, d++)
                {
                }

                max = Math.Max(max, a * b * c * d);
            }
        }

        return max;
    }
}