using System;

namespace Glyph.IO
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RootFolderAttribute : Attribute
    {
        public string Path { get; }

        public RootFolderAttribute(string path)
        {
            Path = path;
        }

        public RootFolderAttribute(Environment.SpecialFolder specialFolder)
        {
            Path = Environment.GetFolderPath(specialFolder);
        }
    }
}