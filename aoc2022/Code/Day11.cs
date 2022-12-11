namespace aoc2022.Code;

internal class Day11 : BaseDay
{
    record Throw(int Index, long Value);

    class Monkey
    {
        private readonly Queue<long> _items;
        private readonly Func<long, long> _operation;

        private readonly int _div;
        private readonly int _true;
        private readonly int _false;

        public long Inspected { get; private set; }

        public Monkey(Func<long, long> operation, int div, int trueMonkey, int falseMonkey, long[] items)
        {
            _operation = operation;
            _div = div;
            _true = trueMonkey;
            _false = falseMonkey;

            _items = new Queue<long>(items);
        }

        public void Catch(long item) => _items.Enqueue(item);

        public IEnumerable<Throw> Round(bool relief, long magic)
        {
            while (_items.TryDequeue(out var item))
            {
                Inspected++;

                item = _operation(item);

                if (relief)
                {
                    item /= 3;
                }
                else
                {
                    item %= magic;
                }

                yield return new Throw(item % _div == 0 ? _true : _false, item);
            }
        }
    }

    (List<Monkey>, long) GetMonkeys()
    {
        var monkeys = new List<Monkey>();
        var magic = 1;

        foreach (var chunk in ReadAllLines(true).Chunk(6))
        {
            var items = chunk[1]["  Starting items: ".Length..].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            var div = int.Parse(chunk[3].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
            var trueMonkey = int.Parse(chunk[4].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
            var falseMonkey = int.Parse(chunk[5].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());

            Func<long, long> func;
            if (chunk[2].Contains("old * old"))
            {
                func = item => item * item;
            }
            else
            {
                var multi = chunk[2].Contains('*');
                var operation = int.Parse(chunk[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
                func = item => multi ? item * operation : item + operation;
            }

            monkeys.Add(new Monkey(func, div, trueMonkey, falseMonkey, items));
            magic *= div;
        }
        return (monkeys, magic);
    }

    long Play(int rounds, bool relief)
    {
        var (monkeys, magic) = GetMonkeys();

        for (var i = 0; i < rounds; i++)
        {
            foreach (var monkey in monkeys)
            {
                foreach (var item in monkey.Round(relief, magic))
                {
                    monkeys[item.Index].Catch(item.Value);
                }
            }
        }

        return monkeys.OrderByDescending(x => x.Inspected).Select(x => x.Inspected).Take(2).Aggregate(1L, (a, b) => a * b);
    }

    protected override object Part1() => Play(20, true);

    protected override object Part2() => Play(10000, false);
}