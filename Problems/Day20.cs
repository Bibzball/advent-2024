using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day20 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Ignore;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override long ExecuteTask1(string input)
    {
        var map = ParseInput(input);
        var solve = map.FindBest();
        
        Console.WriteLine(map.Print(solve));
        var allCheats = map.FindAllCheats(solve, 2);
        allCheats.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));
        // SortedDictionary<long, int> cheatsBySave = new SortedDictionary<long, int>();

        long total = 0;
        foreach (var cheat in allCheats)
        {
            if (cheat.Save >= 100)
                total++;
            
            // if (!cheatsBySave.TryAdd(cheat.Save, 1))
            //     cheatsBySave[cheat.Save]++;
        }

        
        // foreach (var kvp in cheatsBySave)
        // {
        //     Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        // }
        
        return total;
    }

    protected override long ExecuteTask2(string input)
    {
        var map = ParseInput(input);
        var solve = map.FindBest();
        
        var allCheats = map.FindAllCheats(solve, 20);
        allCheats.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));

        long total = 0;
        foreach (var cheat in allCheats)
        {
            if (cheat.Save >= 100)
                total++;
        }
        
        return total;
    }
    
    private class PathNode
    {
        public Cell Cell;
        public long Cost;
        public List<PathNode> Visited;

        public PathNode(Cell cell, long cost)
        {
            Cell = cell;
            Cost = cost;
            Visited = new List<PathNode>();
        }

        public override string ToString()
        {
            return Cell.Position.ToString();
        }
    }
    
    private class Cell : IEquatable<Cell>
    {
        public enum CellType
        {
            Empty,
            Wall,
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
                case CellType.Wall:
                    return "#";
            }

            return "?";
        }
        public bool Equals(Cell? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return m_Position.Equals(other.m_Position);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((Cell)obj);
        }
        
        public override int GetHashCode() => m_Position.GetHashCode();
    }

    private class Cheat
    {
        public int StartIndex;
        public int EndIndex;
        public long Save;
        
        public Cheat(int startIndex, int endIndex, long save)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Save = save;
        }
    }
    
    private class Map : Vector2Map<Cell>
    {
        private Cell m_Start;
        
        public Map(Cell[,] cells) : base(cells.GetLength(0), cells.GetLength(1))
        {
            m_Map = cells;
        }
        
        public void SetStart(Cell start)
        {
            m_Start = start;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(m_Map[x, y]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        
        public PathNode FindBest()
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
                node.Visited.Add(node);
                
                // Cutoff immediately
                if (cachedCosts.TryGetValue(node.Cell.Position, out long cachedCost) && node.Cost > cachedCost)
                    continue;
                
                if (node.Cell.Type == Cell.CellType.End)
                {
                    solutions.Add(node);
                    minSolutionCost = Math.Min(minSolutionCost, node.Cost);
                    continue;
                }
                
                foreach (var dir in Vector2DirectionExtensions.NonDiagonals)
                {
                    if (!node.Cell.TryGetNeighbour(dir, out Cell neighbour))
                        continue;

                    if (node.Visited.Any(visitedNode => visitedNode.Cell.Equals(neighbour)))
                        continue;

                    if (neighbour.Type == Cell.CellType.Wall)
                        continue;

                    var nodeCost = node.Cost + 1;
                    
                    var newNode = new PathNode(neighbour, nodeCost);
                    foreach (var visited in node.Visited)
                        newNode.Visited.Add(visited);
                    
                    // Insert the node sorted such as the queue always has nodes in increasing order of cost
                    int index = queue.BinarySearch(newNode, Comparer<PathNode>.Create((a, b) => a.Cost.CompareTo(b.Cost)));
                    if (index < 0)
                        index = ~index;
                    
                    queue.Insert(index, newNode);
                }
            }

            return solutions.Where(a => a.Cost == minSolutionCost).ToList()[0];
        }
        
        public List<Cheat> FindAllCheats(PathNode bestSolution, int cheatDistance)
        {
            List<Cheat> solutions = new List<Cheat>();

            for (int i = 0; i < bestSolution.Visited.Count; i++)
            {
                var currentNode = bestSolution.Visited[i];
                var currentCell = currentNode.Cell;

                for (int j = bestSolution.Visited.Count - 1; j > i + 2; j--)
                {
                    var targetNode = bestSolution.Visited[j];
                    var targetCell = targetNode.Cell;

                    var targetDistance = Vector2Int.ManhattanDistance(currentCell.Position, targetCell.Position);
                    if (targetDistance <= cheatDistance)
                    {
                        solutions.Add(new Cheat(i, j, j - i - targetDistance));
                    }
                }
            }
         
            return solutions;
        }

        public string Print(PathNode node)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = m_Map[x, y];
                    if (node.Visited.Any(visitedNode => visitedNode.Cell.Equals(cell)))
                        sb.Append("@");
                    else
                        sb.Append(m_Map[x, y]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    private Map ParseInput(string input)
    {
        var lines = input.Split('\n');
        var height = lines.Length;
        var width = lines[0].Length;

        var cells = new Cell[width, height];
        var map = new Map(cells);

        for (var y = 0; y < height; y++)
        {
            var line = lines[y];
            for (var x = 0; x < width; x++)
            {
                var c = line[x];
                var pos = new Vector2Int(x, y);
                var cell = new Cell(pos);
                
                if (c == 'S')
                {
                    cell.Type = Cell.CellType.Start;
                    map.SetStart(cell);
                }
                else if (c == 'E')
                {
                    cell.Type = Cell.CellType.End;
                }
                else if (c == '#')
                {
                    cell.Type = Cell.CellType.Wall;
                }
                
                cells[x, y] = cell;
                foreach (var direction in Vector2DirectionExtensions.NonDiagonals)
                {
                    var neighbour = map.GetNeighbour(pos, direction);
                    if (neighbour != null)
                    {
                        cell.AddNeighbour(direction, neighbour);
                        neighbour.AddNeighbour(direction.Opposite(), cell);
                    }
                }
            }
        }

        return map;
    }
}