using System.Collections;
using System.Numerics;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day10 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    private class Cell
    {
        private int m_Height;
        public int Height => m_Height;
        
        private Vector2Int m_Position;
        public Vector2Int Position => m_Position;

        public Cell(int height, Vector2Int position)
        {
            m_Height = height;
            m_Position = position;
        }

        public override string ToString()
        {
            return $"{m_Position} {m_Height}";
        }
    }

    private class Map : Vector2Map<Cell>
    {
        public Map(int width, int height) : base(width, height)
        {
        }
    }
        
    
    protected override int ExecuteTask1(string input)
    {
        var map = ParseInput(input);
        var trailheads = FindTrailheads(map);
        int result = 0;
        foreach (var trailhead in trailheads)
        {
            result += FindTrailheadScore(map, trailhead);
        }

        return result;
    }

    private int FindTrailheadScore(Map map, Cell trailhead)
    {
        HashSet<Cell> visited = new HashSet<Cell>();
        HashSet<Cell> peaks = new HashSet<Cell>();
        FindPeaks(map, trailhead, visited, peaks);
        return peaks.Count;
    }
    
    private void FindPeaks(Map map, Cell cell, HashSet<Cell> visited, HashSet<Cell> peaks)
    {
        visited.Add(cell);
        
        foreach (Vector2Direction direction in Vector2DirectionExtensions.NonDiagonals)
        {
            var neighbour = map.GetNeighbour(cell.Position, direction);
            if (neighbour == null)
                continue;
            
            if (visited.Contains(neighbour))
                continue;
            
            if (neighbour.Height == cell.Height + 1)
            {
                if (neighbour.Height == 9)
                {
                    peaks.Add(neighbour);
                }
                else
                {
                    HashSet<Cell> visitedCopy = new HashSet<Cell>(visited);
                    FindPeaks(map, neighbour, visitedCopy, peaks);
                }
            }
        }
    }

    private List<Cell> FindTrailheads(Map map)
    {
        List<Cell> result = new();
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var cell = map[x, y];
                if (cell?.Height == 0)
                    result.Add(cell);
            }
        }

        return result;
    }

    private Map ParseInput(string input)
    {
        var lines = input.Split("\n");
        var width = lines[0].Length;
        var height = lines.Length;
        
        var map = new Map(width, height);
        
        for (var y = 0; y < height; y++)
        {
            var line = lines[y];
            for (var x = 0; x < width; x++)
            {
                var cell = new Cell(int.Parse(line[x].ToString()), new Vector2Int(x, y));
                map[x, y] = cell;
            }
        }

        return map;
    }

    protected override int ExecuteTask2(string input)
    {
        var map = ParseInput(input);
        var trailheads = FindTrailheads(map);
        int result = 0;
        foreach (var trailhead in trailheads)
        {
            result += FindTrailheadRating(map, trailhead);
        }

        return result;
    }

    private int FindTrailheadRating(Map map, Cell trailhead)
    {
        HashSet<Cell> visited = new HashSet<Cell>();
        return FindTrailheadRating(map, trailhead, visited);
    }

    private int FindTrailheadRating(Map map, Cell cell, HashSet<Cell> visited)
    {
        visited.Add(cell);
        int value = 0;
        
        foreach (Vector2Direction direction in Vector2DirectionExtensions.NonDiagonals)
        {
            var neighbour = map.GetNeighbour(cell.Position, direction);
            if (neighbour == null)
                continue;
            
            if (visited.Contains(neighbour))
                continue;
            
            if (neighbour.Height == cell.Height + 1)
            {
                if (neighbour.Height == 9)
                {
                    value++;
                }
                else
                {
                    HashSet<Cell> visitedCopy = new HashSet<Cell>(visited);
                    value += FindTrailheadRating(map, neighbour, visitedCopy);
                }
            }
        }

        return value;
    }
}