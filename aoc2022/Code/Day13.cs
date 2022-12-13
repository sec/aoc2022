namespace aoc2022.Code;

internal class Day13 : BaseDay
{
    const string DIV_2 = "[[2]]";
    const string DIV_6 = "[[6]]";

    enum PacketStatus
    {
        InOrder = -1,
        OutOfOrder = 1,
        Equal = 0
    }

    abstract record Item
    {
        public static Item Spawn(ReadOnlySpan<char> input)
        {
            var items = new List<Item>();

            var i = 0;
            while (i < input.Length)
            {
                if (input[i] == '[')
                {
                    i++;

                    var count = 1;
                    var start = i;

                    while (count > 0)
                    {
                        var c = input[i++];
                        if (c == '[')
                        {
                            count++;
                        }
                        else if (c == ']')
                        {
                            count--;
                        }
                    }
                    var list = input[start..(i - 1)];

                    items.Add(Spawn(list));

                    i++; // after ']' there's ','
                }
                else
                {
                    var start = i;
                    while (i < input.Length && char.IsDigit(input[i]))
                    {
                        i++;
                    }
                    var number = input[start..i];
                    var parsed = int.Parse(number);

                    items.Add(new IntItem(parsed));

                    i++; // after ']' there's ','
                }
            }

            return new ListItem(items);
        }
    }

    record IntItem(int Value) : Item;

    record ListItem(List<Item> Items) : Item
    {
        public int Count => Items.Count;

        public Item this[int index] => Items[index];
    }

    class PacketComparer : IComparer<string>
    {
        public static readonly PacketComparer Instance = new();

        public int Compare(string? x, string? y)
        {
            var left = Item.Spawn(x) as ListItem;
            var right = Item.Spawn(y) as ListItem;

            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return (int) Compare(left, right);
        }

        static PacketStatus Compare(ListItem left, ListItem right)
        {
            for (var index = 0; index < left.Count; index++)
            {
                if (index >= right.Count)
                {
                    return PacketStatus.OutOfOrder;
                }

                var a = left[index];
                var b = right[index];

                if (a is IntItem aa && b is IntItem bb)
                {
                    // both numbers
                    var comp = aa.Value.CompareTo(bb.Value);
                    if (comp != 0)
                    {
                        return (PacketStatus) comp;
                    }
                }
                else if (a is ListItem abc && b is ListItem def)
                {
                    // both array's
                    var flag = Compare(abc, def);
                    if (flag != PacketStatus.Equal)
                    {
                        return flag;
                    }
                }
                else
                {
                    if (a is not ListItem p)
                    {
                        p = new ListItem(new List<Item>() { a });
                    }
                    if (b is not ListItem q)
                    {
                        q = new ListItem(new List<Item>() { b });
                    }

                    return Compare(p, q);
                }
            }

            return left.Count == right.Count ? PacketStatus.Equal : PacketStatus.InOrder;
        }
    }

    protected override object Part1()
    {
        var index = 1;

        return ReadAllLines(true)
            .Chunk(2)
            .Select(chunk => new { Index = index++, Status = PacketComparer.Instance.Compare(chunk[0], chunk[1]) })
            .Where(x => x.Status == (int) PacketStatus.InOrder)
            .Sum(x => x.Index);
    }

    protected override object Part2()
    {
        var packets = ReadAllLines(true).Union(new[] { DIV_2, DIV_6 }).ToList();
        packets.Sort(PacketComparer.Instance);

        return (1 + packets.IndexOf(DIV_2)) * (1 + packets.IndexOf(DIV_6));
    }
}