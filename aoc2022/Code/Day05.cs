namespace aoc2022.Code;

internal class Day05 : BaseDay
{
    class Crane
    {
        readonly LinkedList<char> _cargo = new();

        public char Unload()
        {
            var c = _cargo.Last?.Value ?? throw new InvalidDataException();
            _cargo.RemoveLast();

            return c;
        }

        public void Load(char c) => _cargo.AddLast(c);

        public void Init(char c) => _cargo.AddFirst(c);
    }

    static void Move(Crane[] cranes, string[] data, bool isCrateMover9001)
    {
        var cnt = int.Parse(data[1]);
        var src = int.Parse(data[3]) - 1;
        var dst = int.Parse(data[5]) - 1;

        var tmp = new List<char>();

        for (var i = 0; i < cnt; i++)
        {
            tmp.Add(cranes[src].Unload());
        }

        if (isCrateMover9001)
        {
            tmp.Reverse();
        }

        tmp.ForEach(x => cranes[dst].Load(x));
    }

    string Operate(bool isCrateMover9001)
    {
        var cranes = new Crane[9];
        foreach (var line in ReadAllLines(true))
        {
            if (line.Contains('['))
            {
                for (int i = 1, j = 0; i < line.Length; i += 4, j++)
                {
                    if (char.IsLetter(line[i]))
                    {
                        (cranes[j] ??= new()).Init(line[i]);
                    }
                }
            }
            else if (line.StartsWith("move"))
            {
                Move(cranes, line.Split(' ', StringSplitOptions.None), isCrateMover9001);
            }
        }

        return string.Join(string.Empty, cranes.Where(x => x != null).Select(x => x.Unload()));
    }

    protected override object Part1() => Operate(false);

    protected override object Part2() => Operate(true);
}