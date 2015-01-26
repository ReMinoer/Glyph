using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glyph.Tools
{
    public static class Log
    {
        static public bool ViewInConsole { get { return _viewInConsole; } set { _viewInConsole = value; } }
        static public bool UseRealTime { get { return _useRealTime; } set { _useRealTime = value; } }
        
        static public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                _outputPath = value;
                _fileStream.Close();
                _fileStream = new StreamWriter(_outputPath) { AutoFlush = true };
            }
        }

        static private bool _viewInConsole = false;
        static private bool _useRealTime = true;
        static private string _outputPath = "log.txt";
        static private StreamWriter _fileStream = new StreamWriter("log.txt") { AutoFlush = true };

        public static void Write(string message)
        {
            string line = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), message);

            if (ViewInConsole)
                Console.WriteLine(line);

            _fileStream.WriteLine(line);
        }
    }
}
