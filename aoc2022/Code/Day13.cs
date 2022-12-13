using System.Text.Json;

namespace aoc2022.Code;

internal class Day13 : BaseDay
{
    enum PacketStatus
    {
        InOrder = -1,
        OutOfOrder = 1,
        Equal = 0
    }

    const string DIV_2 = "[[2]]";
    const string DIV_6 = "[[6]]";

    static PacketStatus Compare(List<JsonElement> left, List<JsonElement> right)
    {
        for (var index = 0; index < left.Count; index++)
        {
            if (index >= right.Count)
            {
                return PacketStatus.OutOfOrder;
            }

            var a = left[index];
            var b = right[index];

            if (a.ValueKind == JsonValueKind.Number && b.ValueKind == a.ValueKind)
            {
                // both numbers
                var comp = a.GetInt32().CompareTo(b.GetInt32());
                if (comp != 0)
                {
                    return (PacketStatus) comp;
                }
            }
            else if (a.ValueKind == JsonValueKind.Array && b.ValueKind == a.ValueKind)
            {
                // both array's
                var abc = a.EnumerateArray().ToList();
                var def = b.EnumerateArray().ToList();

                var flag = Compare(abc, def);
                if (flag != PacketStatus.Equal)
                {
                    return flag;
                }
            }
            else
            {
                // one array, one number
                var abc = JsonElementToList(a);
                var def = JsonElementToList(b);

                // can we do return Compare(abc, def); in here?
                var flag = Compare(abc, def);
                if (flag != PacketStatus.Equal)
                {
                    return flag;
                }
            }
        }

        return left.Count == right.Count ? PacketStatus.Equal : PacketStatus.InOrder;

        static List<JsonElement> JsonElementToList(JsonElement src) => src.ValueKind == JsonValueKind.Array ? src.EnumerateArray().ToList() : new List<JsonElement> { src };
    }

    class PacketComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            var left = JsonSerializer.Deserialize<List<JsonElement>>(x ?? string.Empty);
            var right = JsonSerializer.Deserialize<List<JsonElement>>(y ?? string.Empty);

            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return (int) Day13.Compare(left, right);
        }
    }

    protected override object Part1()
    {
        var index = 1;
        var comp = new PacketComparer();

        return ReadAllLines(true)
            .Chunk(2)
            .Select(chunk => new { Index = index++, Status = comp.Compare(chunk[0], chunk[1]) })
            .Where(x => x.Status == (int) PacketStatus.InOrder)
            .Sum(x => x.Index);
    }

    protected override object Part2()
    {
        var packets = ReadAllLines(true).Union(new[] { DIV_2, DIV_6 }).ToList();
        packets.Sort(new PacketComparer());

        return (1 + packets.IndexOf(DIV_2)) * (1 + packets.IndexOf(DIV_6));
    }
}