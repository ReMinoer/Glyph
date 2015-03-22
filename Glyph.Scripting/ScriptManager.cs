using System;
using System.Collections.Generic;
using System.IO;
using Diese.Debug;
using Diese.Lua;
using Diese.Lua.Properties;
using Glyph.Debug;
using Glyph.Entities;
using Glyph.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLua;

namespace Glyph.Scripting
{
    public abstract class ScriptManager
    {
        private readonly LanguageFile _languageFile;
        private readonly Dictionary<string, Dictionary<string, LuaFunction>> _luaFunctions;
        protected readonly Lua Lua;
        public TriggerManager Triggers { get; private set; }
        public bool DrawTriggerZone { get; set; }

        protected ScriptManager(LanguageFile languageFile)
        {
            _languageFile = languageFile;

            Triggers = new TriggerManager();
            DrawTriggerZone = false;
            _luaFunctions = new Dictionary<string, Dictionary<string, LuaFunction>>();

            Lua = new Lua();
            Lua.LoadCLRPackage();
            Lua.DoCoroutineManager();
            Lua.DoString(Resources.TableTools);

            Lua["Triggers"] = Triggers;
            Lua["Local"] = _languageFile;
        }

        public void LoadContent(ContentLibrary ressources)
        {
            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).LoadContent(ressources);
        }

        public void LoadScript(string path)
        {
            Triggers.Clear();
            _luaFunctions.Clear();

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

            Lua.DoString(sreader.ReadToEnd());

            stream.Close();

            string scriptName = Path.GetFileNameWithoutExtension(path);
            LuaTable mainTable = Lua.GetTable(scriptName);
            Dictionary<object, object> functions = Lua.GetTableDict(mainTable);

            var scriptFunctions = new Dictionary<string, LuaFunction>();

            foreach (KeyValuePair<object, object> pair in functions)
            {
                if (!(pair.Value is LuaFunction))
                    continue;

                var name = (string)pair.Key;
                var function = (LuaFunction)pair.Value;

                scriptFunctions.Add(name, function);

                if ((string)pair.Key == "init")
                    Lua.CreateCoroutine(function, scriptName + "." + name);
            }

            if (scriptName != null)
                _luaFunctions.Add(scriptName, scriptFunctions);

            foreach (Trigger trigger in Triggers.Values)
                trigger.Enabled += TriggerOnEnabled;

            Log.Message(string.Format("Script loaded : {0}", path), LogTagGlyph.GameEvent);
        }

        public void Update(GameTime gameTime, GameObject entity)
        {
            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).Update(entity);

            Dictionary<string, LuaCoroutineResult> results =
                Lua.UpdateCoroutines(gameTime.ElapsedGameTime.TotalMilliseconds);

            foreach (KeyValuePair<string, LuaCoroutineResult> pair in results)
            {
                string functionName = pair.Key;
                LuaCoroutineResult result = pair.Value;

                if (result.ToString() != "" && result.ErrorMessage != "cannot resume dead coroutine")
                    Log.Message(string.Format("{0} > {1}", functionName, result), LogTagGlyph.Error);
                else if (Lua.StatusCoroutine(functionName) == LuaCoroutineStatus.Dead && result.IsValid)
                    Log.Message(string.Format("{0} > finished", functionName), LogTagGlyph.Script);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!DrawTriggerZone)
                return;

            foreach (Trigger t in Triggers.Values)
                if (t is TriggerZone)
                    (t as TriggerZone).Draw(spriteBatch);
        }

        private void TriggerOnEnabled(object sender, EventArgs eventArgs)
        {
            var trigger = (Trigger)sender;

            string[] subStrings = trigger.Name.Split('.');
            string scriptName = subStrings[0];
            string functionName = subStrings[1];

            if (_luaFunctions.ContainsKey(scriptName) && _luaFunctions[scriptName].ContainsKey(functionName))
            {
                Lua.CreateCoroutine(_luaFunctions[scriptName][functionName], scriptName + "." + functionName);
                Log.Message(string.Format("{0} > triggered", scriptName + "." + functionName), LogTagGlyph.Script);
            }
        }
    }
}