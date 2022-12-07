namespace aoc2022.Code;

internal class Day07 : BaseDay
{
    record Entry(string Name, bool IsDir, int FileSize, Entry? Parent)
    {
        public List<Entry> Childs { get; set; } = new();

        public int TotalSize => IsDir ? Childs.Sum(x => x.TotalSize) : FileSize;
    }

    Entry Build()
    {
        var root = new Entry("/", true, 0, null);
        var current = root;

        foreach (var cmd in ReadAllLines(true).Where(x => x != "$ ls" && !x.StartsWith("dir")))
        {
            if (cmd == "$ cd /")
            {
                current = root;
            }
            else if (cmd == "$ cd ..")
            {
                if (current.Parent is not null)
                {
                    current = current.Parent;
                }
            }
            else if (cmd.StartsWith("$ cd "))
            {
                var dirName = cmd[5..];

                var entry = new Entry(dirName, true, 0, current);
                current.Childs.Add(entry);
                current = entry;
            }
            else
            {
                var d = cmd.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var name = d[1];
                var size = int.Parse(d[0]);

                var entry = new Entry(name, false, size, current);
                current.Childs.Add(entry);
            }
        }
        return root;
    }

    static IEnumerable<Entry> Walk(Entry root, Func<Entry, bool> check)
    {
        foreach (var c in root.Childs)
        {
            if (c.IsDir && check(c))
            {
                yield return c;
            }
            foreach (var cc in Walk(c, check))
            {
                yield return cc;
            }
        }
    }

    protected override object Part1() => Walk(Build(), dir => dir.TotalSize <= 100_000).Sum(x => x.TotalSize);

    protected override object Part2()
    {
        const int TOTAL = 70_000_000;
        const int NEED = 30_000_000;

        var root = Build();
        var minimum = NEED - (TOTAL - root.TotalSize);

        return Walk(root, dir => dir.TotalSize >= minimum).OrderBy(x => x.TotalSize).First().TotalSize;
    }
}