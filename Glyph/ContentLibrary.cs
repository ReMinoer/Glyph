using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph
{
    public class ContentLibrary
    {
        private readonly Dictionary<string, Effect> _effects;
        private readonly Dictionary<string, SpriteFont> _fonts;
        private readonly Dictionary<string, Song> _musics;
        private readonly Dictionary<string, SoundEffect> _sounds;
        private readonly Dictionary<string, Texture2D> _textures;
        private GraphicsDevice _graphicsDevice;
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        // TODO : Etudier le chargement asynchrone

        public Dictionary<string, Texture2D>.KeyCollection Assets
        {
            get { return _textures.Keys; }
        }

        public ContentLibrary()
        {
            _textures = new Dictionary<string, Texture2D>();
            _fonts = new Dictionary<string, SpriteFont>();
            _sounds = new Dictionary<string, SoundEffect>();
            _musics = new Dictionary<string, Song>();
            _effects = new Dictionary<string, Effect>();
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _textures.Clear();
            _fonts.Clear();
            _sounds.Clear();
            _musics.Clear();
            _effects.Clear();

            const string path = "Content";

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(path, "*.xnb", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(path, "*.mgfx", SearchOption.AllDirectories));

            foreach (string s in files)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (s.Contains("Font"))
                    AddFont(s.Replace("Content\\", ""), content);
                else if (s.Contains("Sound"))
                    AddSound(s.Replace("Content\\", ""), content);
                else if (s.Contains("Music"))
                    AddMusic(s.Replace("Content\\", ""), content);
                else if (s.Contains("Effect"))
                    AddEffect(s, content);
                else
                    AddTexture(s.Replace("Content\\", ""), content);

                stopwatch.Stop();
                Logger.Info("Loaded {0} ({1}s)", s, stopwatch.Elapsed.ToString(@"s\.fff"));
                stopwatch.Reset();
            }
        }

        public void AddTexture(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            _textures.Add(asset.Substring(asset.LastIndexOf('\\') + 1), content.Load<Texture2D>(asset));
        }

        public Texture2D GetTexture(string asset)
        {
            return _textures[asset];
        }

        public void AddFont(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            _fonts.Add(asset.Substring(asset.LastIndexOf('\\') + 1), content.Load<SpriteFont>(asset));
        }

        public SpriteFont GetFont(string asset)
        {
            return _fonts[asset];
        }

        public void AddSound(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            _sounds.Add(asset.Substring(asset.LastIndexOf('\\') + 1), content.Load<SoundEffect>(asset));
        }

        public SoundEffect GetSound(string asset)
        {
            return _sounds[asset];
        }

        public void AddMusic(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            _musics.Add(asset.Substring(asset.LastIndexOf('\\') + 1), content.Load<Song>(asset));
        }

        public Song GetMusic(string asset)
        {
            return _musics[asset];
        }

        public void AddEffect(string path, ContentManager content)
        {
            string asset = path.Replace(".mgfx", "");
            byte[] bytecode = File.ReadAllBytes(path);
            var effect = new Effect(_graphicsDevice, bytecode);
            _effects.Add(asset.Substring(asset.LastIndexOf('\\') + 1), effect);
        }

        public Effect GetEffect(string asset)
        {
            return _effects[asset];
        }

        public Dictionary<string, SoundEffect> GetAllSound()
        {
            return _sounds;
        }

        public Dictionary<string, Song> GetAllMusic()
        {
            return _musics;
        }

        public bool Contains(string search)
        {
            return _textures.ContainsKey(search) || _fonts.ContainsKey(search);
        }
    }
}