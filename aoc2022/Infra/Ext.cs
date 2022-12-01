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
}