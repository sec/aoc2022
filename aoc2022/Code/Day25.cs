namespace aoc2022.Code;

internal class Day25 : BaseDay
{
    readonly Dictionary<char, int> _snafu = new()
    {
        { '2', 2 }, { '1', 1 }, { '0', 0 }, { '-', -1 }, { '=', -2 }
    };

    readonly char[] _cycle = new[] { '1', '0', '-', '=' };

    long FromSNAFU(string snafu)
    {
        var deci = 0L;
        for (int i = snafu.Length - 1, j = 0; i >= 0; i--, j++)
        {
            deci += (long) Math.Pow(5, j) * _snafu[snafu[i]];
        }

        return deci;
    }

    string ToSNAFU(long number)
    {
        var sb = new StringBuilder("2");

        while (FromSNAFU(sb.ToString()) < number)
        {
            sb.Append('2');
        }

        for (int i = 0; i < sb.Length; i++)
        {
            for (int j = 0; j < _cycle.Length; j++)
            {
                var old = sb[i];
                sb[i] = _cycle[j];

                var s = sb.ToString();
                var snafu = FromSNAFU(s);

                if (snafu == number)
                {
                    return s;
                }
                else if (snafu < number)
                {
                    sb[i] = old;
                    break;
                }
            }
        }

        return sb.ToString();
    }

    protected override object Part1() => ReadAllLines(true).Sum(FromSNAFU);

    protected override object Part2() => ToSNAFU((long) Part1());
}