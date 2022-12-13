namespace aoc2022.Code;

internal class Day13 : BaseDay
{
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

    record IntItem(int Value) : Item, IComparable
    {
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not IntItem other)
            {
                throw new NotImplementedException();
            }

            return Value.CompareTo(other.Value);
        }
    }

    record ListItem(List<Item> Items) : Item, IComparable
    {
        public int Count => Items.Count;

        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not ListItem other)
            {
                throw new NotImplementedException();
            }

            for (var index = 0; index < Count; index++)
            {
                if (index >= other.Count)
                {
                    return (int) PacketStatus.OutOfOrder;
                }

                var a = Items[index];
                var b = other.Items[index];

                if (a is IntItem aa && b is IntItem bb)
                {
                    var flag = aa.CompareTo(bb);
                    if (flag is not (int) PacketStatus.Equal)
                    {
                        return flag;
                    }
                }
                else if (a is ListItem abc && b is ListItem def)
                {
                    var flag = abc.CompareTo(def);
                    if (flag is not (int) PacketStatus.Equal)
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
                    return p.CompareTo(q);
                }
            }

            return Count == other.Count ? (int) PacketStatus.Equal : (int) PacketStatus.InOrder;
        }
    }

    protected override object Part1()
    {
        var index = 1;

        return ReadAllLines(true)
            .Chunk(2)
            .Select(chunk => new { Index = index++, One = (ListItem) Item.Spawn(chunk[0]), Two = (ListItem) Item.Spawn(chunk[1]) })
            .Where(x => x.One.CompareTo(x.Two) == (int) PacketStatus.InOrder)
            .Sum(x => x.Index);
    }

    protected override object Part2()
    {
        var div2 = new ListItem(new List<Item>() { new IntItem(2) });
        var div6 = new ListItem(new List<Item>() { new IntItem(6) });

        var packets = ReadAllLines(true)
            .Select(x => Item.Spawn(x))
            .Union(new[] { div2, div6 })
            .ToList();

        packets.Sort();

        return (1 + packets.IndexOf(div2)) * (1 + packets.IndexOf(div6));
    }
}