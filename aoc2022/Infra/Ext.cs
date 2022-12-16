namespace aoc2022.Infra;

internal static class Ext
{
    public static IEnumerable<IEnumerable<string>> InGroups(this IEnumerable<string> source)
    {
        var tmp = new List<string>();
        foreach (var line in source)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield return tmp;

                tmp.Clear();
            }
            else
            {
                tmp.Add(line);
            }
        }
        if (tmp.Count > 0)
        {
            yield return tmp;
        }
    }

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
    {
        if (length == 1)
        {
            return list.Select(t => new T[] { t });
        }

        return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    internal static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> items, int count)
    {
        int i = 0;
        foreach (var item in items)
        {
            if (count == 1)
            {
                yield return new T[] { item };
            }
            else
            {
                foreach (var result in GetCombinations(items.Skip(i + 1), count - 1))
                {
                    yield return new T[] { item }.Concat(result);
                }
            }
            ++i;
        }
    }
}