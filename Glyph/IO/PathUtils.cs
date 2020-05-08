using System.IO;

namespace Glyph.IO
{
    static public class PathUtils
    {
        static readonly char AbsoluteSeparator = Path.DirectorySeparatorChar;
        static readonly char RelativeSeparator = Path.AltDirectorySeparatorChar;

        static public string Normalize(string path) => Normalize(path, out _);
        static public string NormalizeFolder(string path)
        {
            path = Normalize(path, out char separator);

            // Add end separator
            if (path[path.Length - 1] != separator)
                path += separator;

            return path;
        }

        static private string Normalize(string path, out char separator)
        {
            bool isAbsolute = Path.IsPathRooted(path);
            separator = isAbsolute ? AbsoluteSeparator : RelativeSeparator;
            char otherSeparator = isAbsolute ? RelativeSeparator : AbsoluteSeparator;

            // Use unique separator
            path = path.Replace(otherSeparator, separator);

            return path;
        }
    }
}