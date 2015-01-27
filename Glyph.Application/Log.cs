using System;
using System.IO;

namespace Glyph.Application
{
    static public class Log
    {
        static public bool ViewInConsole { get; set; }
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

        static private bool _useRealTime = true;
        static private string _outputPath = "log.txt";
        static private StreamWriter _fileStream = new StreamWriter("log.txt") {AutoFlush = true};

        static Log()
        {
            ViewInConsole = false;
        }

        static public void Message(string message)
        {
            string line = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), message);

            if (ViewInConsole)
                Console.WriteLine(line);

            _fileStream.WriteLine(line);
        }

        static public void Warning(string message)
        {
            string line = string.Format("[{0}] *** {1} ***", DateTime.Now.ToString("HH:mm:ss"), message);

            if (ViewInConsole)
                Console.WriteLine(line);

            _fileStream.WriteLine(line);
        }

        static public void Error(string message)
        {
            string line = string.Format("[{0}] !!! {1} !!!", DateTime.Now.ToString("HH:mm:ss"), message);

            if (ViewInConsole)
                Console.WriteLine(line);

            _fileStream.WriteLine(line);
        }
    }
}