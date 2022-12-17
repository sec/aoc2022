namespace aoc2022.Code;

internal class Day17 : BaseDay
{
    class Game
    {
        const int WIDTH = 7;
        const int MARGIN = 10;
        const int KEEP = 50;
        const int BRICK_SIZE = 4;

        readonly byte[][,] _bricks = new[]
        {
            // -
            new byte[BRICK_SIZE, BRICK_SIZE]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 }
            },
            // +
            new byte[BRICK_SIZE, BRICK_SIZE]
            {
                { 0, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 1, 1, 1, 0 },
                { 0, 1, 0, 0 }
            },
            // L
            new byte[BRICK_SIZE, BRICK_SIZE]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 },
                { 1, 1, 1, 0 }
            },
            // I
            new byte[BRICK_SIZE, BRICK_SIZE]
            {
                { 1, 0, 0, 0 },
                { 1, 0, 0, 0 },
                { 1, 0, 0, 0 },
                { 1, 0, 0, 0 }
            },
            // #
            new byte[BRICK_SIZE, BRICK_SIZE]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 0, 0 },
                { 1, 1, 0, 0 },
            },
        };

        readonly int[] _input;
        readonly byte[,] _map;
        readonly int _width, _height;

        int _currentBrick;
        int _jet;
        int _lastFloor;
        long _added;

        public long Height => _height - _lastFloor + _added;

        public Game(string input)
        {
            _input = input.Select(x => x == '<' ? -1 : 1).ToArray();
            _width = WIDTH;
            _height = KEEP + MARGIN;
            _map = new byte[_height, _width];
            _lastFloor = _height;
        }

        public Game PlayTheGame(long rounds)
        {
            var cache = new Dictionary<string, (long Rounds, long Height)>();

            while (rounds > 0)
            {
                var hash = ToString();

                if (cache.TryGetValue(hash, out var hit))
                {
                    var h = Height - hit.Height;
                    var length = hit.Rounds - rounds;

                    _added += (rounds / length) * h;
                    rounds %= length;

                    break;
                }
                else
                {
                    cache[hash] = (rounds, Height);
                    Tick();
                }
            }

            while (rounds > 0)
            {
                Tick();
            }

            void Tick()
            {
                SpawnBrick();
                CutBottom();
                rounds--;
            }

            return this;
        }

        void SpawnBrick()
        {
            var bx = 2;
            var by = _lastFloor - BRICK_SIZE - 3;
            var brick = _bricks[_currentBrick++ % _bricks.Length];

            while (true)
            {
                var mx = _input[_jet++ % _input.Length];

                if (CanMove(bx, by, brick, mx, 0))
                {
                    bx += mx;
                }

                if (CanMove(bx, by, brick, 0, 1))
                {
                    by++;
                }
                else
                {
                    for (int y = 0; y < BRICK_SIZE; y++)
                    {
                        for (int x = 0; x < BRICK_SIZE; x++)
                        {
                            if (brick[y, x] > 0)
                            {
                                _map[by + y, bx + x] = brick[y, x];

                                _lastFloor = Math.Min(_lastFloor, by + y);
                            }
                        }
                    }
                    break;
                }
            }
        }

        bool CanMove(int bx, int by, byte[,] brick, int mx, int my)
        {
            for (int y = 0; y < BRICK_SIZE; y++)
            {
                for (int x = 0; x < BRICK_SIZE; x++)
                {
                    if (brick[y, x] > 0)
                    {
                        var ty = y + by + my;
                        var tx = x + bx + mx;
                        if (ty < 0 || tx < 0 || ty >= _height || tx >= _width || _map[ty, tx] > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        void CutBottom()
        {
            if (_lastFloor <= MARGIN)
            {
                var tmp = (byte[,]) _map.Clone();

                _added += _height - _lastFloor - KEEP;

                for (int y = 0; y < _height - KEEP; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _map[y, x] = 0;
                    }
                }

                for (int y = _height - KEEP, oldY = _lastFloor; y < _height; y++, oldY++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _map[y, x] = tmp[oldY, x];
                    }
                }

                _lastFloor = _height - KEEP;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = _lastFloor; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    sb.Append(_map[y, x]);
                }
            }
            return sb.ToString();
        }
    }

    long Solve(long rounds) => new Game(ReadAllText()).PlayTheGame(rounds).Height;

    protected override object Part1() => Solve(2022);

    protected override object Part2() => Solve(1_000_000_000_000L);
}