// Definitely not proud of this one. I should have started over for
// task 2 and come up with a proper structure instead of adapting
// day 1's

using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day15 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    private class Cell
    {
        
    }

    private abstract class Entity
    {
        protected Map m_Map;

        protected Vector2Int m_Position;
        public Vector2Int Position
        {
            get => m_Position;
            set => m_Position = value;
        }

        private Vector2Int m_MoveIntent;
        public Vector2Int MoveIntent => m_MoveIntent;


        public virtual int Width { get; } = 1;
        public abstract bool CanMove { get; }
        
        public Entity(Map map, Vector2Int position)
        {
            m_Position = position;
            m_Map = map;
        }

        public static bool TryParse(char c, int x, int y, Map m, out Entity entity)
        {
            entity = default;
            
            switch (c)
            {
                case '#':
                    entity = new Wall(m, new Vector2Int(x, y));
                    break;
                case 'O':
                    entity = new Box(m, new Vector2Int(x, y));
                    break;
                case '@':
                    entity = new Lanternfish(m, new Vector2Int(x, y));
                    break;
            }

            return entity != default;
        }

        public bool TryMove(Vector2Int move)
        {
            Vector2Int offset;
            if (move.x == 1)
                offset = new Vector2Int(Width, move.y);
            else
                offset = move;

            int iterationCount = 1;
            if (Math.Abs(move.y) > 0)
                iterationCount = Width;
            
            HashSet<Entity> neighbours = new HashSet<Entity>();
            for (int i = 0; i < iterationCount; i++)
            {
                var neighbourPos = m_Position + new Vector2Int(i, 0) + offset;
                if (m_Map.TryGetEntity(neighbourPos, out var neighbour) && neighbour != this)
                    neighbours.Add(neighbour);
            }
            
            if (neighbours.Count == 0)
            {
                IntentMoveTo(m_Position + move);
                return true;
            }

            foreach (var neighbour in neighbours)
            {
                if (!neighbour.CanMove) 
                    return false;    
                
                if (neighbour.CanMove && neighbour.TryMove(move))
                {
                    IntentMoveTo(m_Position + move);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void IntentMoveTo(Vector2Int neighbourPos)
        {
            m_Map.NotifyMoveIntent(this);
            m_MoveIntent = neighbourPos;
        }

        public void AcknowledgeMove()
        {
            m_Position = m_MoveIntent;
        }
    }

    private class Lanternfish : Entity
    {
        public Lanternfish(Map map, Vector2Int position) : base(map, position)
        {
        }

        public override bool CanMove => true;

        public override string ToString()
        {
            return "@";
        }
    }

    private class Box : Entity
    {
        private int m_Width = 1;
        public override int Width => m_Width;

        public Box(Map map, Vector2Int position) : base(map, position)
        {
        }
        
        public override bool CanMove => true;
        
        public override string ToString()
        {
            if (m_Width == 2)
                return "[]";
            
            return "O";
        }

        public void DoubleWidth()
        {
            m_Position.x *= 2;
            m_Width *= 2;
        }
    }

    private class Wall : Entity
    {
        public Wall(Map map, Vector2Int position) : base(map, position)
        {
        }

        public override bool CanMove => false;
        
        public override string ToString()
        {
            return "#";
        }
    }
    
    private class Map : Vector2Map<Cell>
    {
        private Dictionary<Vector2Int, Entity> m_Entities;
        private Lanternfish m_Fish;
        private HashSet<Entity> m_DirtyEntitiesHelper;
        
        public Map(int width, int height) : base(width, height)
        {
            m_Entities = new Dictionary<Vector2Int, Entity>();
            m_DirtyEntitiesHelper = new HashSet<Entity>();
        }

        public void DoubleWidth()
        {
            var newMap = new Cell[Width * 2, Height];
            
            Dictionary<Vector2Int, Entity> entities = new Dictionary<Vector2Int, Entity>();
            foreach (var entityKvp in m_Entities)
            {
                var newPosition = new Vector2Int(entityKvp.Key.x * 2, entityKvp.Key.y);
                if (entityKvp.Value is Box box)
                {
                    box.DoubleWidth();
                    entities.Add(newPosition, box);
                    entities.Add(newPosition + new Vector2Int(1, 0), box);
                }
                else if (entityKvp.Value is Wall)
                {
                    var wall0 = new Wall(this, newPosition + new Vector2Int(0, 0));
                    var wall1 = new Wall(this, newPosition + new Vector2Int(1, 0));

                    entities.Add(wall0.Position, wall0);
                    entities.Add(wall1.Position, wall1);
                }
                else if (entityKvp.Value is Lanternfish fish)
                {
                    fish.Position = newPosition;
                    entities.Add(fish.Position, fish);
                }
            }

            m_Entities = entities;
            m_Map = newMap;
        }

        public void AddEntity(Entity e)
        {
            if (!m_Entities.TryAdd(e.Position, e))
                throw new Exception($"Entity at position {e.Position} already added");
            
            if (e is Lanternfish lanternfish)
            {
                if (m_Fish != null)
                    throw new Exception($"Lanternfish already added");
                
                m_Fish = lanternfish;
            }
        }

        public bool TryGetEntity(Vector2Int position, out Entity e)
        {
            return m_Entities.TryGetValue(position, out e);
        }

        public void MoveFish(Vector2Int move)
        {
            m_DirtyEntitiesHelper.Clear();
            if (!m_Fish.TryMove(move))
                return;
            
            foreach (var entity in m_DirtyEntitiesHelper)
            {
                for (int x = 0; x < entity.Width; x++)
                    m_Entities.Remove(entity.Position + new Vector2Int(x, 0));
            }
            
            foreach (var entity in m_DirtyEntitiesHelper)
            {
                for (int x = 0; x < entity.Width; x++)
                    m_Entities.Add(entity.MoveIntent + new Vector2Int(x, 0), entity);
                entity.AcknowledgeMove();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; )
                {
                    if (m_Entities.TryGetValue(new Vector2Int(x, y), out Entity e))
                    {
                        sb.Append(e);
                        x += e.Width;
                    }
                    else
                    {
                        sb.Append('.');
                        x++;
                    }
                        
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void NotifyMoveIntent(Entity entity)
        {
            m_DirtyEntitiesHelper.Add(entity);
        }

        public long GPS()
        {
            long gps = 0;
            foreach (var entity in m_Entities.Values)
            {
                if (entity is not Box)
                    continue;
                
                gps += entity.Position.y * 100 + entity.Position.x;
            }

            return gps;
        }
    }
    
    protected override long ExecuteTask1(string input)
    {
        ParseInput(input, out var map, out var moveList);
        for (int i = 0; i < moveList.Count; i++)
        {
            var move = moveList[i];
            map.MoveFish(move);
            // Console.WriteLine(map);
            // Console.ReadLine();
        }
        return map.GPS();
    }

    private void ParseInput(string input, out Map map, out List<Vector2Int> moveList)
    {
        var splitInput = input.Split("\n\n");
        
        var mapRaw = splitInput[0].Split("\n");
        var mapHeight = mapRaw.Length;
        var mapWidth = mapRaw[0].Length;
        map = new Map(mapWidth, mapHeight);
        for (var y = 0; y < mapHeight; y++)
        {
            var line = mapRaw[y];
            for (var x = 0; x < mapWidth; x++)
            {
                if (Entity.TryParse(line[x], x, y, map, out var entity))
                    map.AddEntity(entity);
            }
        }

        moveList = new List<Vector2Int>();
        var movesRaw = splitInput[1];
        foreach (var move in movesRaw)
        {
            switch (move)
            {
                case '<':
                    moveList.Add(new Vector2Int(-1, 0));
                    break;
                case '>':
                    moveList.Add(new Vector2Int(1, 0));
                    break;
                case '^':
                    moveList.Add(new Vector2Int(0, -1));
                    break;
                case 'v':
                    moveList.Add(new Vector2Int(0, 1));
                    break;
            }
        }
    }

    private static string Vector2IntToStr(Vector2Int v2)
    {
        if (v2.x == -1)
            return "<";
        if (v2.x == 1)
            return ">";
        if (v2.y == -1)
            return "^";
        if (v2.y == 1)
            return "v";

        return "?";
    }

    protected override long ExecuteTask2(string input)
    {
        ParseInput(input, out var map, out var moveList);
        map.DoubleWidth();
        // Console.WriteLine(map);
        for (int i = 0; i < moveList.Count; i++)
        {
            var move = moveList[i];
            // Console.WriteLine("Moving " + Vector2IntToStr(move));
            // Console.ReadLine();
            map.MoveFish(move);
            // Console.WriteLine(map);
        }
        return map.GPS() / 2;
    }
}