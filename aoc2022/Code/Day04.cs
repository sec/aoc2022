namespace aoc2022.Code;

internal class Day04 : BaseDay
{
    class Row
    {
        public Pair L { get; init; }
        public Pair R { get; init; }

        public Row(string[] s)
        {
            L = new(s[0]);
            R = new(s[1]);
        }
    }

    class Pair
    {
        public int A { get; init; }
        public int B { get; init; }

        public Pair(string s)
        {
            var d = s.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            A = d[0];
            B = d[1];
        }

        public bool Fully(Pair other) => A <= other.A && B >= other.B;

        public bool Overlap(Pair other)
        {
            for (var i = A; i <= B; i++)
            {
                if (i >= other.A && i <= other.B)
                {
                    return true;
                }
            }

            return false;
        }
    }

    int Solve(Func<Row, bool> check) => ReadAllLinesSplit(",").Select(x => new Row(x)).Count(check);

    protected override object Part1() => Solve(row => row.L.Fully(row.R) || row.R.Fully(row.L));

    protected override object Part2() => Solve(row => row.L.Overlap(row.R));
}