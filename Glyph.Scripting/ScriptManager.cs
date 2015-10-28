using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Diese.Lua;
using Diese.Lua.Properties;
using Glyph.Composition;
using Glyph.Effects;
using Glyph.Entities;
using Glyph.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using NLua;

namespace Glyph.Scripting
{
    public class ScriptManager : GlyphComponent, IDraw
    {
        protected readonly Lua Lua;
        private readonly LanguageFile _languageFile;
        private readonly Dictionary<string, Dictionary<string, LuaFunction>> _luaFunctions;
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SpriteBatch _spriteBatch;
        public TriggerManager Triggers { get; private set; }
        public bool Visible { get; set; }

        public ScriptManager(ContentLibrary contentLibrary, LanguageFile languageFile, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _languageFile = languageFile;
            _spriteBatch = spriteBatch;

            Triggers = new TriggerManager(this);
            Visible = false;
            _luaFunctions = new Dictionary<string, Dictionary<string, LuaFunction>>();

            Lua = new Lua();
            Lua.LoadCLRPackage();
            Lua.LoadCoroutineManager();
            Lua.DoString(Resources.TableTools);

            Lua["Content"] = contentLibrary;
            Lua["GraphicsDevice"] = graphicsDevice;
            Lua["ScreenEffectManager"] = ScreenEffectManager.Instance;
            Lua["Triggers"] = Triggers;
            Lua["Local"] = _languageFile;
        }

        public void LoadContent(ContentLibrary ressources)
        {
            foreach (TriggerZone t in Triggers.Values.OfType<TriggerZone>())
                t.LoadContent(ressources);
        }

        public IEnumerable<string> GetLuaGlobals()
        {
            return Lua.Globals.ToArray();
        }

        public void DoLuaScript(string luaCode)
        {
            Lua.DoString(luaCode);
        }

        public void LoadScript(string path)
        {
            Triggers.Clear();
            _luaFunctions.Clear();
            Lua.CleanDeadCoroutines();

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

                string name = (string)pair.Key;
                var function = (LuaFunction)pair.Value;

                scriptFunctions.Add(name, function);

                if ((string)pair.Key == "init")
                    Lua.CreateCoroutine(function, scriptName + "." + name);
            }

            if (scriptName != null)
                _luaFunctions.Add(scriptName, scriptFunctions);

            Logger.Info("Script loaded : {0}", path);
        }

        public void Update(GameTime gameTime, GlyphObject entity)
        {
            foreach (TriggerZone t in Triggers.Values.OfType<TriggerZone>())
                t.Update(entity);

            Dictionary<string, LuaCoroutineResult> results =
                Lua.UpdateCoroutines(gameTime.ElapsedGameTime.TotalMilliseconds);

            foreach (KeyValuePair<string, LuaCoroutineResult> pair in results)
            {
                string functionName = pair.Key;
                LuaCoroutineResult result = pair.Value;

                if (result.ToString() != "" && result.ErrorMessage != "cannot resume dead coroutine")
                    Logger.Error("{0} > {1}", functionName, result);
                else if (Lua.StatusCoroutine(functionName) == LuaCoroutineStatus.Dead && result.IsValid)
                    Logger.Info("{0} > finished", functionName);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (TriggerZone t in Triggers.Values.OfType<TriggerZone>())
                t.Draw(_spriteBatch);
        }

        public void TriggerOnEnabled(object sender, EventArgs eventArgs)
        {
            var trigger = (Trigger)sender;

            string[] subStrings = trigger.Name.Split('.');
            string scriptName = subStrings[0];
            string functionName = subStrings[1];

            if (_luaFunctions.ContainsKey(scriptName) && _luaFunctions[scriptName].ContainsKey(functionName))
            {
                Lua.CreateCoroutine(_luaFunctions[scriptName][functionName], scriptName + "." + functionName);
                Logger.Info("{0} > triggered", scriptName + "." + functionName);
            }
        }
    }
}