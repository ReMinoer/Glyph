using System;
using System.Diagnostics;
using System.IO;

namespace Glyph.Application
{
    static public class Log
    {
        static public bool ViewInConsole { get { return _viewInConsole; } set { _viewInConsole = value; } }
        static public bool UseRealTime { get { return _useRealTime; } set { _useRealTime = value; } }

        static public string OutputPath
        {
            get { return _outputPath; }
            set
            {
                _outputPath = value;
                _fileStream.Close();
                _fileStream = new StreamWriter(_outputPath) {AutoFlush = true};
            }
        }

        static private readonly Stopwatch ExecutionStopwatch = new Stopwatch();

        static private bool _viewInConsole = true;
        static private bool _useRealTime = false;
        static private string _outputPath = "log.txt";
        static private StreamWriter _fileStream = new StreamWriter("log.txt") {AutoFlush = true};

        static public void StartStopwatch()
        {
            ExecutionStopwatch.Start();
        }

        static public void Message(string message)
        {
            string formatedTime = UseRealTime
                                      ? DateTime.Now.ToString("HH:mm:ss")
                                      : ExecutionStopwatch.Elapsed.ToString(@"hh\:mm\:ss");

            string line = string.Format("[{0}] {1}", formatedTime, message);

            if (ViewInConsole)
                Console.WriteLine(line);

            _fileStream.WriteLine(line);
        }

        static public void Message(string message, ConsoleColor consoleColor)
        {
            if (ViewInConsole)
            {
                Console.ForegroundColor = consoleColor;
                Message(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
                Message(message);
        }

        static public void Message(string message, ConsoleColor consoleColor, string tag)
        {
            Message(string.Format("{1} {0}", message, tag), consoleColor);
        }

        static public void System(string message)
        {
            Message(message, ConsoleColor.Green, "###");
        }

        static public void Warning(string message)
        {
            Message(message, ConsoleColor.Yellow, "***");
        }

        static public void Error(string message)
        {
            Message(message, ConsoleColor.Red, "!!!");
        }

        static public void LoadedContent(string message)
        {
            Message(message, ConsoleColor.Cyan, ">>>");
        }

        static public void GameEvent(string message)
        {
            Message(message, ConsoleColor.Blue, "#");
        }

        static public void Script(string message)
        {
            Message(message, ConsoleColor.Gray, ">");
        }

        static public void Debug(string message)
        {
            Message(message, ConsoleColor.Magenta, "$");
        }

        static public void CopyOutputFile(string copyPath)
        {
            File.Copy(_outputPath, copyPath);
        }
    }
}