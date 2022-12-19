namespace aoc2022.Code;

internal class Day19 : BaseDay
{
    sealed class Resources
    {
        public const byte Ore = 0;
        public const byte Clay = 1;
        public const byte Obsidian = 2;
        public const byte Geode = 3;
    }

    record Cost(int Kind, byte Amount);

    class Blueprint
    {
        public int Id { get; private set; }

        public int Max { get; private set; }

        public int QualityLevel => Id * Max;

        public Dictionary<byte, List<Cost>> Costs { get; private set; } = new();

        class State
        {
            readonly byte[] _robots;
            readonly byte[] _units;
            readonly byte _minute;
            readonly Blueprint _blueprint;

            public byte Minute => _minute;
            public byte Geodes => _units[Resources.Geode];

            public State(byte minute, Blueprint blueprint, byte[] robots, byte[] units)
            {
                _blueprint = blueprint;
                _minute = minute;

                _robots = Ext.CopyArray(robots);
                _units = Ext.CopyArray(units);
            }

            bool CanBuild(byte what)
            {
                var needed = _blueprint.Costs[what];
                foreach (var need in needed)
                {
                    if (_units[need.Kind] < need.Amount)
                    {
                        return false;
                    }
                }
                return true;
            }

            void StartBuild(byte what, byte[] units)
            {
                var needed = _blueprint.Costs[what];
                foreach (var need in needed)
                {
                    units[need.Kind] -= need.Amount;
                }
            }

            static void GetMinerals(byte[] units, byte[] robots)
            {
                units[0] += robots[0];
                units[1] += robots[1];
                units[2] += robots[2];
                units[3] += robots[3];
            }

            public IEnumerable<State> NextStates()
            {
                if (CanBuild(Resources.Geode))
                {
                    StartBuild(Resources.Geode, _units);
                    GetMinerals(_units, _robots);
                    _robots[Resources.Geode]++;

                    yield return new State((byte) (_minute + 1), _blueprint, _robots, _units);
                    yield break;
                }

                var units = Ext.CopyArray(_units);
                var robots = Ext.CopyArray(_robots);

                // do nothing, just get minerals
                GetMinerals(units, robots);
                yield return new State((byte) (_minute + 1), _blueprint, robots, units);

                foreach (var what in new[] { Resources.Geode, Resources.Obsidian, Resources.Clay, Resources.Ore })
                {
                    if (CanBuild(what))
                    {
                        if (what != Resources.Geode && _robots[what] > 8)
                        {
                            continue;
                        }

                        units = Ext.CopyArray(_units);
                        robots = Ext.CopyArray(_robots);

                        StartBuild(what, units);
                        GetMinerals(units, robots);
                        robots[what]++;

                        yield return new State((byte) (_minute + 1), _blueprint, robots, units);

                        if (what == Resources.Geode)
                        {
                            yield break;
                        }
                    }
                }
            }

            public long LongId() => BitConverter.ToInt64(new byte[] { _units[0], _units[1], _units[2], _units[3], _robots[0], _robots[1], _robots[2], _robots[3] });
        }

        public Blueprint(byte[] data)
        {
            Id = data[0];

            Costs[Resources.Ore] = new() { new(Resources.Ore, data[1]) };
            Costs[Resources.Clay] = new() { new(Resources.Ore, data[2]) };
            Costs[Resources.Obsidian] = new() { new(Resources.Ore, data[3]), new(Resources.Clay, data[4]) };
            Costs[Resources.Geode] = new() { new(Resources.Ore, data[5]), new(Resources.Obsidian, data[6]) };
        }

        public void Optimize(byte time)
        {
            var fringe = new PriorityQueue<State, byte>();
            var tracker = new Dictionary<byte, byte>();

            var hash = new Dictionary<int, HashSet<long>>();
            for (int i = 0; i <= time; i++)
            {
                hash[i] = new();
            }

            var start = new State(minute: 0, blueprint: this, new byte[] { 1, 0, 0, 0 }, new byte[] { 0, 0, 0, 0 });
            fringe.Enqueue(start, (byte) (255 - start.Geodes));
            hash[start.Minute].Add(start.LongId());

            while (fringe.TryDequeue(out var current, out var _))
            {
                Max = Math.Max(Max, current.Geodes);

                // no point in going, if current have more Geodes, current won't catch up
                if (tracker.TryGetValue(current.Minute, out var best) && best > current.Geodes)
                {
                    continue;
                }
                tracker[current.Minute] = current.Geodes;

                // game over
                if (current.Minute >= time)
                {
                    continue;
                }

                // go deep
                foreach (var next in current.NextStates())
                {
                    var id = next.LongId();
                    var h = hash[next.Minute];

                    if (!h.Contains(id))
                    {
                        h.Add(id);
                        fringe.Enqueue(next, (byte) (255 - next.Geodes));
                    }
                }
            }
        }
    }

    List<Blueprint> GetBlueprints()
    {
        return ReadAllLinesSplit(": ", true)
            .Select(x => x.Where(x => byte.TryParse(x, out var _)))
            .Select(x => x.Select(byte.Parse))
            .Select(x => new Blueprint(x.ToArray()))
            .ToList();
    }

    protected override object Part1()
    {
        var bps = GetBlueprints();
        bps.ForEach(x => x.Optimize(24));

        return bps.Sum(x => x.QualityLevel);
    }

    protected override object Part2()
    {
        //TODO: answer for example is wrong, for real input is correct, go figure :)
        var bps = GetBlueprints().Take(3).ToList();
        bps.ForEach(x => x.Optimize(32));

        return bps.Aggregate(1, (a, b) => a * b.Max);
    }
}