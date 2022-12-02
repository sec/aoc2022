namespace aoc2022.Code;

internal class Day02 : BaseDay
{
    const int DRAW = 3;
    const int WON = 6;
    const int LOST = 0;

    static int Fixed(string s)
    {
        return s switch
        {
            // Rock vs
            "A X" => 1 + DRAW,
            "A Y" => 2 + WON,
            "A Z" => 3 + LOST,

            // Paper vs
            "B X" => 1 + LOST,
            "B Y" => 2 + DRAW,
            "B Z" => 3 + WON,

            // Scissors vs
            "C X" => 1 + WON,
            "C Y" => 2 + LOST,
            "C Z" => 3 + DRAW,

            _ => throw new NotImplementedException()
        };
    }

    static string Adjust(string s)
    {
        return s switch
        {
            // Rock vs
            "A X" => "A Z",
            "A Y" => "A X",
            "A Z" => "A Y",

            // Paper vs
            "B X" => "B X",
            "B Y" => "B Y",
            "B Z" => "B Z",

            // Scissors vs
            "C X" => "C Y",
            "C Y" => "C Z",
            "C Z" => "C X",

            _ => throw new NotImplementedException()
        };
    }

    protected override object Part1() => ReadAllLines().Select(Fixed).Sum();

    protected override object Part2() => ReadAllLines().Select(x => Fixed(Adjust(x))).Sum();
}