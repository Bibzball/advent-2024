using System.Text;
using advent_2024.Framework.Logger;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day6 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override int ExecuteTask1(string input)
    {
        var map = ParseMap(input);
        int timeout = 10000;
        while (!map.ProcessStep())
        {
            timeout--;
            if (timeout == 0)
                break;
        }
        
        return map.VisitedCount;
    }
    
    
    protected override int ExecuteTask2(string input)
    {
        Logging.DebugLevel = Logging.Level.Info;
        
        int result = 0;
        
        var originalMap = ParseMap(input);
        for (int x = 0; x < originalMap.Width; x++)
        {
            for (int y = 0; y < originalMap.Height; y++)
            {
                if (originalMap[x, y].State == CellState.Empty)
                {
                    var copy = originalMap.CopyWithObstacle(x, y);
                    int timeout = 10000;
                    while (true)
                    {
                        var loopResult = copy.ProcessStepAndCheckLoop();
                        if (loopResult == Map.ProgressState.GuardExit)
                        {
                            Logging.LogDebug("Task 2", $"Guard exited {x}, {y}");
                            break;
                        }

                        if (loopResult == Map.ProgressState.LoopDetected)
                        {
                            Logging.LogInfo("Task 2", $"Found loop with {x}, {y}");
                            result++;
                            break;
                        }
                        
                        timeout--;
                        if (timeout == 0)
                        {
                            Logging.LogError("Task 2", $"Timeout for {x}, {y}");
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }

    private enum CellState
    {
        Empty,
        Guard,
        Visited,
        Obstacle,
    }

    private class Cell
    {
        public CellState State => m_State;
        private CellState m_State;
        private List<Vector2Direction> m_DirectionsVisited = new List<Vector2Direction>();

        public Cell(CellState state)
        {
            m_State = state;
        }

        public bool VisitCell()
        {
            if (m_State == CellState.Visited)
                return false;

            m_State = CellState.Visited;
            return true;
        }
        
        public bool VisitCellAndCheckLoop(Vector2Direction direction)
        {
            m_State = CellState.Visited;
            if (m_DirectionsVisited.Contains(direction))
                return true;

            m_DirectionsVisited.Add(direction);
            return false;
        }

        public override string ToString()
        {
            switch (State)
            {
                case CellState.Guard:
                    return "^";
                case CellState.Visited:
                    return "X";
                case CellState.Obstacle:
                    return "#";
                case CellState.Empty:
                    return ".";
            }

            return "?";
        }
    }

    private class Map : Vector2Map<Cell>
    {
        public Map CopyWithObstacle(int xObs, int yObs)
        {
            Map copy = new Map(this.Width, this.Height);
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    copy.InitialiseCell(new Vector2Int(x, y), this[x,y].State);
                }
            }
            
            copy.InitialiseCell(new Vector2Int(xObs, yObs), CellState.Obstacle);

            return copy;
        }
        
        public enum ProgressState
        {
            InProgress,
            GuardExit,
            LoopDetected
        }
        
        public Vector2Direction GuardDirection => m_GuardDirection;
        private Vector2Direction m_GuardDirection;
        private Vector2Int m_GuardPosition;
        private int m_VisitedCount;
        public int VisitedCount => m_VisitedCount;
        
        public Map(int width, int height) : base(width, height)
        {
        }

        public void InitialiseCell(Vector2Int position, CellState state)
        {
            this[position] = new Cell(state);
            if (state == CellState.Guard)
            {
                m_GuardPosition = position;
                m_GuardDirection = Vector2Direction.N;
            }
        }

        public bool ProcessStep()
        {
            var cell = this[m_GuardPosition];
            var neighbour = GetNeighbour(m_GuardPosition, m_GuardDirection);
            if (neighbour == null)
            {
                // It's over!
                if (cell.VisitCell())
                {
                    m_VisitedCount++;
                }
                return true;
            }

            if (neighbour.State == CellState.Obstacle)
            {
                m_GuardDirection = m_GuardDirection.TurnClockwise();
                return false;
            }

            if (cell.VisitCell())
            {
                m_VisitedCount++;
            }
            m_GuardPosition += m_GuardDirection.ToVector2Int();
            return false;
        }
        
        public ProgressState ProcessStepAndCheckLoop()
        {
            var cell = this[m_GuardPosition];
            var neighbour = GetNeighbour(m_GuardPosition, m_GuardDirection);
            if (neighbour == null)
            {
                return ProgressState.GuardExit;
            }

            if (neighbour.State == CellState.Obstacle)
            {
                m_GuardDirection = m_GuardDirection.TurnClockwise();
                return ProgressState.InProgress;
            }

            if (cell.VisitCellAndCheckLoop(m_GuardDirection))
            {
                return ProgressState.LoopDetected;
            }
            
            m_GuardPosition += m_GuardDirection.ToVector2Int();
            return ProgressState.InProgress;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(this[x, y]);
                }     
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    
    private Map ParseMap(string input)
    {
        var raw = input.Split("\n");
        
        var width = raw[0].Length;
        var height = raw.Length;
        
        var map = new Map(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellState state;
                switch (raw[y][x])
                {
                    case '.': state = CellState.Empty; break;
                    case '^': state = CellState.Guard; break;
                    case '#': state = CellState.Obstacle; break;
                    default: throw new FormatException($"Invalid cell state: {raw[y][x]}");
                }
                map.InitialiseCell(new Vector2Int(x, y), state);
            }
        }

        return map;
    }
}