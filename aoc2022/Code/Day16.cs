using System.Collections.Concurrent;

namespace aoc2022.Code;

internal class Day16 : BaseDay
{
    static readonly Dictionary<string, List<string>> Paths = new();

    class Valve
    {
        public string Id { get; private set; }
        public int Pressure { get; private set; }
        public int Flow { get; private set; }
        public bool Opened { get; private set; }
        public int OpenedAt { get; private set; }

        public void Open(int time, int maxTime)
        {
            OpenedAt = time;
            Opened = true;
            Pressure = Flow * (maxTime - OpenedAt);
        }

        public Valve(string id, int flow, bool opened)
        {
            Id = id;
            Flow = flow;
            Opened = flow == 0 || opened;
        }

        public Valve(Valve other) : this(other.Id, other.Flow, other.Opened)
        {
            Pressure = other.Pressure;
            OpenedAt = other.OpenedAt;
        }

        public override string ToString() => $"({Id},{(Opened ? "1" : "0")},{OpenedAt})";
    }

    class State
    {
        readonly List<Valve> _valves;
        readonly Valve _player;
        readonly Valve _elephant;

        public State(IEnumerable<Valve> list, string current, string elephant)
        {
            _valves = list.ToList();
            _player = _valves.Single(x => x.Id == current);
            _elephant = _valves.Single(X => X.Id == elephant);
        }

        public IEnumerable<State> GetNextState(int time, bool useElephant)
        {
            var tmp = useElephant ? WalkWithElephant(time) : WalkAlone(time);
            foreach (var t in tmp)
            {
                yield return t;
            }
        }

        public IEnumerable<State> WalkAlone(int time)
        {
            if (!_player.Opened)
            {
                var list = CloneValves(_valves);
                list.Single(x => x.Id == _player.Id).Open(time, 30);

                yield return new State(list, _player.Id, _elephant.Id);
            }

            foreach (var newCurrent in Paths[_player.Id])
            {
                yield return new State(CloneValves(_valves), newCurrent, _elephant.Id);
            }
        }

        public IEnumerable<State> WalkWithElephant(int time)
        {
            if (!_player.Opened && !_elephant.Opened)
            {
                var list = CloneValves(_valves);
                list.Single(x => x.Id == _player.Id).Open(time, 26);
                list.Single(x => x.Id == _elephant.Id).Open(time, 26);

                yield return new State(list, _player.Id, _elephant.Id);
            }

            if (!_player.Opened)
            {
                var list = CloneValves(_valves);
                list.Single(x => x.Id == _player.Id).Open(time, 26);
                foreach (var elephant in Paths[_elephant.Id])
                {
                    yield return new State(CloneValves(list), _player.Id, elephant);
                }
            }

            if (!_elephant.Opened)
            {
                var list = CloneValves(_valves);
                list.Single(x => x.Id == _elephant.Id).Open(time, 26);
                foreach (var player in Paths[_player.Id])
                {
                    yield return new State(CloneValves(list), player, _elephant.Id);
                }
            }

            if (_player.Id == _elephant.Id)
            {
                var paths = Paths[_player.Id].GetCombinations(2).ToList();
                foreach (var path in paths)
                {
                    yield return new State(CloneValves(_valves), path.First(), path.Last());
                }
            }
            else
            {
                foreach (var player in Paths[_player.Id])
                {
                    foreach (var elephant in Paths[_elephant.Id])
                    {
                        if (player != elephant)
                        {
                            yield return new State(CloneValves(_valves), player, elephant);
                        }
                    }
                }
                foreach (var elephant in Paths[_elephant.Id])
                {
                    foreach (var player in Paths[_player.Id])
                    {
                        if (player != elephant)
                        {
                            yield return new State(CloneValves(_valves), player, elephant);
                        }
                    }
                }
            }
        }

        static List<Valve> CloneValves(List<Valve> src) => src.Select(x => new Valve(x)).ToList();

        public bool NoMoreMoves => _valves.All(x => x.Opened);

        public int Pressure => _valves.Sum(x => x.Pressure);

        public override string ToString() => _player.Id + string.Join(",", _valves) + _elephant.Id;
    }

    (List<State>, HashSet<string>) GetInput()
    {
        var list = ReadAllLinesSplit(";", true).Select(line =>
        {
            var left = line[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var id = left[1];
            var flow = int.Parse(left[4]["rate=".Length..]);

            var index = 0;
            for (int i = 0; i < line[1].Length; i++)
            {
                if (char.IsUpper(line[1][i]))
                {
                    index = i;
                    break;
                }
            }

            var paths = line[1][index..].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            Paths[id] = paths.ToList();

            return new Valve(id, flow, "id" == "AA");
        });

        var hash = new HashSet<string>();

        List<State> states = new()
        {
            new State(list, "AA", "AA")
        };
        hash.Add(states.Single().ToString());

        return (states, hash);
    }

    int ReleaseTheValves(bool useElephant)
    {
        var (states, hash) = GetInput();
        var max = 0;
        var time = useElephant ? 26 : 30;

        for (var minute = 1; minute <= time; minute++)
        {
            max = Math.Max(max, states.OrderByDescending(x => x.Pressure).First().Pressure);

            states.RemoveAll(x => x.NoMoreMoves && x.Pressure < max);

            var newStates = new ConcurrentBag<State>();
            Parallel.ForEach(states, state => Parallel.ForEach(state.GetNextState(minute, useElephant), next => newStates.Add(next)));

            states.Clear();
            foreach (var ns in newStates.OrderByDescending(x => x.Pressure).Take(10_000))
            {
                var id = ns.ToString();
                if (!hash.Contains(id))
                {
                    states.Add(ns);
                    hash.Add(id);
                }
            }

            if (states.Count > 0)
            {
                max = Math.Max(max, states.OrderByDescending(x => x.Pressure).First().Pressure);
            }
        }

        return max;
    }

    protected override object Part1() => ReleaseTheValves(false);

    protected override object Part2() => ReleaseTheValves(true);
}