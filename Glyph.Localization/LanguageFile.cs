using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Diese.Serialization;
using Microsoft.Xna.Framework;

namespace Glyph.Localization
{
    public class LanguageFile : SerializableDictionary<string, SerializableDictionary<string, string>>
    {
        static public string FileExtension = ".glyphlang";

        static public LanguageFile LoadContent(CultureInfo culture)
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(TitleContainer.OpenStream("Localization/" + culture.Name + FileExtension));
            }
            catch
            {
                try
                {
                    sr = new StreamReader(TitleContainer.OpenStream("Localization/default" + FileExtension));
                }
                catch
                {
                    return new LanguageFile();
                }
            }
            var serializer = new XmlSerializer(typeof(LanguageFile));
            var result = (LanguageFile)serializer.Deserialize(sr);
            sr.Close();
            return result;
        }

        static public LanguageFile Load(CultureInfo culture, string directory = "")
        {
            var serializer = new SerializerXml<LanguageFile>();
            return serializer.Instantiate(directory + culture.Name + FileExtension);
        }

        public void LoadLevelEditorText(string path)
        {
            if (!File.Exists(path + ".text"))
                return;

            var sr = new StreamReader(path + ".text");

            string levelName = path.Split('\\').Last().Split('/').Last().Split('.').First();

            if (ContainsKey(levelName))
                this[levelName] = new SerializableDictionary<string, string>();
            else
                Add(levelName, new SerializableDictionary<string, string>());

            for (int i = 1; !sr.EndOfStream; i++)
                this[levelName].Add("$" + i, sr.ReadLine());
            sr.Close();
        }

        public void Save(CultureInfo culture, string directory = "")
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var serializer = new SerializerXml<LanguageFile>();
            serializer.Save(this, directory + culture.Name + FileExtension);
        }
    }
}