using System.Text.RegularExpressions;
using Google.OrTools.LinearSolver;

namespace advent_2024.problems;

public class Day13 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    private class Machine
    {
        private Button m_ButtonA;
        public Button ButtonA => m_ButtonA;
        
        private Button m_ButtonB;
        public Button ButtonB => m_ButtonB;

        private long m_PrizeX;
        public long PrizeX => m_PrizeX;
        
        private long m_PrizeY;
        public long PrizeY => m_PrizeY;

        public Machine(Button buttonA, Button buttonB, string str)
        {
            m_ButtonA = buttonA;
            m_ButtonB = buttonB;
            
            var xRegex = @"X\=([0-9]+)";
            var yRegex = @"Y\=([0-9]+)";

            m_PrizeX = int.Parse(Regex.Match(str, xRegex).Groups[1].Value);
            m_PrizeY = int.Parse(Regex.Match(str, yRegex).Groups[1].Value);
        }

        public void AddOffset(long offset)
        {
            m_PrizeX += offset;
            m_PrizeY += offset;
        }

        public override string ToString() => $"({m_PrizeX},{m_PrizeY}) {m_ButtonA} {m_ButtonB}";

        public bool Solve(out long aButtonPresses, out long bButtonPresses, out long tokensRequired)
        {
            var solver = Solver.CreateSolver("SCIP");

            var a = solver.MakeIntVar(0.0, double.PositiveInfinity, "a");
            var b = solver.MakeIntVar(0.0, double.PositiveInfinity, "b");

            solver.Add(m_ButtonA.X * a + m_ButtonB.X * b == m_PrizeX); 
            solver.Add(m_ButtonA.Y * a + m_ButtonB.Y * b == m_PrizeY);

            var objective = solver.Objective();
            objective.SetCoefficient(a, m_ButtonA.Cost);
            objective.SetCoefficient(b, m_ButtonB.Cost);
            objective.SetMinimization();

            var resultStatus = solver.Solve();
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                aButtonPresses = 0;
                bButtonPresses = 0;
                tokensRequired = 0;
                return false;
            }

            aButtonPresses = (long)a.SolutionValue();
            bButtonPresses = (long)b.SolutionValue();
            tokensRequired = aButtonPresses * m_ButtonA.Cost + bButtonPresses * m_ButtonB.Cost;
            return true;
        }
    }

    private class Button
    {
        private int m_X;
        public int X => m_X;
        private int m_Y;
        public int Y => m_Y;
        
        private int m_Cost;
        public int Cost => m_Cost;

        public Button(string str, int cost)
        {
            var xRegex = @"X\+([0-9]+)";
            var yRegex = @"Y\+([0-9]+)";

            m_X = int.Parse(Regex.Match(str, xRegex).Groups[1].Value);
            m_Y = int.Parse(Regex.Match(str, yRegex).Groups[1].Value);
            m_Cost = cost;
        }

        public override string ToString() => $"({m_X},{m_Y}) {m_Cost}$";
    }
    
    protected override long ExecuteTask1(string input)
    {
        var machines = ParseInput(input);
        long totalTokensRequired = 0;
        foreach (var m in machines)
        {
            if (m.Solve(out var aPresses, out var bPresses, out var tokensRequired))
            {
                totalTokensRequired += tokensRequired;
            }
        }

        return totalTokensRequired;
    }
    
    private List<Machine> ParseInput(string input)
    {
        var res = new List<Machine>();
        var lines = input.Split('\n');
        for (var i = 0; i < lines.Length; i += 4)
        {
            var buttonA = new Button(lines[i], 3);
            var buttonB = new Button(lines[i+1], 1);
            var machine = new Machine(buttonA, buttonB, lines[i + 2]);
            res.Add(machine);
        }

        return res;
    }

    protected override long ExecuteTask2(string input)
    {
        var machines = ParseInput(input);
        long totalTokensRequired = 0;
        foreach (var m in machines)
        {
            m.AddOffset(10000000000000);
            if (m.Solve(out var aPresses, out var bPresses, out var tokensRequired))
            {
                totalTokensRequired += tokensRequired;
            }
        }

        return totalTokensRequired;
    }
}