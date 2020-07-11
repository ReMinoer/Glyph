using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glyph.Audio;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Pipeline;
using Glyph.Tools;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Demos.DropAssets
{
    static public class Program
    {
        private const string ContentPath = "Content";
        private const string CachePath = "Cache";

        static private GlyphGame _game;
        static private SpriteLoader _spriteLoader;
        static private SoundLoader _soundLoader;
        static private SoundEmitter _soundEmitter;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static private bool AllocConsole();

        [STAThread]
        static public void Main()
        {
            AllocConsole();

            using (_game = new GlyphGame(x => new RawContentLibrary(x, ContentPath, CachePath)))
            {
                var form = (Form)Control.FromHandle(_game.Window.Handle);
                form.AllowDrop = true;
                form.DragOver += OnDragOver;
                form.DragDrop += OnDragDrop;

                GlyphEngine engine = _game.Engine;
                GlyphObject root = engine.Root;

                root.Add<SceneNode>();
                engine.InteractionManager.Root.Add(root.Add<InteractiveRoot>().Interactive);

                var freeCamera = root.Add<FreeCamera>();
                freeCamera.View = engine.RootView;
                freeCamera.Client = _game;

                var scene = root.Add<GlyphObject>();
                scene.Add<SceneNode>().RootNode();

                _spriteLoader = scene.Add<SpriteLoader>();
                scene.Add<SpriteRenderer>();

                _soundLoader = scene.Add<SoundLoader>();
                _soundEmitter = scene.Add<SoundEmitter>();
                scene.Add<SoundListener>();

                _game.Run();
            }
        }

        static private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        static private async void OnDragDrop(object sender, DragEventArgs e)
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (filePaths == null || filePaths.Length == 0)
                return;

            string filePath = filePaths[0];
            Console.WriteLine($"Drop {filePath}...");

            await CopyFileToContentFolder(filePath);
            await LoadAssetAsync(filePath);
        }

        static private Task CopyFileToContentFolder(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(ContentPath))
                Directory.CreateDirectory(ContentPath);

            string copyPath = Path.Combine(ContentPath, fileName);
            return Task.Run(() => File.Copy(filePath, copyPath, overwrite: true));
        }

        static private async Task LoadAssetAsync(string filePath)
        {
            string assetPath = Path.GetFileNameWithoutExtension(filePath);

            var asset = await _game.Engine.ContentLibrary.GetOrLoad<object>(assetPath);
            switch (asset)
            {
                case Texture2D _:
                    _spriteLoader.Asset = assetPath;
                    await _spriteLoader.LoadContent(_game.Engine.ContentLibrary);
                    break;
                case SoundEffect _:
                    const string key = nameof(key);
                    _soundLoader.Remove(key);
                    _soundLoader.Add(key, assetPath);
                    await _soundLoader.LoadContent(_game.Engine.ContentLibrary);
                    _soundEmitter.Play(key);
                    break;
            }
        }
    }
}