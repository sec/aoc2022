namespace aoc2022.Code;

internal class Day06 : BaseDay
{
    static int Decode(string signal, int magicNumber)
    {
        var last = new List<char>();

        for (var i = 0; i < signal.Length; i++)
        {
            last.Add(signal[i]);

            if (last.Skip(1).Distinct().Count() == magicNumber)
            {
                return i + 1;
            }

            if (last.Count > magicNumber)
            {
                last.RemoveAt(0);
            }
        }

        return -1;
    }

    protected override object Part1() => Decode(ReadAllText(), 4);

    protected override object Part2() => Decode(ReadAllText(), 14);
}