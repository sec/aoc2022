global using System;
global using System.Linq;
global using System.Reflection;
global using System.Collections.Generic;
global using System.IO;
global using System.Text.RegularExpressions;
global using System.Text;
global using aoc2022.Infra;

var day = DateTime.Now.Day;
if (args.Length == 1)
{
    day = int.Parse(args[0]);
}

var t = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(x => x.Name == $"Day{day:d2}")
    .Single();

if (Activator.CreateInstance(t) is BaseDay c)
{
    c.Solve(true);
    c.Solve(false);
}