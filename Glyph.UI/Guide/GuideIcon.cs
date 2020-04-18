using System;
using System.Threading.Tasks;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Carvers;
using Glyph.Graphics.Renderer;
using Glyph.Transition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Diese.Collections;
using Fingear.Inputs;

namespace Glyph.UI.Guide
{
    public class GuideIcon : GlyphObject
    {
        protected readonly TransitionFloat TransitionOpacity = new TransitionFloat {Start = 0, End = 1, Duration = 500};
        protected bool IsGamePadUsed;
        protected readonly SpriteSheetSplit SpriteSheetSplit;
        protected readonly SpriteSheet ComputerIcon;
        protected readonly SpriteSheet GamePadIcon;
        protected readonly SpriteArea SpriteArea;
        protected readonly SpriteTransformer SpriteTransformer;
        protected readonly Text Text;
        private readonly Vector2 _keyPadding = new Vector2(20, 50);

        public SceneNode SceneNode { get; }
        public bool Clickable { get; set; }
        public bool KeyNameVisible { get; set; }

        public string ComputerIconAsset
        {
            get => ComputerIcon.Asset;
            set => ComputerIcon.Asset = value;
        }

        public string GamePadIconAsset
        {
            get => GamePadIcon.Asset;
            set => GamePadIcon.Asset = value;
        }

        private Fingear.Controls.IControl _control;
        public Fingear.Controls.IControl Control
        {
            get => _control;
            set
            {
                _control = value;

                if (_control.Inputs.AnyOfType(out KeyInput keyInput))
                    return;

                Text.Content = Enum.GetName(typeof(Keys), keyInput.DisplayName);
            }
        }

        public event EventHandler Clicked;
        public event EventHandler IconChanged;

        public GuideIcon(GlyphResolveContext context)
            : base(context)
        {
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

            var userInterface = Add<UserInterface>();
            userInterface.TouchStarted += OnTouchStarted;

            Schedulers.Initialize.Plan(InitializeLocal).AtEnd();
            Schedulers.LoadContent.Plan(LoadContentLocal);
            Schedulers.Update.Plan(HandleInput);
            Schedulers.Update.Plan(UpdateLocal).After(HandleInput);
        }

        private void InitializeLocal()
        {
            Visible = true;
            TransitionOpacity.Reset();
        }

        private async Task LoadContentLocal(IContentLibrary content)
        {
            IconChanged?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool isGamePadUsed = InputManager.Instance.InputSources.AnyOfType<GamePadSource>();
            if (IsGamePadUsed == isGamePadUsed)
                return;

            IsGamePadUsed = isGamePadUsed;
            SpriteSheetSplit.CurrentFrame = IsGamePadUsed ? 1 : 0;
            TransitionOpacity.Reset();

            IconChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
        {
            if (!Clickable)
                return;
            if (!SpriteArea.ContainsPoint(e.CursorPosition))
                return;

            e.Handle();
            Clicked?.Invoke(this, EventArgs.Empty);
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