namespace advent_2024.problems;

public class Day19 : Day<int, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    private Dictionary<string, bool> m_doableCache = new Dictionary<string, bool>();
    private Dictionary<string, long> m_countCache = new Dictionary<string, long>();
    
    protected override int ExecuteTask1(string input)
    {
        ParseInput(input, out string[] availablePatterns, out string[] targetDesigns);
        int result = 0;
        
        m_doableCache.Clear();
        
        foreach (var design in targetDesigns)
        {
            if (IsPatternDoable(design, availablePatterns))
                result++;
        }
        
        return result;
    }


    private bool IsPatternDoable(string design, string[] availablePatterns)
    {
        if (design.Length == 0)
            return true;
        
        if (m_doableCache.TryGetValue(design, out var doable))
            return doable;
        
        foreach (var candidate in availablePatterns)
        {
            if (design.StartsWith(candidate))
            {
                if (IsPatternDoable(design.Substring(candidate.Length), availablePatterns))
                {
                    m_doableCache[design] = true;
                    return true;
                }
            }
        }

        m_doableCache[design] = false;
        return false;
    }

    private long HowManyWays(string design, string[] availablePatterns)
    {
        if (design.Length == 0)
            return 1;
        
        if (m_countCache.TryGetValue(design, out var knownCount))
            return knownCount;
     
        long count = 0;
        foreach (var candidate in availablePatterns)
        {
            if (design.StartsWith(candidate))
            {
                count += HowManyWays(design.Substring(candidate.Length), availablePatterns);
            }
        }

        m_countCache[design] = count;
        return count;
    }
    
    private void ParseInput(string input, out string[] availablePatterns, out string[] targetPatterns)
    {
        var lines = input.Split('\n');
        availablePatterns = lines[0].Split(',');
        for (int i = 0; i < availablePatterns.Length; i++)
            availablePatterns[i] = availablePatterns[i].Trim();

        List<string> targetPatternsList = new List<string>();
        for (int i = 2; i < lines.Length; i++)
        {
            targetPatternsList.Add(lines[i]);
        }

        targetPatterns = targetPatternsList.ToArray();
    }

    protected override long ExecuteTask2(string input)
    {
        ParseInput(input, out string[] availablePatterns, out string[] targetDesigns);
        long result = 0;
        
        m_countCache.Clear();
        
        foreach (var design in targetDesigns)
        {
            result += HowManyWays(design, availablePatterns);
        }
        
        return result;
    }
}