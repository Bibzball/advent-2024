namespace advent_2024.Framework.Math;

public abstract class Vector2Map<T>
{
    protected T[,] m_Map;
    
    public int Width => m_Map.GetLength(0);
    public int Height => m_Map.GetLength(1);
    
    public Vector2Map(int width, int height)
    {
        m_Map = new T[width, height];
    }

    public T? this[int x, int y]
    {
        get
        {
            try
            {
                return m_Map[x, y];
            }
            catch (IndexOutOfRangeException)
            {
                return default;
            }
        }
        set => m_Map[x, y] = value;
    }

    public T? this[Vector2Int pos]
    {
        get
        {
            try
            {
                return m_Map[pos.x, pos.y];
            }
            catch (IndexOutOfRangeException)
            {
                return default;
            }
        }
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
        return this[neighbour];
    }

    public T GetTiledNeighbour(Vector2Int pos, Vector2Int distance)
    {
        var x = (pos.x + distance.x) % Width;
        if (x < 0) x += Width;
        var y = (pos.y + distance.y) % Height;
        if (y < 0) y += Height;
        return m_Map[x, y];
    }
}