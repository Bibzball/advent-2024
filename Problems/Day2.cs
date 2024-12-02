using advent_2024.Framework.Logger;

namespace advent_2024.problems;

public class Day2 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override int ExecuteTask1(string input)
    {
        var raw = ReadInput(input);
        int safeCount = 0;
        foreach (var list in raw)
        {
            if (IsSafe(list))
                safeCount++;
        }

        return safeCount;
    }

    protected override int ExecuteTask2(string input)
    {
        var raw = ReadInput(input);
        int safeCount = 0;
        foreach (var list in raw)
        {
            if (IsSafeWithTolerance(list))
            {
                // Logging.LogDebug("Safe", string.Join(',', list)); 
                safeCount++;
            }
            // else
            // {
            //     Logging.LogDebug("Not Safe", string.Join(',', list));
            // }
                
        }

        return safeCount;
    }

    private List<List<int>> ReadInput(string input)
    {
        List<List<int>> result = new List<List<int>>();
        
        var lines = input.Trim().Split('\n');
        foreach (var line in lines)
        {
            var list = new List<int>();
            var values = line.Split(' ');
            foreach (var value in values)
            {
                list.Add(int.Parse(value));
            }
            result.Add(list);
        }

        return result;
    }

    private bool IsSafe(List<int> input)
    {
        int sign = Math.Sign(input[1] - input[0]);
        for (int i = 1; i < input.Count; i++)
        {
            int diff = input[i] - input[i - 1];
            if (Math.Sign(diff) != sign)
                return false;
            if (Math.Abs(diff) <= 0 || Math.Abs(diff) > 3)
                return false;
        }

        return true;
    }

    private bool IsSafeWithTolerance(List<int> input)
    {
        if (IsSafe(input))
            return true;

        for (int i = 0; i < input.Count; i++)
        {
            var copy = new List<int>(input);
            copy.RemoveAt(i);
            if (IsSafe(copy))
                return true;
        }

        return false;
    }
}