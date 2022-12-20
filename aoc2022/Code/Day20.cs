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

                var index = list.IndexOf(item);
                var oldIndex = index;

                var howMuch = item.Number % (list.Count - 1);
                var sign = Math.Sign(howMuch);

                howMuch = Math.Abs(howMuch);
                while (howMuch-- > 0)
                {
                    if (sign == 1)
                    {
                        if (index >= list.Count - 1)
                        {
                            index = 0;
                        }
                        index++;
                    }
                    else
                    {
                        if (index <= 0)
                        {
                            index = list.Count - 1;
                        }
                        index--;
                    }
                }

                list.RemoveAt(oldIndex);
                list.Insert(index, item);
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