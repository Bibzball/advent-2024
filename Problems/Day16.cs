using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day16 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Test;
    protected override TaskState Task2State => TaskState.Final;

    private class PathNode
    {
        public Cell Cell;
        public long Cost;
        public Vector2Direction Direction;
        public HashSet<Cell> Visited;

        public PathNode(Cell cell, long cost, Vector2Direction direction)
        {
            Cell = cell;
            Cost = cost;
            Direction = direction;
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

        public List<PathNode> Solve()
        {
            List<PathNode> queue = new();
            var startNode = new PathNode(m_Start, 0, Vector2Direction.E);
            Dictionary<Vector2Int, Dictionary<Vector2Direction, long>> cachedCosts = new();
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
                if (!cachedCosts.ContainsKey(node.Cell.Position))
                {
                    cachedCosts.Add(node.Cell.Position, new Dictionary<Vector2Direction, long>());
                }
                else
                {
                    var cachedCostsForCell = cachedCosts[node.Cell.Position];
                    if (cachedCostsForCell.TryGetValue(node.Direction, out long cachedCost) && node.Cost > cachedCost)
                        continue;
                }
                
                cachedCosts[node.Cell.Position][node.Direction] = node.Cost;

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

                    if (node.Visited.Contains(neighbour))
                        continue;

                    var nodeCost = node.Cost;
                    if (dir == node.Direction)
                        nodeCost++;
                    else
                        nodeCost+=1001;
                    
                    var newNode = new PathNode(neighbour, nodeCost, dir);
                    foreach (var visited in node.Visited)
                        newNode.Visited.Add(visited);
                    
                    // Insert the node sorted such as the queue always has nodes in increasing order of cost
                    int index = queue.BinarySearch(newNode, Comparer<PathNode>.Create((a, b) => a.Cost.CompareTo(b.Cost)));
                    if (index < 0)
                        index = ~index;
                    
                    queue.Insert(index, newNode);
                }
            }

            return solutions.Where(a => a.Cost == minSolutionCost).ToList();
        }
        
        public string Print(HashSet<Vector2Int> bestTiles)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = m_Map[x, y];
                    if (cell == null)
                        sb.Append("#");
                    else if (bestTiles.Contains(new Vector2Int(x, y)))
                        sb.Append("@");
                    else
                        sb.Append(".");
                }
                sb.AppendLine();
            }

            return sb.ToString();
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
                    else if (node.Visited.Contains(cell))
                        sb.Append("@");
                    else
                        sb.Append(".");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
    
    protected override long ExecuteTask1(string input)
    {
        var map = ParseInput(input);
        var solutions = map.Solve();
        return solutions[0].Cost;
    }

    protected override long ExecuteTask2(string input)
    {
        var map = ParseInput(input);
        var solutions = map.Solve();
        HashSet<Vector2Int> bestTiles = new();
        foreach (var solution in solutions)
        {
            foreach (var cell in solution.Visited)
                bestTiles.Add(cell.Position);
        }

        // Console.WriteLine(map.Print(bestTiles));

        return bestTiles.Count;
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

                bool isWall = false;

                if (c == 'S')
                {
                    cell.Type = Cell.CellType.Start;
                    map.SetStart(cell);
                }
                else if (c == 'E')
                {
                    cell.Type = Cell.CellType.End;
                    map.SetEnd(cell);
                }
                else if (c == '#')
                    isWall = true;

                if (isWall) continue;
                
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