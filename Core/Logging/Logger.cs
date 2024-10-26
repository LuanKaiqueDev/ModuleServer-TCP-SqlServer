using System.Runtime.CompilerServices;

namespace Core.Logging;

public static class Logger
{
    private const string Reset = "\u001b[0m";
    private const string Green = "\u001b[32m"; 
    private const string Yellow = "\u001b[33m"; 
    private const string White = "\u001b[37m"; 
    private const string Red = "\u001b[31m"; 

    public static void Information(string message,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "")
    {
        string className = GetClassName(filePath);
        CreateColoredFullFormat(LogType.Info, className, method, message);
    }

       
    public static void Error(string message,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "")
    {
        string className = GetClassName(filePath);
        CreateColoredFullFormat(LogType.Error, className, method, message);
    }

    private static void CreateColoredFullFormat(LogType type, string className, string method, string message)
    {
        string coloredType = $"[{Red}Error{Reset}]"; 
        string coloredClassName = $"{Green}[{className}]{Reset}";
        string coloredMethod = $"{Yellow}[{method}]{Reset}"; 
        string coloredMessage = $"{White}{message}{Reset}";

        if (type == LogType.Error) 
            Console.WriteLine($"{coloredType}{coloredClassName}{coloredMethod}: {coloredMessage}");
        else
            Console.WriteLine($"{coloredClassName}{coloredMethod}: {coloredMessage}");
    }
    private static string GetClassName(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }

    private enum LogType
    {
        Info,
        Error,
    }
}