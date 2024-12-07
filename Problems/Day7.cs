namespace advent_2024.problems;

public class Day7 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    protected override long ExecuteTask1(string input)
    {
        long result = 0;
        var equations = ParseInput(input);
        foreach (var equation in equations)
        {
            if (equation.IsValid(false))
                result += equation.Value;
        }

        return result;
    }

    protected override long ExecuteTask2(string input)
    {
        long result = 0;
        var equations = ParseInput(input);
        foreach (var equation in equations)
        {
            if (equation.IsValid(true))
                result += equation.Value;
        }

        return result;
    }

    private enum Operator
    {
        Add,
        Mult,
        Concat,
    }

    private static long Operate(Operator op, long val1, long val2)
    {
        switch (op)
        {
            case Operator.Add:
                return val1 + val2;
            case Operator.Mult:
                return val1 * val2;
            case Operator.Concat:
                return long.Parse(val1.ToString() + val2.ToString());
            default:
                throw new ArgumentOutOfRangeException(nameof(op), op, null);
        }
    }
    
    private class Equation
    {
        private long m_Result;
        private List<long> m_Operands;
        public long Value => m_Result;

        public Equation(long result, List<long> operands)
        {
            m_Result = result;
            m_Operands = operands;
        }

        public bool IsValid(bool includeConcat)
        {
            return IsValid(0, 0, includeConcat);
        }

        private bool IsValid(long result, int operandIndex, bool includeConcat)
        {
            if (operandIndex >= m_Operands.Count)
                return result == m_Result;

            bool isValid = false;
            foreach (var op in Enum.GetValues<Operator>())
            {
                if (!includeConcat && op == Operator.Concat)
                    continue;
                
                var temp = Operate(op, result, m_Operands[operandIndex]);
                isValid |= IsValid(temp, operandIndex + 1, includeConcat);
            }
            
            return isValid;
        }
    }
    
    private List<Equation> ParseInput(string input)
    {
        List<Equation> equations = new List<Equation>();
        var lines = input.Split('\n');
        foreach (var line in lines)
        {
            var columnIndex = line.IndexOf(':');
            var result = long.Parse(line.Substring(0, columnIndex));
            var operandsRaw = line.Substring(columnIndex + 1).Trim();
            var operands = operandsRaw.Split(' ').Select(long.Parse).ToList();
            equations.Add(new Equation(result, operands));
        }

        return equations;
    }
}