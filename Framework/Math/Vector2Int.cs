namespace advent_2024.Framework.Math;

public class Vector2Int : IEquatable<Vector2Int>
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2Int operator + (Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }
    
    public static Vector2Int operator - (Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator *(Vector2Int a, int b)
    {
        return new Vector2Int(a.x * b, a.y * b);
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return System.Math.Abs(b.x - a.x) + System.Math.Abs(b.y - a.y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public bool Equals(Vector2Int? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return x == other.x && y == other.y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Vector2Int)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}