namespace advent_2024.problems
{
    public abstract class Day<T1, T2>
    {
        protected enum TaskState
        {
            Test,
            Final,
        }
        
        protected abstract TaskState Task1State { get; }
        protected abstract TaskState Task2State { get; }

        delegate T TaskMethod<T>(string input);

        public T1 Task1()
        {
            return Task(Task1State, ExecuteTask1);
        }
        
        public T2 Task2()
        {
            return Task(Task2State, ExecuteTask2);
        }

        private T Task<T>(TaskState state, TaskMethod<T> method)
        {
            string input;
            switch (state)
            {
                case TaskState.Test:
                    input = ReadTestInput();
                    break;
                case TaskState.Final:
                    input = ReadProblemInput();
                    break;
                default:
                    throw new Exception("Invalid task state");
            }

            return method(input);
        }
        
        protected abstract T1 ExecuteTask1(string input);
        protected abstract T2 ExecuteTask2(string input);
        
        private string ReadFile(string path)
        {
            var task = File.ReadAllTextAsync(path);
            task.Wait();
            return task.Result;
        }
        
        protected string ReadTestInput()
        {
            return ReadFile($"Files/{GetType().Name}.test.txt");
        }

        protected string ReadProblemInput()
        {
            return ReadFile($"Files/{GetType().Name}.txt");
        }
    }
}
