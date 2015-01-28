﻿//////////////////////////////////////////////////////////////////////////
////License:  The MIT License (MIT)
////Copyright (c) 2010 David Amador (http://www.david-amador.com)
////
////Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
////
////The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
////
////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//////////////////////////////////////////////////////////////////////////

using Glyph.Application;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    // WATCH : Viewport improvement behaviour
    static public class Resolution
    {
        static public bool FullScreen { get; private set; }

        static public Vector2 Size { get; private set; }
        static public Vector2 WindowSize { get { return new Vector2(_windowWidth, _windowHeight); } }
        static public Vector2 VirtualSize { get { return new Vector2(_virtualWidth, _virtualHeight); } }

        static public float ScaleRatio { get { return Size.X / _virtualWidth; } }
        static public Vector2 WindowMargin { get { return (WindowSize - Size) / 2; } }

        static private GraphicsDeviceManager _device;
        static private GameWindow _window;

        static private int _windowWidth = 800;
        static private int _windowHeight = 450;
        static private int _virtualWidth = 800;
        static private int _virtualHeight = 450;
        static private int _savedWidth = 800;
        static private int _savedHeight = 450;

        static private Matrix _scaleMatrix;
        static private bool _dirtyMatrix = true;

        static private bool _currentlyResizing;

        static public void Init(GraphicsDeviceManager device, GameWindow window)
        {
            _device = device;
            _window = window;

            UpdateResolution();
            ApplyResolutionSettings();
            _dirtyMatrix = true;
        }

        static public void SetWindow(int width, int height, bool fullScreen)
        {
            _windowWidth = width;
            _windowHeight = height;

            if (!fullScreen)
            {
                _savedWidth = _windowWidth;
                _savedHeight = _windowHeight;
            }

            FullScreen = fullScreen;

            UpdateResolution();
            ApplyResolutionSettings();
            _dirtyMatrix = true;
        }

        static public void SetVirtualResolution(int width, int height)
        {
            _virtualWidth = width;
            _virtualHeight = height;

            UpdateResolution();
            _dirtyMatrix = true;

            Log.System(string.Format("Virtual resolution changed (Width:{0},Height:{1},Ratio:{2})", _virtualWidth, _virtualHeight, GetVirtualAspectRatio().ToString("F2")));
        }

        static public void ToogleFullscreen()
        {
            if (FullScreen)
                SetWindow(_savedWidth, _savedHeight, false);
            else
            {
                _savedWidth = _windowWidth;
                _savedHeight = _windowHeight;
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                SetWindow(displayMode.Width, displayMode.Height, true);
            }
        }

        static public Matrix GetTransformationMatrix()
        {
            if (_dirtyMatrix)
                RecreateScaleMatrix();

            return _scaleMatrix;
        }

        static public float GetVirtualAspectRatio()
        {
            return _virtualWidth / (float)_virtualHeight;
        }

        static public void BeginDraw()
        {
            ResetViewport();
            _device.GraphicsDevice.Clear(Color.Black);
        }

        static public void ResetViewport()
        {
            float targetAspectRatio = GetVirtualAspectRatio();

            int width = _device.PreferredBackBufferWidth;
            var height = (int)(width / targetAspectRatio + .5f);

            if (height > _device.PreferredBackBufferHeight)
            {
                height = _device.PreferredBackBufferHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            var viewport = new Viewport
            {
                X = (_device.PreferredBackBufferWidth / 2) - (width / 2),
                Y = (_device.PreferredBackBufferHeight / 2) - (height / 2),
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };

            _device.GraphicsDevice.Viewport = viewport;
        }

        static private void ApplyResolutionSettings()
        {
            if (_currentlyResizing)
                return;

            _currentlyResizing = true;

#if XBOX360
                _FullScreen = true;
#endif

            if (FullScreen == false)
            {
                if ((_windowWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (_windowHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    _device.PreferredBackBufferWidth = _windowWidth;
                    _device.PreferredBackBufferHeight = _windowHeight;
                    _window.IsBorderless = false;
                    _device.IsFullScreen = false;
                    _device.ApplyChanges();
                }
            }
            else
            {
                int maxWidth = 0;
                int maxHeight = 0;
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    if (dm.Width >= maxWidth && dm.Height >= maxHeight)
                    {
                        maxWidth = dm.Width;
                        maxHeight = dm.Height;
                    }

                _window.IsBorderless = true;
                _device.IsFullScreen = true;
                _window.Position = new Point(0, 0);
                _device.PreferredBackBufferWidth = maxWidth;
                _device.PreferredBackBufferHeight = maxHeight;
                _device.ApplyChanges();
                /*
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    if ((dm.Width == _windowWidth) && (dm.Height == _windowHeight))
                    {
                        _window.IsBorderless = true;
                        _window.Position = new Point(0, 0);
                        _device.PreferredBackBufferWidth = _windowWidth;
                        _device.PreferredBackBufferHeight = _windowHeight;
                        _device.ApplyChanges();
                    }*/
            }

            _windowWidth = _device.PreferredBackBufferWidth;
            _windowHeight = _device.PreferredBackBufferHeight;

            _currentlyResizing = false;

            Log.System(string.Format("Window resized (Width:{0},Height:{1},Ratio:{2},Fullscreen:{3})", _windowWidth, _windowHeight, ((double)_windowWidth / _windowHeight).ToString("F2"), FullScreen));
        }

        static private void UpdateResolution()
        {
            float virtualRatio = GetVirtualAspectRatio();
            Size = WindowSize.X / WindowSize.Y > virtualRatio
                       ? WindowSize.SetX(WindowSize.Y * virtualRatio)
                       : WindowSize.SetY(WindowSize.X / virtualRatio);
        }

        static private void RecreateScaleMatrix()
        {
            _dirtyMatrix = false;
            _scaleMatrix = Matrix.CreateScale((float)_device.GraphicsDevice.Viewport.Width / _virtualWidth,
                (float)_device.GraphicsDevice.Viewport.Width / _virtualWidth, 1f);

            float virtualRatio = GetVirtualAspectRatio();
            Size = WindowSize.X / WindowSize.Y > virtualRatio
                       ? WindowSize.SetX(WindowSize.Y * virtualRatio)
                       : WindowSize.SetY(WindowSize.X / virtualRatio);
        }
    }
}