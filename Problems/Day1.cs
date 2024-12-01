using System.Net;
using advent_2024.Framework.Logger;

namespace advent_2024.problems;

public class Day1 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    protected override int ExecuteTask1(string input)
    {
        ParseToLists(input, out int[] list1, out int[] list2);
        return ComputeDistance(list1, list2);
    }

    protected override int ExecuteTask2(string input)
    {
        ParseToLists(input, out int[] list1, out int[] list2);
        return ComputeSimilarity(list1, list2);
    }
    
    private void ParseToLists(string content, out int[] ints1, out int[] ints2)
    { 
        var lines = content.Trim().Split('\n');
        ints1 = new int[lines.Length];
        ints2 = new int[lines.Length];

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            var parts = line.Split("   ");
            ints1[i] = int.Parse(parts[0]);
            ints2[i] = int.Parse(parts[1]);
        }
        
        Array.Sort(ints1);
        Array.Sort(ints2);
    }

    private int ComputeDistance(int[] list1, int[] list2)
    {
        int result = 0;
        for (var i = 0; i < list1.Length; i++)
        {
            var distance = Math.Abs(list1[i] - list2[i]);
            result += distance;
        }

        return result;
    }

    private int ComputeSimilarity(int[] list1, int[] list2)
    {
        Dictionary<int, int> list2Counts = new Dictionary<int, int>();
        for (var i = 0; i < list2.Length; i++)
        {
            if (!list2Counts.ContainsKey(list2[i]))
                list2Counts.Add(list2[i], 1);
            else
                list2Counts[list2[i]]++;
        }
        
        var result = 0;
        for (var i = 0; i < list1.Length; i++)
        {
            var value = list1[i];
            var similarity = list2Counts.GetValueOrDefault(value, 0);
            result += value * similarity;
        }

        return result;
    }
}