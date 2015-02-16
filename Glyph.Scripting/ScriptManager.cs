using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Diese.Debug;
using Glyph.Debug;
using Glyph.Entities;
using Glyph.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Scripting
{
    public abstract class ScriptManager<TCommands>
    {
        public Dictionary<string, Script<TCommands>> Scripts { get; private set; }
        public TriggerManager Triggers { get; private set; }
        public bool DrawTriggerZone { get; set; }
        private readonly LanguageFile _languageFile;

        protected ScriptManager(LanguageFile languageFile)
        {
            _languageFile = languageFile;

            Scripts = new Dictionary<string, Script<TCommands>>();
            Triggers = new TriggerManager();
            DrawTriggerZone = false;
        }

        public void LoadContent(ContentLibrary ressources)
        {
            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).LoadContent(ressources);
        }

        public void LoadScript(string path)
        {
            Stream stream;
            StreamReader sreader;
            try
            {
                stream = TitleContainer.OpenStream(path);
                sreader = new StreamReader(stream);
            }
            catch (ArgumentException)
            {
                sreader = new StreamReader(path);
                stream = sreader.BaseStream;
            }
            catch (NotSupportedException)
            {
                sreader = new StreamReader(path);
                stream = sreader.BaseStream;
            }

            string filename = path.Split('\\').Last().Split('/').Last().Split('.').First();

            Scripts.Clear();
            Triggers.Clear();

            while (!sreader.EndOfStream)
            {
                string line = sreader.ReadLine();
                if (line == null)
                    throw new NullReferenceException();

                var lineRead = line.Trim().Split(' ');

                if (lineRead[0] == "triggerlist")
                {
                    line = sreader.ReadLine();
                    if (line == null)
                        throw new NullReferenceException();

                    lineRead = line.Trim().Split(' ');
                    while (lineRead[0] != "end")
                    {
                        if (lineRead.Length > 1)
                        {
                            var coord = new[] {0, 0, 0, 0, 0};
                            var s = lineRead[1].Split(',');
                            for (int i = 0; i < coord.Length; i++)
                                coord[i] = Int32.Parse(s[i]);
                            Triggers.Add(lineRead[0], new TriggerZone(coord[0], coord[1], coord[2], coord[3], coord[4]));
                        }
                        else
                            Triggers.Add(lineRead[0], new Trigger());

                        string readLine = sreader.ReadLine();
                        if (readLine == null)
                            throw new NullReferenceException();

                        lineRead = readLine.Trim().Split(' ');
                    }
                }

                if (lineRead[0] != "function")
                    continue;

                if (lineRead[1] == "unique")
                {
                    Scripts.Add(lineRead[2], CreateScript(_languageFile, true));
                    Scripts[lineRead[2]].LoadContent(sreader, filename);
                }
                else
                {
                    Scripts.Add(lineRead[1], CreateScript(_languageFile, false));
                    Scripts[lineRead[1]].LoadContent(sreader, filename);
                }
            }

            stream.Close();

            if (Scripts.ContainsKey("init"))
                Scripts["init"].Actif = true;

            Log.Message(string.Format("Script loaded : {0}", path), LogTagGlyph.GameEvent);
        }

        public void Update(GameObject entity)
        {
            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).Update(entity);
            foreach (var s in Scripts.Values)
                s.Update(Triggers);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!DrawTriggerZone)
                return;

            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).Draw(spriteBatch);
        }

        protected abstract Script<TCommands> CreateScript(LanguageFile languageFile, bool isUnique);
    }
}