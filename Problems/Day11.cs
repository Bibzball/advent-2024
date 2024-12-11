namespace advent_2024.problems;

public class Day11 : Day<int, long>
{
    protected override TaskState Task1State => TaskState.Test;
    protected override TaskState Task2State => TaskState.Final;

    private class Stone : IEquatable<Stone>
    {
        private long m_Value;

        public Stone(long value)
        {
            m_Value = value;
        }

        public Stone(String value)
        {
            m_Value = long.Parse(value);
        }
        
        public List<Stone> Step()
        {
            List<Stone> result = new List<Stone>();
            if (m_Value == 0)
            {
                result.Add(new Stone(1));
                return result;
            }

            var str = m_Value.ToString();
            if (str.Length % 2 == 0)
            {
                var str1 = str.Substring(0, str.Length / 2);
                var str2 = str.Substring(str.Length / 2);
                result.Add(new Stone(str1));
                result.Add(new Stone(str2));
                return result;
            }

            result.Add(new Stone(m_Value * 2024));
            return result;
        }

        public override string ToString() => m_Value.ToString();
        public bool Equals(Stone? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return m_Value == other.m_Value;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((Stone)obj);
        }
        public override int GetHashCode() => m_Value.GetHashCode();
    }
    
    protected override int ExecuteTask1(string input)
    {
        List<Stone> stones = ParseInput(input);
        const int blinkCount = 25;
        stones = BlinkStones(stones, blinkCount);
        return stones.Count;
    }
    
    private List<Stone> BlinkStones(List<Stone> stones, int blinkCount)
    {
        // Console.WriteLine(String.Join(' ', stones));
        
        for (int b = 0; b < blinkCount; b++)
        {
            for (int i = stones.Count - 1; i >= 0; i--)
            {
                var stone = stones[i];
                stones.RemoveAt(i);
                foreach (var newStone in stone.Step())
                    stones.Insert(i, newStone);
            }
            
            // Console.WriteLine(String.Join(' ', stones));
        }
        
        return stones;
    }
    
    private Dictionary<Stone, long> BlinkStonesCache(List<Stone> stones, int blinkCount)
    {
        // Console.WriteLine(String.Join(' ', stones));
        Dictionary<Stone, long> stoneCache = new Dictionary<Stone, long>();
        foreach (var stone in stones)
        {
            if (!stoneCache.ContainsKey(stone))
                stoneCache.Add(stone, 1);
            else
                stoneCache[stone]++;
        }
        
        for (int b = 0; b < blinkCount; b++)
        {
            Dictionary<Stone, long> stoneCacheCopy = new Dictionary<Stone, long>();
            foreach (var kvpStone in stoneCache)
            {
                var stoneCount = kvpStone.Value;
                var newStones = kvpStone.Key.Step();
                foreach (var newStone in newStones)
                {
                    if (!stoneCacheCopy.ContainsKey(newStone))
                        stoneCacheCopy.Add(newStone, stoneCount);
                    else
                        stoneCacheCopy[newStone]+= stoneCount;
                }
            }

            stoneCache = stoneCacheCopy;
            
            // Console.WriteLine(String.Join(' ', stones));
        }
        
        return stoneCache;
    }

    private List<Stone> ParseInput(string input)
    {
        List<Stone> stones = new List<Stone>();
        var stonesRaw = input.Split(' ');
        foreach (var stoneRaw in stonesRaw)
        {
            stones.Add(new Stone(stoneRaw));
        }

        return stones;
    }

    protected override long ExecuteTask2(string input)
    {
        List<Stone> stones = ParseInput(input);
        const int blinkCount = 75;
        var stoneCache = BlinkStonesCache(stones, blinkCount);
        long res = 0;
        foreach (var kvp in stoneCache)
            res += kvp.Value;
        return res;
    }
}