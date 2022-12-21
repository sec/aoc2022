namespace aoc2022.Code;

internal class Day21 : BaseDay
{
    class Monkeys
    {
        Dictionary<string, Func<long>> _monkeys = new();

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

                    if (name == "root" && changeRoot)
                    {
                        oper[1] = "=";
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
        }

        public long Yell(string who) => _monkeys[who]();

        public void Human(long i) => _monkeys["humn"] = () => i;
    }

    protected override object Part1()
    {
        var system = new Monkeys(ReadAllLines(true), false);

        return system.Yell("root");
    }

    protected override object Part2()
    {
        long magic = 0;

        Parallel.For(0L, long.MaxValue, (i, s) =>
        {
            var system = new Monkeys(ReadAllLines(true), true);
            system.Human(i);
            if (system.Yell("root") == 1)
            {
                magic = i;
                s.Stop();
            }
        });

        return magic;
    }
}
