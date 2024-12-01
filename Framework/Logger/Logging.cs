namespace advent_2024.Framework.Logger;

public class Logging
{
    public enum Level
    {
        Error,
        Warning,
        Info,
        Debug
    }
    
    public static Level DebugLevel { get; set; } = Level.Debug;
    
    public static void LogError(string header, string message)
    {
        _Log(header, message, Level.Error);
    }

    public static void LogWarning(string header, string message)
    {
        _Log(header, message, Level.Warning);
    }

    public static void LogInfo(string header, string message)
    {
        _Log(header, message, Level.Info);
    }

    public static void LogDebug(string header, string message)
    {
        _Log(header, message, Level.Debug);
    }

    private static void _Log(string header, string message, Level level)
    {
        if (level > DebugLevel)
            return;
        
        string formattedMessage = $"[{header}] {message}";
        if (level == Level.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(formattedMessage);
            return;
        }
        
        Console.WriteLine(formattedMessage);
    }
}