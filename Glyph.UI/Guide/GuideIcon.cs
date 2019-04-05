using System;
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
using Fingear;
using Glyph.Core.Inputs;

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
        private Fingear.IControl _control;
        private readonly ProjectionCursorControl _sceneCursor;
        private readonly IControl<InputActivity> _clic;
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

                if (_control.Inputs.AnyOfType(out KeyInput keyInput))
                    return;

                Text.Content = Enum.GetName(typeof(Keys), keyInput.DisplayName);
            }
        }

        public event EventHandler Clicked;
        public event EventHandler IconChanged;

        public GuideIcon(GlyphInjectionContext context, RootView rootView, ProjectionManager projectionManager)
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

            var controls = Add<Core.Inputs.Controls>();
            controls.AddMany(new Fingear.IControl[]
            {
                _sceneCursor = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, rootView, new ReadOnlySceneNodeDelegate(SceneNode.RootNode), projectionManager),
                _clic = MenuControls.Instance.Clic
            });

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
            bool isGamePadUsed = InputManager.Instance.InputSources.AnyOfType<GamePadSource>();

            if (IsGamePadUsed != isGamePadUsed)
            {
                IsGamePadUsed = isGamePadUsed;

                SpriteSheetSplit.CurrentFrame = IsGamePadUsed ? 1 : 0;
                TransitionOpacity.Reset();

                IconChanged?.Invoke(this, EventArgs.Empty);
            }

            if (Clickable)
            {
                _sceneCursor.IsActive(out System.Numerics.Vector2 mouseInScreen);
                bool hover = SpriteArea.ContainsPoint(mouseInScreen.AsMonoGameVector());
                if (hover && _clic.IsActive())
                    Clicked?.Invoke(this, EventArgs.Empty);
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