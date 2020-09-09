using System;
using System.Linq;

namespace Glyph.IO
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class FileTypeAttribute : Attribute
    {
        public FileType FileType { get; private set; }

        public string DisplayName
        {
            get => FileType.DisplayName;
            set => FileType = new FileType
            {
                DisplayName = value,
                Extensions = FileType.Extensions
            };
        }

        public override object TypeId => FileType;

        public FileTypeAttribute(params string[] extensions)
        {
            FileType = new FileType
            {
                DisplayName = extensions?.FirstOrDefault()?.TrimStart('.').ToUpper() + " Files",
                Extensions = extensions
            };
        }
    }
}