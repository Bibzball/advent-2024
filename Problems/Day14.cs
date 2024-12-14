using System.Text;
using System.Text.RegularExpressions;
using advent_2024.Framework.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace advent_2024.problems;

public class Day14 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Test;
    protected override TaskState Task2State => TaskState.Final;

    private class Cell
    {
        private Vector2Int m_Position;

        public Vector2Int Position => m_Position;

        public Cell(Vector2Int position)
        {
            m_Position = position;
        }
    }

    private class Robot
    {
        private Vector2Int m_Position;
        private Vector2Int m_Velocity;
        
        public Vector2Int Position => m_Position;

        public Robot(string line)
        {
            var regex = @"p\=([0-9]+),([0-9]+) v\=(-*[0-9]+),(-*[0-9]+)";
            var match = Regex.Match(line, regex);
            m_Position = new Vector2Int(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            m_Velocity = new Vector2Int(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
        }

        public void Step(Map map)
        {
            m_Position = map.GetTiledNeighbour(m_Position, m_Velocity).Position;
        }

        public override string ToString()
        {
            return $"({m_Position.x}, {m_Position.y}, {m_Velocity.x}, {m_Velocity.y})";
        }
    }

    private class Map : Vector2Map<Cell>
    {
        private List<Robot> m_Robots = new List<Robot>();
        
        public Map(int width, int height) : base(width, height) { }

        public void AddRobot(Robot robot)
        {
            m_Robots.Add(robot);
        }

        public int GetQuadrant(Vector2Int position)
        {
            var halfWidth = Width / 2;
            var halfHeight = Height / 2;

            if (position.x == halfWidth || position.y == halfHeight)
                return -1;
            
            if (position.x < halfWidth && position.y < halfHeight)
                return 0;
            if (position.x > halfWidth && position.y < halfHeight)
                return 1;
            if (position.x < halfWidth && position.y > halfHeight)
                return 2;

            return 3;
        }

        public void Step()
        {
            foreach (var robot in m_Robots)
                robot.Step(this);
        }

        public Dictionary<int, int> GetRobotCountsPerQuadrant()
        {
            var counts = new Dictionary<int, int>();
            foreach (var robot in m_Robots)
            {
                var quadrant = GetQuadrant(robot.Position);
                if (!counts.TryAdd(quadrant, 1))
                    counts[quadrant]++;
            }
            return counts;
        }

        public Image<Rgba32> Render()
        {
            Dictionary<Vector2Int, int> cellCounts = new Dictionary<Vector2Int, int>();
            foreach (var robot in m_Robots)
            {
                if (!cellCounts.TryAdd(robot.Position, 1))
                    cellCounts[robot.Position]++;
            }

            Image<Rgba32> image = new Image<Rgba32>(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color;
                    if (cellCounts.ContainsKey(new Vector2Int(x, y)))
                        color = Color.White;
                    else
                        color = Color.Black;

                    image[x, y] = color;
                }
            }

            return image;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Vector2Int, int> cellCounts = new Dictionary<Vector2Int, int>();
            foreach (var robot in m_Robots)
            {
                if (!cellCounts.TryAdd(robot.Position, 1))
                    cellCounts[robot.Position]++;
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pos = new Vector2Int(x, y);
                    var count = cellCounts.GetValueOrDefault(pos, 0);
                    if (count == 0)
                        sb.Append('.');
                    else
                        sb.Append(count);
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    
    protected override long ExecuteTask1(string input)
    {
        var map = ParseInput(input, Task1State);
        for (int i = 0; i < 100; i++)
        {
            map.Step();
        }
        
        var quadrants = map.GetRobotCountsPerQuadrant();
        long result = 1;    
        for (int i = 0; i < 4; i++)
        {
            if (quadrants.TryGetValue(i, out var quadrantCount))
                result *= quadrantCount;
        }

        return result;
    }

    private Map ParseInput(string input, TaskState state)
    {
        var lines = input.Split('\n');

        int width, height;
        if (state == TaskState.Test)
        {
            width = 11;
            height = 7;
        }
        else
        {
            width = 101;
            height = 103;
        }

        Map map = new Map(width, height);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var cell = new Cell(new Vector2Int(x, y));
                map[x, y] = cell;
            }
        }
        
        foreach (var line in lines)
        {
            var r = new Robot(line.Trim());
            map.AddRobot(r);
        }

        return map;
    }

    protected override long ExecuteTask2(string input)
    {
        var map = ParseInput(input, Task2State);
        for (int i = 0; i < 10000; i++)
        {
            map.Step();
            map.Render().Save($"Day14/{i + 1}.png");
        }

        return 0;
    }
}