using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph
{
    public class ContentLibrary
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        // TODO : Etudier le chargement asynchrone
        private readonly Dictionary<string, Effect> _effects;
        private readonly Dictionary<string, SpriteFont> _fonts;
        private readonly Dictionary<string, Song> _musics;
        private readonly Dictionary<string, SoundEffect> _sounds;
        private readonly Dictionary<string, Texture2D> _textures;

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

        public void LoadContent(ContentManager content)
        {
            _textures.Clear();
            _fonts.Clear();
            _sounds.Clear();
            _musics.Clear();
            _effects.Clear();

            const string path = "Content";

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(path, "*.xnb", SearchOption.AllDirectories));
            
            foreach (string filename in files)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string file = filename.Replace("Content\\", "");

                if (file.Contains("Font"))
                    AddFont(file, content);
                else if (file.Contains("Sound"))
                    AddSound(file, content);
                else if (file.Contains("Music"))
                    AddMusic(file, content);
                else if (file.Contains("Effect"))
                    AddEffect(file, content);
                else
                    AddTexture(file, content);

                stopwatch.Stop();
                lock (Logger)
                    Logger.Info("Loaded {0} ({1}s)", file, stopwatch.Elapsed.ToString(@"s\.fff"));
                stopwatch.Reset();
            }
        }

        public void AddTexture(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            var texture = content.Load<Texture2D>(asset);

            lock (_textures)
                _textures.Add(asset.Substring(asset.LastIndexOf('\\') + 1), texture);
        }

        public Texture2D GetTexture(string asset)
        {
            return _textures[asset];
        }

        public void AddFont(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            var font = content.Load<SpriteFont>(asset);

            lock (_fonts)
                _fonts.Add(asset.Substring(asset.LastIndexOf('\\') + 1), font);
        }

        public SpriteFont GetFont(string asset)
        {
            return _fonts[asset];
        }

        public void AddSound(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            var sound = content.Load<SoundEffect>(asset);

            lock (_sounds)
                _sounds.Add(asset.Substring(asset.LastIndexOf('\\') + 1), sound);
        }

        public SoundEffect GetSound(string asset)
        {
            return _sounds[asset];
        }

        public void AddMusic(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            var song = content.Load<Song>(asset);

            lock (_musics)
                _musics.Add(asset.Substring(asset.LastIndexOf('\\') + 1), song);
        }

        public Song GetMusic(string asset)
        {
            return _musics[asset];
        }

        public void AddEffect(string path, ContentManager content)
        {
            string asset = path.Replace(".xnb", "");
            var effect = content.Load<Effect>(asset);

            lock (_effects)
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