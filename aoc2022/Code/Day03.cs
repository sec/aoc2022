namespace aoc2022.Code;

internal class Day03 : BaseDay
{
    static int GetPriority(char item) => char.IsUpper(item) ? item - 38 : item - 96;

    static List<char> GetCommon(string rucksack)
    {
        var left = rucksack.Substring(0, rucksack.Length / 2);
        var right = rucksack.Substring(rucksack.Length / 2);

        return left.Where(right.Contains).Distinct().ToList();
    }

    protected override object Part1() => ReadAllLines(true).Sum(x => GetCommon(x).Sum(GetPriority));

    protected override object Part2()
    {
        var sum = 0;

        foreach (var rucksacks in ReadAllLines(true).Chunk(3))
        {
            var a = rucksacks[0];
            var b = rucksacks[1];
            var c = rucksacks[2];

            foreach (var item in a)
            {
                if (!b.Contains(item) || !c.Contains(item))
                {
                    continue;
                }

                sum += GetPriority(item);
                break;
            }
        }

        return sum;
    }
}