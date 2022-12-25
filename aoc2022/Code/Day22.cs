namespace aoc2022.Code;

internal class Day22 : BaseDay
{
    class Map
    {
        enum Face { Right = 0, Down = 1, Left = 2, Up = 3 }

        readonly (int, int)[] _moves = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

        readonly int _width, _height;
        readonly string _path;
        readonly char[,] _map;

        int _cx, _cy;
        Face _face;

        public Map(string[] input)
        {
            _width = input.Select(x => x.Length).Max();
            _height = input.Length - 1;

            _path = input.Last();
            _map = new char[_height, _width];
            _face = Face.Right;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (x >= input[y].Length)
                    {
                        _map[y, x] = ' ';
                        continue;
                    }

                    _map[y, x] = input[y][x];

                    if (_cx == 0 && _cx == _cy && _map[y, x] == '.')
                    {
                        _cx = x;
                        _cy = y;
                    }
                }
            }
        }

        public Map Walk(bool use3d)
        {
            var sb = new StringBuilder();
            foreach (var c in _path)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
                else
                {
                    Move(int.Parse(sb.ToString()), use3d);
                    Turn(c);
                    sb.Clear();
                }
            }
            Move(int.Parse(sb.ToString()), use3d);

            return this;
        }

        private void Turn(char c)
        {
            var right = new[] { Face.Right, Face.Down, Face.Left, Face.Up };
            var left = new[] { Face.Left, Face.Down, Face.Right, Face.Up };

            switch (c)
            {
                case 'R':
                    _face = right[(int) (_face + 1) % 4];
                    break;
                case 'L':
                    _face = left[(int) (_face + 1) % 4];
                    break;
            }
        }

        bool CanMove(int addX, int addY) => _cx + addX >= 0 && _cx + addX < _width && _cy + addY >= 0 && _cy + addY < _height && Empty(_cx + addX, _cy + addY);

        bool OffGrid(int addX, int addY) => _cx + addX < 0 || _cx + addX >= _width || _cy + addY < 0 || _cy + addY >= _height || Void(_cx + addX, _cy + addY);

        bool Empty(int x, int y) => _map[y, x] == '.';

        bool Void(int x, int y) => _map[y, x] == ' ';

        bool NonVoid(int x, int y) => _map[y, x] != ' ';

        int WrapX(int y, int sign)
        {
            for (int i = sign > 0 ? 0 : _width - 1; i < _width && i >= 0; i += sign)
            {
                if (NonVoid(i, y))
                {
                    return i;
                }
            }
            throw new InvalidDataException();
        }

        int WrapY(int x, int sign)
        {
            for (int i = sign > 0 ? 0 : _height - 1; i < _height && i >= 0; i += sign)
            {
                if (NonVoid(x, i))
                {
                    return i;
                }
            }
            throw new InvalidDataException();
        }

        private void Move(int v, bool use3d)
        {
            var (addX, addY) = _moves[(int) _face];
            while (v-- > 0)
            {
                if (CanMove(addX, addY))
                {
                    _cx += addX;
                    _cy += addY;

                    continue;
                }

                if (OffGrid(addX, addY))
                {
                    var x = -1;
                    var y = -1;
                    if (!use3d)
                    {
                        switch (_face)
                        {
                            case Face.Left:
                            case Face.Right:
                                y = _cy;
                                x = WrapX(y, _face == Face.Left ? -1 : 1);
                                break;

                            case Face.Down:
                            case Face.Up:
                                x = _cx;
                                y = WrapY(x, _face == Face.Down ? 1 : -1);
                                break;
                        }
                    }
                    else
                    {
                        // 3d magic here
                        x = _cx;
                        y = _cy;

                        var mapping = GetMapping();

                        var cubeX = _cx / 50;
                        var cubeY = _cy / 50;

                        var map = mapping[(cubeX, cubeY, _face)];
                    }

                    if (Empty(x, y))
                    {
                        _cx = x;
                        _cy = y;
                    }
                }
            }
        }

        public int Password => 1000 * (_cy + 1) + 4 * (_cx + 1) + (int) _face;

        Dictionary<(int CubeX, int CubeY, Face Dir), (int CubeX, int CubeY, Face Dir)> GetMapping()
        {
            return new()
            {
                { (1, 0, Face.Right), (2, 0, Face.Right) },
                { (1, 0, Face.Left), (0, 2, Face.Right) },
                { (1, 0, Face.Up), (0, 3, Face.Right) },
                { (1, 0, Face.Down), (1, 1, Face.Down) },

                { (2, 0, Face.Right), (1, 2, Face.Left) },
                { (2, 0, Face.Left), (1, 0, Face.Left) },
                { (2, 0, Face.Up), (0, 3, Face.Up) },
                { (2, 0, Face.Down), (1, 1, Face.Left) },

                { (1, 1, Face.Right), (2, 0, Face.Up) },
                { (1, 1, Face.Left), (0, 2, Face.Down) },
                { (1, 1, Face.Up), (1, 0, Face.Up) },
                { (1, 1, Face.Down), (1, 2, Face.Down) },

                { (1, 2, Face.Right), (2, 0, Face.Left) },
                { (1, 2, Face.Left), (0, 2, Face.Left) },
                { (1, 2, Face.Up), (1, 1, Face.Up) },
                { (1, 2, Face.Down), (0, 3, Face.Left) },

                { (0, 2, Face.Right), (1, 2, Face.Right) },
                { (0, 2, Face.Left), (1, 0, Face.Right) },
                { (0, 2, Face.Up), (1, 1, Face.Right) },
                { (0, 2, Face.Down), (0, 3, Face.Down) },

                { (0, 3, Face.Right), (1, 2, Face.Up) },
                { (0, 3, Face.Left), (1, 0, Face.Down) },
                { (0, 3, Face.Up), (0, 2, Face.Up) },
                { (0, 3, Face.Down), (2, 0, Face.Down) }
            };
        }
    }

    protected override object Part1() => new Map(ReadAllLines(true)).Walk(false).Password;

    protected override object Part2() => _testRun ? -1 : new Map(ReadAllLines(true)).Walk(true).Password;
}