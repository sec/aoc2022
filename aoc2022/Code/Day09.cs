namespace aoc2022.Code;

internal class Day09 : BaseDay
{
    record Point(int X, int Y)
    {
        public Point Move(Point p) => new(X + p.X, Y + p.Y);

        public Point Diff(Point o) => new(Math.Abs(X - o.X), Math.Abs(Y - o.Y));

        public bool Plancked => X > 1 || Y > 1;
    }

    record Move(Point Where, int Count);

    readonly Point[] UpDownLeftRight = new[] { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0), };
    readonly Point[] Corners = new[] { new Point(1, 1), new Point(1, -1), new Point(-1, -1), new Point(-1, 1), };

    IEnumerable<Move> Moves => ReadAllLinesSplit(" ", true).Select(x => new Move(x[0] switch
    {
        "U" => UpDownLeftRight[0],
        "D" => UpDownLeftRight[1],
        "L" => UpDownLeftRight[2],
        "R" => UpDownLeftRight[3],
        _ => throw new NotImplementedException()
    }, int.Parse(x[1])));

    int CountTail(int length)
    {
        var visit = new HashSet<Point>();

        var knots = new Point[length];
        for (int i = 0; i < knots.Length; i++)
        {
            knots[i] = new Point(0, 0);
        }

        foreach (var move in Moves)
        {
            for (var i = 0; i < move.Count; i++)
            {
                visit.Add(knots[0]);

                knots[length - 1] = knots[length - 1].Move(move.Where);
                for (int k = length - 2; k >= 0; k--)
                {
                    var head = knots[k + 1];
                    var tail = knots[k];

                    var diff = head.Diff(tail);
                    if (diff.Plancked)
                    {
                        var moves = diff.X == 0 || diff.Y == 0 ? UpDownLeftRight : Corners;
                        foreach (var correct in moves)
                        {
                            var corrected = tail.Move(correct);
                            var newdiff = corrected.Diff(head);

                            if (!(newdiff.Plancked))
                            {
                                knots[k] = corrected;
                                break;
                            }
                        }
                    }
                }
            }
            visit.Add(knots[0]);
        }

        return visit.Count;
    }

    protected override object Part1() => CountTail(2);

    protected override object Part2() => CountTail(10);
}