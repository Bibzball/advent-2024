using System.Text;
using System.Text.RegularExpressions;

namespace advent_2024.problems;

public class Day17 : Day<string, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;

    private abstract class Instruction
    {
        public abstract string Execute(Program p, long operand);
        public abstract string ToString(Program p, long operand);
    }

    private class Adv : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            p.RegisterA = numerator / denonimator;
            return null;
        }

        public override string ToString(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            return $"{numerator}/{denonimator}={numerator / denonimator} - Setting Register A";
        }
    }

    private class Bxl : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            p.RegisterB ^= operand;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            return $"{p.RegisterB}^{operand}={p.RegisterB^operand} - Setting Register B";
        }
    }

    private class Bst : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            p.RegisterB = p.ToComboOperand(operand) % 8;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            return $"{p.ToComboOperand(operand)}%8={p.ToComboOperand(operand) % 8} - Setting Register B";
        }
    }
    
    private class Jnz : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            if (p.RegisterA == 0)
                return null;

            p.InstructionPointer = operand - 2;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            if (p.RegisterA == 0)
                return "Exiting";

            return $"Setting P to {operand}";
        }
    }
    
    private class Bxc : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            p.RegisterB ^= p.RegisterC;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            return $"{p.RegisterB}^{p.RegisterC}={p.RegisterB^p.RegisterC} - Setting Register B";
        }
    }
    
    private class Out : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            return (p.ToComboOperand(operand) % 8).ToString();
        }
        
        public override string ToString(Program p, long operand)
        {
            return $"Outputting {(p.ToComboOperand(operand) % 8).ToString()}";
        }
    }
    
    private class Bdv : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            p.RegisterB = numerator / denonimator;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            return $"{numerator}/{denonimator}={numerator / denonimator} - Setting Register B";
        }
    }
    
    private class Cdv : Instruction
    {
        public override string Execute(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            p.RegisterC = numerator / denonimator;
            return null;
        }
        
        public override string ToString(Program p, long operand)
        {
            var numerator = p.RegisterA;
            var denonimator = (long)Math.Pow(2, p.ToComboOperand(operand));
            return $"{numerator}/{denonimator}={numerator / denonimator} - Setting Register C";
        }
    }

    private class Program
    {
        private static Dictionary<int, Instruction> s_Instructions = new Dictionary<int, Instruction>()
        {
            {0, new Adv()},
            {1, new Bxl()},
            {2, new Bst()},
            {3, new Jnz()},
            {4, new Bxc()},
            {5, new Out()},
            {6, new Bdv()},
            {7, new Cdv()},
        };
        
        public long RegisterA;
        public long RegisterB;
        public long RegisterC;

        public long InstructionPointer = 0;

        private int[] m_instructions;

        public long ToComboOperand(long i)
        {
            if (i <= 3)
                return i;

            if (i == 4)
                return RegisterA;

            if (i == 5)
                return RegisterB;

            if (i == 6)
                return RegisterC;

            throw new Exception("Unexpected operand");
        }

        public Program(long regA, long regB, long regC, int[] instructions)
        {
            RegisterA = regA;
            RegisterB = regB;
            RegisterC = regC;

            m_instructions = instructions;
        }

        public List<string> Run()
        {
            // Console.WriteLine(ToString());

            List<string> output = new List<string>();
            while (InstructionPointer < m_instructions.Length)
            {
                var instruction = m_instructions[InstructionPointer];
                var operand = m_instructions[InstructionPointer + 1];
                var instructionInstance = s_Instructions[instruction];
                // Console.WriteLine(instructionInstance.ToString(this, operand));
                var candidate = instructionInstance.Execute(this, operand);
                // Console.WriteLine(ToString());
                if (candidate != null)
                    output.Add(candidate);
                InstructionPointer += 2;
            }

            return output;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"A={RegisterA}");
            sb.AppendLine($"B={RegisterB}");
            sb.AppendLine($"C={RegisterC}");
            sb.AppendLine($"P={InstructionPointer}");
            return sb.ToString();
        }
    }
    
    protected override string ExecuteTask1(string input)
    {
        var program = ParseInput(input);
        return String.Join(',',program.Run());
    }
    
    private Program ParseInput(string input)
    {
        var lines = input.Split('\n');
        
        long registerA = long.Parse(lines[0].Substring(12).Trim());
        long registerB = long.Parse(lines[1].Substring(12).Trim());
        long registerC = long.Parse(lines[2].Substring(12).Trim());

        string program = lines[4].Substring(9);
        var strInstructions = program.Split(',');
        List<int> intInstructions = new List<int>();
        foreach (var instruction in strInstructions)
            intInstructions.Add(int.Parse(instruction));

        return new Program(registerA, registerB, registerC, intInstructions.ToArray());
    }

    protected override long ExecuteTask2(string input)
    {
        int[] instructions = new int[] { 2, 4, 1, 7, 7, 5, 1, 7, 4, 6, 0, 3, 5, 5, 3, 0 };
        long a = 0;
        Program p;
        
        for (int i = instructions.Length - 1; i >= 0; i--)
        {
            var instructionsCopy = new List<int>();
            for (int j = i; j < instructions.Length; j++)
                instructionsCopy.Add(instructions[j]);

            var resultExpected = String.Join(',', instructionsCopy);
            p = new Program(a, 0, 0, instructions);
            while (resultExpected != String.Join(',', p.Run()))
            {
                a++;
                p = new Program(a, 0, 0, instructions);
            }

            // Console.WriteLine(a);
            a <<= 3;
        }
        
        return a;
    }
}