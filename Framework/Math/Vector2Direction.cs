namespace advent_2024.Framework.Math;

public enum Vector2Direction
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW,
}

public static class Vector2DirectionExtensions
{
    public static Vector2Int ToVector2Int(this Vector2Direction direction)
    {
        switch (direction)
        {
            case Vector2Direction.N:
                return new Vector2Int(0, -1);
            case Vector2Direction.NE:
                return new Vector2Int(1, -1);
            case Vector2Direction.E:
                return new Vector2Int(1, 0);
            case Vector2Direction.SE:
                return new Vector2Int(1, 1);
            case Vector2Direction.S:
                return new Vector2Int(0, 1);
            case Vector2Direction.SW:
                return new Vector2Int(-1, 1);
            case Vector2Direction.W:
                return new Vector2Int(-1, 0);
            case Vector2Direction.NW:
                return new Vector2Int(-1, -1);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Vector2Direction ToVector2Direction(this Vector2Int direction)
    {
        if (direction.x == 0)
        {
            if (direction.y == 0) throw new ArithmeticException($"Invalid direction {direction}");
            if (direction.y == 1) return Vector2Direction.S;
            if (direction.y == -1) return Vector2Direction.N;
        }
        else if (direction.x == -1)
        {
            if (direction.y == 0) return Vector2Direction.W;
            if (direction.y == 1) return Vector2Direction.SW;
            if (direction.y == -1) return Vector2Direction.NW;      
        }
        else if (direction.x == 1)
        {
            if (direction.y == 0) return Vector2Direction.E;
            if (direction.y == 1) return Vector2Direction.SE;
            if (direction.y == -1) return Vector2Direction.NE; 
        }
        
        throw new ArithmeticException($"Invalid direction {direction}");
    }

    public static Vector2Direction TurnClockwise(this Vector2Direction direction)
    {
        var vec = direction.ToVector2Int();
        vec = vec.Turn(MathF.PI / 2);
        return vec.ToVector2Direction();
    }

    private static Vector2Int Turn(this Vector2Int vec, float angle)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);
        
        return new Vector2Int((int)(cos * vec.x - sin * vec.y), (int)(sin * vec.x + cos * vec.y)); 
    }
}

public abstract class Vector2Map<T>
{
    private T[,] m_Map;
    
    public int Width => m_Map.GetLength(0);
    public int Height => m_Map.GetLength(1);
    
    public Vector2Map(int width, int height)
    {
        m_Map = new T[width, height];
    }

    public T this[int x, int y]
    {
        get => m_Map[x, y];
        set => m_Map[x, y] = value;
    }

    public T this[Vector2Int pos]
    {
        get => m_Map[pos.x, pos.y];
        set => m_Map[pos.x, pos.y] = value;
    }

    public T? GetNeighbour(int x, int y, Vector2Direction direction)
    {
        return GetNeighbour(new Vector2Int(x, y), direction);
    }

    public T? GetNeighbour(Vector2Int pos, Vector2Direction direction)
    {
        Vector2Int dirIncrement = direction.ToVector2Int();
        Vector2Int neighbour = pos + dirIncrement;

        try
        {
            return this[neighbour];
        }
        catch (IndexOutOfRangeException)
        {
            return default;
        }
    }
}