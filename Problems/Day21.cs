using System.Text;
using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day21 : Day<int, int>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Test;

    private class Key
    {
        public char Value;
        public Vector2Int Position;
    }
    
    private class Keypad : Vector2Map<Key?>
    {
        private Dictionary<char, Key> m_keys = new Dictionary<char, Key>();

        private Vector2Int m_currentPosition;
        private Vector2Int m_startPosition;
        
        public Keypad(char?[,] keys) : base(keys.GetLength(1), keys.GetLength(0))
        {
            for (var y = 0; y < keys.GetLength(0); y++)
            {
                for (var x = 0; x < keys.GetLength(1); x++)
                {
                    var c = keys[y, x];
                    if (c == null)
                        continue;
                    
                    var position = new Vector2Int(x, y);

                    if (c == 'A')
                    {
                        m_startPosition = position;
                        m_currentPosition = m_startPosition;
                    }
                    
                    var key = new Key
                    {
                        Value = c.Value,
                        Position = position,
                    };
                    m_Map[x, y] = key;
                    m_keys[c.Value] = key;
                }
            }
        }

        public string GoTo(char value)
        {
            StringBuilder res = new StringBuilder();
            var targetPos = m_keys[value].Position;
            var diff = targetPos - m_currentPosition;

            bool reverse = false;
            int tempX = m_currentPosition.x;
            int tempY = m_currentPosition.y;
            while (tempX != targetPos.x)
            {
                tempX += Math.Sign(diff.x);
                if (m_Map[tempX, tempY] == null)
                {
                    reverse = true;
                    break;
                }
            }

            if (!reverse)
            {
                while (tempY != targetPos.y)
                {
                    tempY += Math.Sign(diff.y);
                    if (m_Map[tempX, tempY] == null)
                    {
                        reverse = true;
                        break;
                    }
                }
            }

            if (!reverse)
            {
                for (int x = 0; x < Math.Abs(diff.x); x++)
                    res.Append(diff.x > 0 ? '>' : '<');
                for (int y = 0; y < Math.Abs(diff.y); y++)
                    res.Append(diff.y > 0 ? 'v' : '^');
            }
            else
            {
                for (int y = 0; y < Math.Abs(diff.y); y++)
                    res.Append(diff.y > 0 ? 'v' : '^');
                for (int x = 0; x < Math.Abs(diff.x); x++)
                    res.Append(diff.x > 0 ? '>' : '<');
            }

            
            res.Append('A');
            m_currentPosition = targetPos;
            return res.ToString();
        }

        public string Input(string input)
        {
            StringBuilder res = new StringBuilder();
            m_currentPosition = m_startPosition;
            foreach (var c in input)
            {
                res.Append(GoTo(c));
            }

            return res.ToString();
        }
    }
    
    protected override int ExecuteTask1(string input)
    {
        var lines = input.Split('\n');
        int value = 0;
        foreach (var line in lines)
        {
            int val = int.Parse(line.Substring(0, 3));
            string solve = Solve(line);
            value += val * solve.Length;
            Console.WriteLine($"{solve.Length} * {val}");
        }
        
        return value;
    }

    private string Solve(string input)
    {
        var mainKeyPad = new Keypad(new char?[,]
        {
            { '7',  '8', '9' }, 
            { '4',  '5', '6' },
            { '1',  '2', '3' },
            { null, '0', 'A' },
        });
        
        List<Keypad> keypads = new List<Keypad>();
        for (int i = 0; i < 2; i++)
        {
            keypads.Add(new Keypad(new char?[,]
            {
                { null,  '^', 'A' }, 
                { '<',  'v', '>' },
            }));
        }
        
        input = mainKeyPad.Input(input);
        foreach (var keypad in keypads)
        {
            input = keypad.Input(input);
        }

        return input;
    }

    protected override int ExecuteTask2(string input)
    {
        return 0;
    }
}