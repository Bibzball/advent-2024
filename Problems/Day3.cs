using System.Text.RegularExpressions;

namespace advent_2024.problems;

public class Day3 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override int ExecuteTask1(string input)
    {
        return ParseInput1(input);
    }
    protected override int ExecuteTask2(string input)
    {
        return ParseInput2(input);
    }
    
    private int ParseInput1(string input)
    {
        int result = 0;
        var regex = @"mul\(([0-9]+),([0-9]+)\)";
        var matches = Regex.Matches(input, regex);
        foreach (Match match in matches)
        {
            var op1 = int.Parse(match.Groups[1].Value);
            var op2 = int.Parse(match.Groups[2].Value);
            result += op1 * op2;
        }

        return result;
    }

    private int ParseInput2(string input)
    {
        int result = 0;
        var doSplits = input.Split("do()");
        for (var i = 0; i < doSplits.Length; i++)
        {
            var doSplit = doSplits[i];
            var dontIndex = doSplit.IndexOf("don't()");
            if (dontIndex != -1)
            {
                doSplit = doSplit.Substring(0, dontIndex);
            }

            result += ParseInput1(doSplit);
        }

        return result;
    }
}