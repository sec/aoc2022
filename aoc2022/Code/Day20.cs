namespace aoc2022.Code;

internal class Day20 : BaseDay
{
    record Mix(long Number, int Id);

    static void Cycle(List<Mix> list, int times)
    {
        var org = list.ToArray();

        for (int i = 0; i < times; i++)
        {
            foreach (var item in org)
            {
                if (item.Number == 0)
                {
                    continue;
                }

                var oldIndex = list.IndexOf(item);
                var index = (item.Number + oldIndex) % (list.Count - 1);

                if (index <= 0)
                {
                    index = list.Count - 1 + index;
                }

                list.RemoveAt(oldIndex);
                list.Insert((int) index, item);
            }
        }
    }

    static long Solve(List<Mix> list, int times)
    {
        Cycle(list, times);

        var zero = list.IndexOf(list.Single(x => x.Number == 0));

        return new[] { 1000, 2000, 3000 }.Select(x => list[(zero + x) % list.Count].Number).Sum();
    }

    List<Mix> Numbers(long key) => ReadAllLines(true).Select((x, i) => new Mix(long.Parse(x) * key, i)).ToList();

    protected override object Part1() => Solve(Numbers(1), 1);

    protected override object Part2() => Solve(Numbers(811589153L), 10);
}