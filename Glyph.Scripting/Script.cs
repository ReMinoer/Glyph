using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Glyph.Application;
using Glyph.Localization;

namespace Glyph.Scripting
{
    public abstract class Script<TCommands>
    {
        public bool Actif { get; set; }

        public bool UniqueUse { get; set; }
        public bool AlreadyUse { get; set; }
        public List<string> Cond { get; set; }
        private readonly LanguageFile _languageFile;
        private readonly List<ScriptLine> _lines;
        private int _ligneActuel;

        protected Script(LanguageFile languageFile)
        {
            _languageFile = languageFile;

            Actif = false;

            _lines = new List<ScriptLine>();
            _ligneActuel = 0;

            UniqueUse = false;
            AlreadyUse = false;
            Cond = new List<string>();
        }

        protected Script(LanguageFile languageFile, bool u)
            : this(languageFile)
        {
            UniqueUse = u;
        }

        public void LoadContent(StreamReader sreader, string filename)
        {
            string readLine = sreader.ReadLine();
            if (readLine == null)
                throw new NullReferenceException();

            string[] keywords = readLine.Trim().Split(' ');

            if (keywords[0] == "cond")
            {
                foreach (string s in keywords[1].Split(','))
                    Cond.Add(s);

                readLine = sreader.ReadLine();
                if (readLine == null)
                    throw new NullReferenceException();
                keywords = readLine.Trim().Split(' ');
            }

            while (keywords[0] != "end")
            {
                var line = new ScriptLine {Cmd = "cmd_" + keywords[0]};

                ParameterInfo[] parameters = typeof(TCommands).GetMethod(line.Cmd).GetParameters();
                line.Args = new object[parameters.Length];

                int j = 0;
                for (int i = 0; i < line.Args.Length; i++)
                {
                    if (CustomizedKeywordRead(line, keywords, i, j))
                        continue;

                    switch (keywords[i + j + 1].ElementAt(0))
                    {
                        case '$':
                            line.Args[i] = _languageFile[filename][keywords[i + j + 1]];
                            break;
                        case '"':
                        {
                            while (keywords[i + j + 1].Last() != '"')
                            {
                                line.Args[i] += keywords[i + j + 1].Trim(new[] {'"'}) + " ";
                                j++;
                            }
                            line.Args[i] += keywords[i + j + 1].Trim(new[] {'"'});
                            break;
                        }
                        default:
                            line.Args[i] = parameters[i].ParameterType.InvokeMember("Parse", BindingFlags.InvokeMethod,
                                null, null, new object[] {keywords[i + j + 1]});
                            break;
                    }
                }

                _lines.Add(line);

                readLine = sreader.ReadLine();
                if (readLine == null)
                    throw new NullReferenceException();
                keywords = readLine.Trim().Split(' ');
            }
        }

        public void Update(Dictionary<string, Trigger> switchs)
        {
            if (!Actif && Cond.Any())
            {
                Actif = true;
                foreach (string name in Cond)
                {
                    if (CustomizedConditionRead(name))
                        continue;

                    Actif = switchs[name].Actif && Actif;
                }
            }

            if (!Actif || UniqueUse && AlreadyUse)
                return;

            while (_ligneActuel < _lines.Count)
            {
                if (
                    !(bool)
                     typeof(TCommands).InvokeMember(_lines[_ligneActuel].Cmd, BindingFlags.InvokeMethod, null, null,
                         _lines[_ligneActuel].Args))
                    return;

                if (_lines[_ligneActuel].Args.Any())
                    Log.Script(string.Format("{0} {1}", _lines[_ligneActuel].Cmd.Substring(4),
                        _lines[_ligneActuel].Args.Aggregate((x, y) => x + " " + y)));
                else
                    Log.Script(_lines[_ligneActuel].Cmd.Substring(4));

                _ligneActuel++;
            }
            Actif = false;
            _ligneActuel = 0;
            AlreadyUse = true;
        }

        protected abstract bool CustomizedKeywordRead(ScriptLine line, string[] keywords, int i, int j);
        protected abstract bool CustomizedConditionRead(string name);
    }
}