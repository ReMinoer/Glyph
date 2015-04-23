using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Audio
{
    static public class AudioManager
    {
        // TODO : Finaliser et tester le manager audio (controle volume, plusieurs effets,...)
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static public Dictionary<string, Song> Musics;
        static private Dictionary<string, SoundEffect> _sounds;
        static private Dictionary<string, SoundCollection> _soundCollections;
        static private float _volumeFondu;
        static private AudioTransitionState _transitionState = AudioTransitionState.Ready;
        static private readonly Period WaitTime = new Period(0);
        static private bool _transitionRequire;
        private const float VolumeSpeed = 0.0005f;
        static public float Volume { get; set; }
        static public string ActualSong { get; private set; }
        static public string NextSong { get; private set; }

        static public bool IsRepeating
        {
            get { return MediaPlayer.IsRepeating; }
            set { MediaPlayer.IsRepeating = value; }
        }

        static public int IdNextSong
        {
            get
            {
                int id = -1;
                for (var i = 0; i < Musics.Count; i++)
                    if (Musics.ElementAt(i).Key == NextSong)
                        id = i;
                return id;
            }
        }

        static public void LoadContent(ContentLibrary ressources)
        {
            Musics = new Dictionary<string, Song>();
            Musics = ressources.GetAllMusic();
            _sounds = new Dictionary<string, SoundEffect>();
            _sounds = ressources.GetAllSound();
            _soundCollections = new Dictionary<string, SoundCollection>();

            ActualSong = "";
            NextSong = "";
            _transitionState = AudioTransitionState.Ready;
        }

        static public void Update(GameTime gameTime)
        {
            MusicTransition(gameTime);

            MediaPlayer.Volume = MediaPlayer.Volume.Transition(Volume * _volumeFondu, VolumeSpeed, gameTime);
            _transitionRequire = false;
        }

        static public void MusicTransition(GameTime gameTime)
        {
            switch (_transitionState)
            {
                case AudioTransitionState.Ready:
                    if (_transitionRequire)
                        if (MediaPlayer.State == MediaState.Playing)
                        {
                            _volumeFondu = 0;
                            _transitionState = AudioTransitionState.Fondu;
                        }
                        else
                            _transitionState = AudioTransitionState.Play;
                    break;

                case AudioTransitionState.Fondu:
                    if (Math.Abs(MediaPlayer.Volume) < float.Epsilon)
                    {
                        WaitTime.Init();
                        _transitionState = AudioTransitionState.Wait;
                    }
                    break;

                case AudioTransitionState.Wait:
                    WaitTime.Update(gameTime);
                    if (WaitTime.IsEnd)
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
        }

        static public void Play(string asset)
        {
            if (Musics.ContainsKey(asset) && (NextSong != asset || !IsRepeating))
            {
                NextSong = asset;
                _transitionRequire = true;
                _transitionState = AudioTransitionState.Ready;
            }
        }

        static public void Resume()
        {
            MediaPlayer.Resume();
        }

        static public void Pause()
        {
            MediaPlayer.Pause();
        }

        static public void Stop()
        {
            NextSong = "";
            _transitionRequire = true;
            _transitionState = AudioTransitionState.Ready;
        }

        static public void Previous()
        {
            if (!Musics.Any())
                return;

            int id = IdNextSong;
            string name = id != -1 ? Musics.ElementAt(id == 0 ? Musics.Count - 1 : id - 1).Key : Musics.ElementAt(0).Key;

            Play(name);
        }

        static public void Next()
        {
            if (!Musics.Any())
                return;

            int id = IdNextSong;
            string name = id != -1 ? Musics.ElementAt(id == Musics.Count - 1 ? 0 : id + 1).Key : Musics.ElementAt(0).Key;

            Play(name);
        }

        static public void PlaySound(string asset)
        {
            if (_sounds.ContainsKey(asset))
                _sounds[asset].Play();
        }

        static public void PlaySoundCollection(string asset)
        {
            if (_soundCollections.ContainsKey(asset))
                _soundCollections[asset].Play();
        }

        static public void UpdateSoundCollection(string asset, GameTime gameTime)
        {
            if (_soundCollections.ContainsKey(asset))
                _soundCollections[asset].Update(gameTime);
        }

        static public void AddSoundCollection(string asset, int period = 0)
        {
            if (_soundCollections.ContainsKey(asset))
                return;

            _soundCollections.Add(asset, new SoundCollection(period));

            var i = 1;
            while (_sounds.ContainsKey(asset + i))
            {
                _soundCollections[asset].AddSound(_sounds[asset + i]);
                i++;
            }
        }

        public enum AudioTransitionState
        {
            Ready,
            Fondu,
            Wait,
            Play
        }
    }
}