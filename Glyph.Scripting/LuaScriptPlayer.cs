using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Diese.Lua;
using Glyph.Composition;
using Glyph.Scripting.Triggers;
using Microsoft.Xna.Framework;
using NLog;
using NLua;

namespace Glyph.Scripting
{
    public class LuaScriptPlayer : GlyphComponent, IScriptPlayer
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Lua _lua;
        private readonly Dictionary<string, LuaFunction> _loadedFunctions;
        private readonly Dictionary<string, LuaFunction> _coroutines;
        private readonly Dictionary<string, ITrigger> _triggers;
        private readonly Dictionary<string, ITriggerArea> _triggerAreas;
        public bool Enabled { get; set; }
        public bool UseUnscaledTime { get; set; }
        public Dictionary<object, IActor> Actors { get; private set; }
        public IReadOnlyDictionary<string, ITrigger> Triggers { get; private set; }
        public IReadOnlyDictionary<string, ITriggerArea> TriggerAreas { get; private set; }

        public LuaScriptPlayer(ScriptManager scriptManager)
        {
            Enabled = true;

            _lua = scriptManager.Lua;
            
            _loadedFunctions = new Dictionary<string, LuaFunction>();
            _coroutines = new Dictionary<string, LuaFunction>();

            Actors = new Dictionary<object, IActor>();

            _triggers = new Dictionary<string, ITrigger>();
            _triggerAreas = new Dictionary<string, ITriggerArea>();

            Triggers = new ReadOnlyDictionary<string, ITrigger>(_triggers);
            TriggerAreas = new ReadOnlyDictionary<string, ITriggerArea>(_triggerAreas);
        }

        public void LoadScript(string path)
        {
            _loadedFunctions.Clear();
            _coroutines.Clear();
            _lua.CleanDeadCoroutines();

            Stream stream;
            StreamReader streamReader;
            try
            {
                stream = TitleContainer.OpenStream(path);
                streamReader = new StreamReader(stream);
            }
            catch (ArgumentException)
            {
                streamReader = new StreamReader(path);
                stream = streamReader.BaseStream;
            }
            catch (NotSupportedException)
            {
                streamReader = new StreamReader(path);
                stream = streamReader.BaseStream;
            }

            _lua.DoString(streamReader.ReadToEnd());

            stream.Close();

            string scriptName = Path.GetFileNameWithoutExtension(path);
            LuaTable mainTable = _lua.GetTable(scriptName);
            Dictionary<object, object> functions = _lua.GetTableDict(mainTable);

            foreach (KeyValuePair<object, object> pair in functions)
            {
                if (!(pair.Value is LuaFunction))
                    continue;

                string name = scriptName + "." + (string)pair.Key;
                var function = (LuaFunction)pair.Value;

                _loadedFunctions.Add(name, function);

                if ((string)pair.Key == "init")
                    CreateCoroutine(function, name);
            }

            Logger.Info("Script loaded : {0}", path);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ITriggerArea triggerArea in _triggerAreas.Values)
                triggerArea.UpdateStatus(Actors.Values);

            foreach (string coroutineName in _coroutines.Keys)
            {
                LuaCoroutineResult result = _lua.ResumeCoroutine(coroutineName, elapsedTime.GetDelta(this));

                if (result.ToString() != "" && result.ErrorMessage != "cannot resume dead coroutine")
                    Logger.Error("{0} > {1}", coroutineName, result);
                else if (_lua.StatusCoroutine(coroutineName) == LuaCoroutineStatus.Dead && result.IsValid)
                    Logger.Info("{0} > finished", coroutineName);
            }
        }

        public void RegisterVar(string name, bool singleUse = false)
        {
            var triggerVar = new TriggerVar(singleUse)
            {
                Name = name
            };

            Register(triggerVar);
        }

        public void RegisterArea(ITriggerArea triggerArea)
        {
            Register(triggerArea);
            _triggerAreas.Add(triggerArea.Name, triggerArea);
        }

        public void UnregisterTrigger(ITrigger trigger)
        {
            UnregisterTrigger(_triggers.First(x => x.Value == trigger).Key);
        }

        public void UnregisterTrigger(string name)
        {
            _triggers.Remove(name);

            if (_triggerAreas.ContainsKey(name))
                _triggerAreas.Remove(name);
        }

        public void CleanTriggers()
        {
            _triggers.Clear();
            _triggerAreas.Clear();
        }

        public void SetTriggerAreasVisibility(bool visible)
        {
            foreach (ITriggerArea triggerArea in _triggerAreas.Values)
                triggerArea.Visible = false;
        }

        private void Register(ITrigger trigger)
        {
            if (string.IsNullOrWhiteSpace(trigger.Name))
                throw new ArgumentException("Trigger name can't be null or white space !");

            _triggers.Add(trigger.Name, trigger);
            trigger.Triggered += TriggerOnTriggered;
        }

        void IScriptPlayer.RegisterTrigger(ITrigger trigger)
        {
            var triggerArea = trigger as ITriggerArea;
            if (triggerArea != null)
                RegisterArea(triggerArea);
            else
                Register(trigger);
        }

        private void CreateCoroutine(LuaFunction luaFunction, string name)
        {
            _lua.CreateCoroutine(luaFunction, name);
            _coroutines.Add(name, luaFunction);
            Logger.Info("{0} > triggered", name);
        }

        private void TriggerOnTriggered(ITrigger trigger)
        {
            if (_loadedFunctions.ContainsKey(trigger.Name))
                CreateCoroutine(_loadedFunctions[trigger.Name], trigger.Name);
        }
    }
}