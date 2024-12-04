namespace advent_2024.problems;

public class Day4 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override int ExecuteTask1(string input)
    {
        int result = 0;
        var table = ParseInput(input);
        for (int x = 0; x < table.Width; x++)
        {
            for (int y = 0; y < table.Height; y++)
            {
                var item = table.Get(x, y);
                result += TryPatternFind(item);
            }
        }

        return result;
    }
    
    protected override int ExecuteTask2(string input)
    {
        var result = 0;
        var table = ParseInput(input);
        for (int x = 0; x < table.Width; x++)
        {
            for (int y = 0; y < table.Height; y++)
            {
                var item = table.Get(x, y);
                result += FindMASCross(item);
            }
        }

        return result;
    }

    private enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    private static void GetDirectionIndex(int x, int y, Direction direction, out int newX, out int newY)
    {
        switch (direction)
        {
            case Direction.N:
                newX = x;
                newY = y - 1;
                break;
            case Direction.NE:
                newX = x + 1;
                newY = y - 1;
                break;
            case Direction.E:
                newX = x + 1;
                newY = y;
                break;
            case Direction.SE:
                newX = x + 1;
                newY = y + 1;
                break;
            case Direction.S:
                newX = x;
                newY = y + 1;
                break;
            case Direction.SW:
                newX = x - 1;
                newY = y + 1;
                break;
            case Direction.W:
                newX = x - 1;
                newY = y;
                break;
            case Direction.NW:
                newX = x - 1;
                newY = y - 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    private const string c_PatternToFind = "XMAS";
    
    private class TableItem
    {
        private Dictionary<Direction, TableItem?> m_Neighbours;

        private char m_Value;
        public char Value => m_Value;
        
        public TableItem(char value)
        {
            m_Value = value;
            m_Neighbours = new Dictionary<Direction, TableItem?>();
        }

        public void AddNeighbour(Direction direction, TableItem item)
        {
            if (m_Neighbours.ContainsKey(direction))
                throw new Exception($"Neighbour {direction} is already added");
            
            m_Neighbours[direction] = item;
        }

        public TableItem? GetNeighbour(Direction direction)
        {
            return m_Neighbours.GetValueOrDefault(direction, null);
        }
    }

    private class Table
    {
        private TableItem[,] m_Items;
        
        public int Width => m_Items.GetLength(0);
        public int Height => m_Items.GetLength(1);

        public Table(int width, int height)
        {
            m_Items = new TableItem[width, height];
        }

        public TableItem Get(int x, int y)
        {
            return m_Items[x, y];
        }

        public TableItem? GetNeighbour(int x, int y, Direction direction)
        {
            GetDirectionIndex(x, y, direction, out var nX, out var nY);
            try
            {
                return m_Items[nX, nY];
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }
        }

        public void Add(TableItem item, int x, int y)
        {
            m_Items[x, y] = item;
        }
    }

    private Table ParseInput(string input)
    {
        var rows = input.Split("\n");
        
        var width = rows[0].Length;
        var height = rows.Length;
        
        var table = new Table(width, height);
        for (int y = 0; y < height; y++)
        {
            var row = rows[y];
            for (int x = 0; x < width; x++)
            {
                table.Add(new TableItem(row[x]), x, y);
            }
        }

        for (int x = 0; x < table.Width; x++)
        {
            for (int y = 0; y < table.Height; y++)
            {
                var item = table.Get(x, y);
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    var neighbour = table.GetNeighbour(x, y, direction);
                    if (neighbour == null)
                        continue;

                    item.AddNeighbour(direction, neighbour);
                }
            }
        }
        return table;
    }
    
    private int TryPatternFind(TableItem item)
    {
        if (item.Value != c_PatternToFind[0])
            return 0;
        
        int sum = 0;
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            var neighbour = item.GetNeighbour(direction);
            if (neighbour == null)
                continue;
            
            sum += TryPatternFind(neighbour, 1, direction);
        }

        return sum;
    }

    private int TryPatternFind(TableItem item, int charIndex, Direction direction)
    {
        if (item.Value != c_PatternToFind[charIndex])
            return 0;

        if (charIndex == c_PatternToFind.Length - 1)
            return 1;
        
        var neighbour = item.GetNeighbour(direction);
        if (neighbour == null)
            return 0;
        
        return TryPatternFind(neighbour, charIndex + 1, direction);
    }
    
    private int FindMASCross(TableItem item)
    {
        if (item.Value != 'A')
            return 0;
                
        var neNeighbour = item.GetNeighbour(Direction.NE);
        var nwNeighbour = item.GetNeighbour(Direction.NW);
        var swNeighbour = item.GetNeighbour(Direction.SW);
        var seNeighbour = item.GetNeighbour(Direction.SE);
        if (neNeighbour == null || nwNeighbour == null || seNeighbour == null || swNeighbour == null)
            return 0;

        int masCount = 0;
        if (neNeighbour.Value == 'M' && swNeighbour.Value == 'S')
            masCount++;
        if (seNeighbour.Value == 'M' && nwNeighbour.Value == 'S')
            masCount++;
        if (swNeighbour.Value == 'M' && neNeighbour.Value == 'S')
            masCount++;
        if (nwNeighbour.Value == 'M' && seNeighbour.Value == 'S')
            masCount++;

        if (masCount >= 2)
            return 1;

        return 0;
    }
}