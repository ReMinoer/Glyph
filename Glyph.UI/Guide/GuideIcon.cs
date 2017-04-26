using System;
using System.Linq;
using Diese.Injection;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Carvers;
using Glyph.Graphics.Renderer;
using Glyph.Input;
using Glyph.Transition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Diese.Collections;
using Glyph.Core.ControlLayers;

namespace Glyph.UI.Guide
{
    public class GuideIcon : GlyphObject
    {
        private readonly ControlManager _controlManager;
        protected readonly TransitionFloat TransitionOpacity = new TransitionFloat {Start = 0, End = 1, Duration = 500};
        protected bool IsGamePadUsed;
        protected readonly SpriteSheetSplit SpriteSheetSplit;
        protected readonly SpriteSheet ComputerIcon;
        protected readonly SpriteSheet GamePadIcon;
        protected readonly SpriteArea SpriteArea;
        protected readonly SpriteTransformer SpriteTransformer;
        protected readonly Text Text;
        private readonly Vector2 _keyPadding = new Vector2(20, 50);
        private Fingear.IControl _control;
        public SceneNode SceneNode { get; private set; }
        public bool Clickable { get; protected set; }
        public bool KeyNameVisible { get; set; }

        public string ComputerIconAsset
        {
            get { return ComputerIcon.Asset; }
            set { ComputerIcon.Asset = value; }
        }

        public string GamePadIconAsset
        {
            get { return GamePadIcon.Asset; }
            set { GamePadIcon.Asset = value; }
        }

        public Fingear.IControl Control
        {
            get { return _control; }
            set
            {
                _control = value;

                KeyInput keyInput;
                if (_control.Inputs.Any(out keyInput))
                    return;

                Text.Content = Enum.GetName(typeof(Keys), keyInput.DisplayName);
            }
        }

        public event EventHandler Clicked;
        public event EventHandler IconChanged;

        public GuideIcon(ControlManager controlManager, IDependencyInjector injector)
            : base(injector)
        {
            _controlManager = controlManager;

            SceneNode = Add<SceneNode>();
            SpriteTransformer = Add<SpriteTransformer>();

            SpriteSheetSplit = Add<SpriteSheetSplit>();

            ComputerIcon = new SpriteSheet
            {
                Asset = ComputerIconAsset
            };
            SpriteSheetSplit.Add(ComputerIcon);

            GamePadIcon = new SpriteSheet
            {
                Asset = GamePadIconAsset
            };
            SpriteSheetSplit.Add(GamePadIcon);

            SpriteSheetSplit.ApplyCarver(new UniformCarver(1, 1));

            SpriteArea = Add<SpriteArea>();

            Text = Add<Text>();
            Text.Asset = "guide-keys";
            Text.SpriteTransformer.Color = Color.Black;

            Add<SpriteRenderer>();

            Schedulers.Initialize.Plan(InitializeLocal);
            Schedulers.LoadContent.Plan(LoadContentLocal);
            Schedulers.Update.Plan(HandleInput);
            Schedulers.Update.Plan(UpdateLocal).After(HandleInput);
        }

        private void InitializeLocal()
        {
            Visible = true;
            TransitionOpacity.Reset();
        }

        private void LoadContentLocal(ContentLibrary content)
        {
            IconChanged?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool isGamePadUsed = _controlManager.InputSources.Any<GamePadSource>();

            if (IsGamePadUsed != isGamePadUsed)
            {
                IsGamePadUsed = isGamePadUsed;

                SpriteSheetSplit.CurrentFrame = IsGamePadUsed ? 1 : 0;
                TransitionOpacity.Reset();

                IconChanged?.Invoke(this, EventArgs.Empty);
            }

            if (Clickable)
            {
                MouseControls mouseControls;
                if (_controlManager.TryGetLayer(out mouseControls))
                {
                    System.Numerics.Vector2 mouseInScreen;
                    if (mouseControls.VirtualScreenPosition.IsActive(out mouseInScreen))
                    {
                        bool hover = SpriteArea.ContainsPoint(mouseInScreen.AsMonoGameVector());

                        MenuControls menuControls;
                        if (hover && _controlManager.TryGetLayer(out menuControls) && menuControls.Clic.IsActive())
                            Clicked?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void UpdateLocal(ElapsedTime elapsedTime)
        {
            float opacity = TransitionOpacity.Update(elapsedTime.GameTime, !Visible);
            SpriteTransformer.Opacity = opacity;
            Text.SpriteTransformer.Opacity = opacity;

            if (SpriteSheetSplit.Rectangle != null)
            {
                Vector2 textScale = (SpriteSheetSplit.Rectangle.Value.Size.ToVector2() - _keyPadding) / Text.MeasureContent();
                Text.SceneNode.LocalScale = MathHelper.Min(textScale.X, textScale.Y);
            }

            Text.Visible = !IsGamePadUsed && KeyNameVisible;
        }
    }
}