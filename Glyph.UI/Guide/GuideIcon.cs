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
using Glyph.Input.StandardControls;
using Glyph.Transition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.UI.Guide
{
    public class GuideIcon : GlyphObject
    {
        private readonly ControlManager _controlManager;
        protected readonly TransitionFloat TransitionOpacity = new TransitionFloat {Start = 0, End = 1, Duration = 500};
        protected bool IsGamePadUsed;
        protected SpriteSheetSplit SpriteSheetSplit;
        protected SpriteSheet ComputerIcon;
        protected SpriteSheet GamePadIcon;
        protected SpriteArea SpriteArea;
        protected SpriteTransformer SpriteTransformer;
        protected Text Text;
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

                KeyInput keyInput = _control.Inputs.OfType<KeyInput>().FirstOrDefault();
                if (keyInput == null)
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
            if (IconChanged != null)
                IconChanged(this, EventArgs.Empty);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool isGamePadUsed = _controlManager.InputSources.OfType<GamePadSource>().Any();

            if (IsGamePadUsed != isGamePadUsed)
            {
                IsGamePadUsed = isGamePadUsed;

                SpriteSheetSplit.CurrentFrame = IsGamePadUsed ? 1 : 0;
                TransitionOpacity.Reset();

                if (IconChanged != null)
                    IconChanged(this, EventArgs.Empty);
            }

            if (Clickable)
            {
                MouseControls mouseControls;
                if (_controlManager.TryGetLayer(out mouseControls))
                {
                    Fingear.Vector2 mouseInScreen;
                    if (mouseControls.VirtualScreenPosition.IsActive(out mouseInScreen))
                    {
                        bool hover = SpriteArea.ContainsPoint(mouseInScreen.AsMonoGameVector());

                        MenuControls menuControls;
                        if (hover && _controlManager.TryGetLayer(out menuControls) && menuControls.Clic.IsActive())
                            if (Clicked != null)
                                Clicked(this, EventArgs.Empty);
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