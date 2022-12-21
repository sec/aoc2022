namespace aoc2022.Code;

internal class Day21 : BaseDay
{
    class Monkeys
    {
        readonly Dictionary<string, Func<long>> _monkeys = new();
        public string Left { get; }
        public string Right { get; }

        public Monkeys(string[] data, bool changeRoot)
        {
            foreach (var d in data)
            {
                var input = d.Split(" ", 2);
                var name = input[0][..^1];
                var oper = input[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (oper.Length == 1)
                {
                    var number = long.Parse(oper[0]);
                    _monkeys[name] = () => number;
                }
                else
                {
                    Func<long> func;
                    var left = oper[0];
                    var right = oper[2];

                    if (name == "root")
                    {
                        if (changeRoot)
                        {
                            oper[1] = "=";
                        }
                        Left = left;
                        Right = right;
                    };

                    func = oper[1] switch
                    {
                        "*" => () => _monkeys[left]() * _monkeys[right](),
                        "+" => () => _monkeys[left]() + _monkeys[right](),
                        "-" => () => _monkeys[left]() - _monkeys[right](),
                        "/" => () => _monkeys[left]() / _monkeys[right](),
                        "=" => () => _monkeys[left]() == _monkeys[right]() ? 1 : 0,
                        _ => throw new NotImplementedException()
                    };

                    _monkeys[name] = func;
                }
            }
            ArgumentNullException.ThrowIfNullOrEmpty(Left);
            ArgumentNullException.ThrowIfNullOrEmpty(Right);
        }

        public long Yell(string who) => _monkeys[who]();

        public void Human(long i) => _monkeys["humn"] = () => i;
    }

    protected override object Part1() => new Monkeys(ReadAllLines(true), false).Yell("root");

    protected override object Part2()
    {
        var system = new Monkeys(ReadAllLines(true), true);

        var digits = 1;
        var final = system.Yell(system.Right);
        var match = final.ToString();
        var jump = (long) Math.Pow(10, (system.Yell(system.Right) / 1000L).ToString().Length - 1);

        for (var i = 0L; i < long.MaxValue; i += jump)
        {
            system.Human(i);

            var left = system.Yell(system.Left);
            if (left == final)
            {
                return i;
            }

            if (left.ToString($"d{match.Length}")[..digits] == match[..digits])
            {
                digits++;
                if (jump >= 10)
                {
                    jump /= 10;
                }
            }
        }
        throw new InvalidProgramException();
    }
}