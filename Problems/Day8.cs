using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day8 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    private class Cell
    {
        private char m_Value;
        public char Value => m_Value;
        
        private Vector2Int m_Position;
        public Vector2Int Position => m_Position;

        public Cell(int x, int y, char value)
        {
            m_Value = value;
            m_Position = new Vector2Int(x, y);
        }
    }
    
    private class Map : Vector2Map<Cell>
    {
        private Dictionary<char, List<Cell>> m_CellsByChar;
        public Dictionary<char, List<Cell>> CellsByChar => m_CellsByChar;

        private HashSet<Vector2Int> m_Antinodes;
        public int AntinodesCount => m_Antinodes.Count;
        
        public Map(Cell[,] values) : base(values.GetLength(0), values.GetLength(1))
        {
            m_Antinodes = new HashSet<Vector2Int>();
            m_CellsByChar = new Dictionary<char, List<Cell>>();
            
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var c = values[x, y];
                    m_Map[x, y] = c;

                    if (c.Value == '.')
                        continue;
                    
                    if (!m_CellsByChar.ContainsKey(c.Value))
                        m_CellsByChar.Add(c.Value, new List<Cell>());

                    m_CellsByChar[c.Value].Add(c);
                }
            }
        }

        public void AddAntinode(Vector2Int cellPos)
        {
            m_Antinodes.Add(cellPos);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var c = m_Map[x, y];
                    if (m_Antinodes.Contains(c.Position) && c.Value == '.')
                        sb.Append('#');
                    else
                        sb.Append(c.Value);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    
    protected override int ExecuteTask1(string input)
    {
        var map = ParseInput(input);

        foreach (var charCells in map.CellsByChar)
        {
            if (charCells.Value.Count <= 1)
                continue;
            
            var cells = charCells.Value;
            for (int i = 0; i < cells.Count; i++)
            {
                var cellA = cells[i];
                for (int j = i + 1; j < cells.Count; j++)
                {
                    var cellB = cells[j];
                    var diff = cellB.Position - cellA.Position;

                    var cellCPos = cellB.Position + diff;
                    var cellDPos = cellA.Position - diff;
                    
                    var cellC = map[cellCPos.x, cellCPos.y];
                    var cellD = map[cellDPos.x, cellDPos.y];

                    if (cellC != null)
                        map.AddAntinode(cellCPos);
                    if (cellD != null)
                        map.AddAntinode(cellDPos);
                }
            }
        }

        return map.AntinodesCount;
    }

    private Map ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var width = lines[0].Length;
        var height = lines.Length;
        
        Cell[,] values = new Cell[width, height];
        for (var y = 0; y < height; y++)
        {
            var line = lines[y];
            for (var x = 0; x < width; x++)
            {
                values[x, y] = new Cell(x, y, line[x]);
            }
        }

        return new Map(values);
    }

    protected override int ExecuteTask2(string input)
    {
        var map = ParseInput(input);

        foreach (var charCells in map.CellsByChar)
        {
            if (charCells.Value.Count <= 1)
                continue;
            
            var cells = charCells.Value;
            for (int i = 0; i < cells.Count; i++)
            {
                var cellA = cells[i];
                for (int j = i + 1; j < cells.Count; j++)
                {
                    var cellB = cells[j];
                    var diff = cellB.Position - cellA.Position;

                    var forward = cellB.Position;
                    var forwardCell = map[forward.x, forward.y];
                    while (forwardCell != null)
                    {
                        map.AddAntinode(forwardCell.Position);
                        forward += diff;
                        forwardCell = map[forward.x, forward.y];
                    }
                    
                    var backward = cellA.Position;
                    var backwardCell = map[backward.x, backward.y];
                    while (backwardCell != null)
                    {
                        map.AddAntinode(backwardCell.Position);
                        backward -= diff;
                        backwardCell = map[backward.x, backward.y];
                    }
                }
            }
        }
        
        return map.AntinodesCount;
    }
}