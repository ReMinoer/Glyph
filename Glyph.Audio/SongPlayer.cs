using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Audio
{
    public class SongPlayer : GlyphComponent, ILoadContent, IUpdate
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static private SongPlayer _instance;
        static public SongPlayer Instance => _instance ?? (_instance = new SongPlayer());

        private const float VolumeSpeed = 0.0005f;
        private readonly Period _waitTime = new Period(0);

        public Dictionary<string, Song> Musics;

        private float _volumeFondu;
        private AudioTransitionState _transitionState = AudioTransitionState.Ready;
        private bool _transitionRequire;

        public float Volume { get; set; }
        public string ActualSong { get; private set; }
        public string NextSong { get; private set; }

        public bool IsRepeating
        {
            get { return MediaPlayer.IsRepeating; }
            set { MediaPlayer.IsRepeating = value; }
        }

        private int IdNextSong
        {
            get
            {
                int id = -1;
                for (int i = 0; i < Musics.Count; i++)
                    if (Musics.ElementAt(i).Key == NextSong)
                        id = i;
                return id;
            }
        }

        private SongPlayer()
        {
            
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            // TODO: Refactor music loading
            Musics = new Dictionary<string, Song>();

            ActualSong = "";
            NextSong = "";
            _transitionState = AudioTransitionState.Ready;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            switch (_transitionState)
            {
                case AudioTransitionState.Ready:
                    if (_transitionRequire)
                        if (MediaPlayer.State == MediaState.Playing)
                        {
                            _volumeFondu = 0;
                            _transitionState = AudioTransitionState.Fading;
                        }
                        else
                            _transitionState = AudioTransitionState.Play;
                    break;

                case AudioTransitionState.Fading:
                    if (System.Math.Abs(MediaPlayer.Volume) < float.Epsilon)
                    {
                        _waitTime.Init();
                        _transitionState = AudioTransitionState.Wait;
                    }
                    break;

                case AudioTransitionState.Wait:
                    _waitTime.Update(elapsedTime.GameTime);
                    if (_waitTime.IsEnd)
                        _transitionState = AudioTransitionState.Play;
                    break;

                case AudioTransitionState.Play:
                    if (NextSong != "")
                    {
                        _volumeFondu = 1f;
                        MediaPlayer.Volume = _volumeFondu * Volume;
                        MediaPlayer.Play(Musics[NextSong]);
                        ActualSong = NextSong;
                        Logger.Info("Now playing song : {0}", ActualSong);
                    }
                    _transitionState = AudioTransitionState.Ready;
                    break;
            }

            //MediaPlayer.Volume = MediaPlayer.Volume.Transition(Volume * _volumeFondu, VolumeSpeed, elapsedTime.GameTime);
            _transitionRequire = false;
        }

        public void Play(string asset)
        {
            if (!Musics.ContainsKey(asset) || (NextSong == asset && IsRepeating))
                return;

            NextSong = asset;
            _transitionRequire = true;
            _transitionState = AudioTransitionState.Ready;
        }

        public void Resume()
        {
            MediaPlayer.Resume();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public void Stop()
        {
            NextSong = "";
            _transitionRequire = true;
            _transitionState = AudioTransitionState.Ready;
        }

        public void Previous()
        {
            if (!Musics.Any())
                return;

            int id = IdNextSong;
            string name = id != -1 ? Musics.ElementAt(id == 0 ? Musics.Count - 1 : id - 1).Key : Musics.ElementAt(0).Key;

            Play(name);
        }

        public void Next()
        {
            if (!Musics.Any())
                return;

            int id = IdNextSong;
            string name = id != -1 ? Musics.ElementAt(id == Musics.Count - 1 ? 0 : id + 1).Key : Musics.ElementAt(0).Key;

            Play(name);
        }

        public enum AudioTransitionState
        {
            Ready,
            Fading,
            Wait,
            Play
        }
    }
}