using advent_2024.Framework.Logger;
using advent_2024.problems;

public class Advent2024
{
    static void Main(string[] args)
    {
        // var day = new Day1();
        // var day = new Day2();
        // var day = new Day3();
        var day = new Day4();
        // var day = new Day5();
        // var day = new Day6();
        // var day = new Day7();
        // var day = new Day8();
        // var day = new Day9();
        // var day = new Day10();
        // var day = new Day11();
        // var day = new Day12();
        // var day = new Day13();
        // var day = new Day14();
        // var day = new Day15();
        // var day = new Day16();
        // var day = new Day17();
        // var day = new Day18();
        // var day = new Day19();
        // var day = new Day20();
        // var day = new Day21();
        // var day = new Day22();
        // var day = new Day23();
        // var day = new Day24();

        Task1(day);
        Task2(day);
    }

    private static void Task1<T1, T2>(Day<T1, T2> day)
    {
        T1 result = day.Task1();
        Logging.LogInfo($"{day.GetType().Name}: Task 1", result.ToString());
    }
    
    private static void Task2<T1, T2>(Day<T1, T2> day)
    {
        T2 result = day.Task2();
        Logging.LogInfo($"{day.GetType().Name}: Task 2", result.ToString());
    }
}