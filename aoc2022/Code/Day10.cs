namespace aoc2022.Code;

internal class Day10 : BaseDay
{
    static int CPU(int cycle, string[] code)
    {
        var x = 1;
        var counter = 1;
        var index = 0;

        while (counter < cycle)
        {
            var inst = code[index++];
            if (inst == "noop")
            {
                counter++;
            }
            else
            {
                counter++;
                if (counter < cycle)
                {
                    x += int.Parse(inst["add x".Length..]);
                }
                counter++;
            }
        }

        return x;
    }

    protected override object Part1() => new[] { 20, 60, 100, 140, 180, 220 }.Select(x => x * CPU(x, ReadAllLines())).Sum();

    protected override object Part2()
    {
        var code = ReadAllLines();
        var sb = new StringBuilder();
        for (var y = 0; y < 6; y++)
        {
            sb.AppendLine();

            for (var x = 0; x < 40; x++)
            {
                var cycle = (y * 40 + x) + 1;
                var reg = CPU(cycle, code);

                sb.Append(reg - 1 <= x && reg + 1 >= x ? '█' : ' ');
            }
        }
        return sb;
    }
}