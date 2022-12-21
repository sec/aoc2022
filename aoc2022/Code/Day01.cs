namespace aoc2022.Code;

internal class Day01 : BaseDay
{
    private IEnumerable<int> Elfs() => ReadAllLines().InGroups().Select(x => x.Select(int.Parse).Sum());

    protected override object Part1() => Elfs().Max();

    protected override object Part2() => Elfs().OrderDescending().Take(3).Sum();
}