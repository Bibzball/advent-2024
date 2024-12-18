using System.Diagnostics;
using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day18 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Ignore;
    protected override TaskState Task2State => TaskState.Final;
    
    private class PathNode
    {
        public Cell Cell;
        public long Cost;
        public HashSet<Cell> Visited;

        public PathNode(Cell cell, long cost)
        {
            Cell = cell;
            Cost = cost;
            Visited = new HashSet<Cell>();
        }

        public override string ToString()
        {
            return Cell.Position.ToString();
        }
    }
    
    private class Cell
    {
        public enum CellType
        {
            Empty,
            Start,
            End,
        }
        
        private Vector2Int m_Position;
        private Dictionary<Vector2Direction, Cell> m_Neighbours;
        private CellType m_Type;
        public CellType Type { get => m_Type; set => m_Type = value; }
        
        public Vector2Int Position => m_Position;

        public Cell(Vector2Int position)
        {
            m_Position = position;
            m_Neighbours = new Dictionary<Vector2Direction, Cell>();
        }
        
        public void AddNeighbour(Vector2Direction direction, Cell neighbour)
        {
            m_Neighbours[direction] = neighbour;
        }
        
        public bool TryGetNeighbour(Vector2Direction direction, out Cell neighbour)
        {
            return m_Neighbours.TryGetValue(direction, out neighbour);
        }

        public override string ToString()
        {
            switch (m_Type)
            {
                case CellType.Empty:
                    return ".";
                case CellType.Start:
                    return "S";
                case CellType.End:
                    return "E";
            }

            return "?";
        }
    }
    
     private class Map : Vector2Map<Cell>
    {
        private Cell m_Start;
        private Cell m_End;
        
        public Map(Cell[,] cells) : base(cells.GetLength(0), cells.GetLength(1))
        {
            m_Map = cells;
        }
        
        public void SetStart(Cell start)
        {
            m_Start = start;
        }

        public void SetEnd(Cell end)
        {
            m_End = end;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = m_Map[x, y];
                    if (cell == null)
                        sb.Append("#");
                    else
                        sb.Append(m_Map[x, y]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public List<PathNode> Solve(bool stopAtFirstSolution)
        {
            List<PathNode> queue = new();
            var startNode = new PathNode(m_Start, 0);
            Dictionary<Vector2Int, long> cachedCosts = new();
            queue.Add(startNode);

            List<PathNode> solutions = new List<PathNode>();
            
            long minSolutionCost = long.MaxValue;
            
            while (queue.Count > 0)
            {
                var node = queue[0];
                queue.RemoveAt(0);
                node.Visited.Add(node.Cell);
                
                // Console.WriteLine(Print(node));
                // Console.ReadLine();

                // Cutoff immediately
                if (cachedCosts.TryGetValue(node.Cell.Position, out long cachedCost) && node.Cost >= cachedCost)
                    continue;
                
                cachedCosts[node.Cell.Position] = node.Cost;

                if (node.Cell.Type == Cell.CellType.End)
                {
                    solutions.Add(node);
                    if (stopAtFirstSolution)
                        return solutions;
                    minSolutionCost = Math.Min(minSolutionCost, node.Cost);
                    continue;
                }
                
                foreach (var dir in Vector2DirectionExtensions.NonDiagonals)
                {
                    if (!node.Cell.TryGetNeighbour(dir, out Cell neighbour))
                        continue;

                    if (node.Visited.Contains(neighbour))
                        continue;

                    var newNode = new PathNode(neighbour, node.Cost + 1);
                    foreach (var visited in node.Visited)
                        newNode.Visited.Add(visited);
                    
                    // DFS
                    queue.Insert(0, newNode);
                }
            }

            return solutions.Where(a => a.Cost == minSolutionCost).ToList();
        }

        public string Print(PathNode node)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = m_Map[x, y];
                    if (cell == null)
                        sb.Append("#");
                    else if (node != null && node.Visited.Contains(cell))
                        sb.Append("@");
                    else
                        sb.Append(".");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
    
    protected override int ExecuteTask1(string input)
    {
        Vector2Int size;
        int kbs;
        if (Task1State == TaskState.Test)
        {
            size = new Vector2Int(7, 7);
            kbs = 12;
        }
        else
        {
            size = new Vector2Int(71, 71);
            kbs = 1024;
        }
        var map = ParseInput(input, size, kbs);
        var solutions = map.Solve(false);
        return (int)solutions[0].Cost;
    }
    
    private Map ParseInput(string input, Vector2Int size, int kbs)
    {
        var corrupted = new List<Vector2Int>();
        var lines = input.Split('\n');
        for (var i = 0; i < kbs; i++)
        {
            var line = lines[i];
            var rawVec = line.Split(',');
            corrupted.Add(new Vector2Int(int.Parse(rawVec[0]), int.Parse(rawVec[1])));
        }

        Cell[,] cells = new Cell[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (!corrupted.Contains(position))
                {
                    cells[x, y] = new Cell(position);
                }
            }
        }

        var map = new Map(cells);
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (map[x, y] == null)
                    continue;
                
                foreach (var direction in Vector2DirectionExtensions.NonDiagonals)
                {
                    var neighbour = map.GetNeighbour(pos, direction);
                    if (neighbour != null)
                    {
                        map[x, y].AddNeighbour(direction, neighbour);
                    }
                }
            }
        }

        map[0, 0].Type = Cell.CellType.Start;
        map.SetStart(map[0,0]);
        map[size.x - 1, size.y - 1].Type = Cell.CellType.End;
        map.SetEnd(map[size.x - 1, size.y - 1]);
        return map;
    }

    protected override int ExecuteTask2(string input)
    {
        Vector2Int size;
        int kbs;
        if (Task2State == TaskState.Test)
        {
            size = new Vector2Int(7, 7);
            kbs = 12;
        }
        else
        {
            size = new Vector2Int(71, 71);
            // Well... I brute forced this value, I'll be honest :(((
            kbs = 3000;
        }

        var lineCount = input.Split('\n').Length;
        for (int i = kbs + 1; i < lineCount; i++)
        {
            Console.WriteLine($"Let's go {i}!");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var map = ParseInput(input, size, i);
            var solutions = map.Solve(true);
            stopWatch.Stop();
            Console.WriteLine($"Exited in {(long)stopWatch.Elapsed.TotalMilliseconds} ms");
            if (solutions.Count == 0)
                return i;
        }

        return -1;
    }
}