using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PeixinhoDecoup;

public class View : Game
{
    private Texture2D startButton;
    private Vector2 startButtonPosition;
    private bool isStartButtonVisible = true;
    private bool isGameStarted = false;  // Flag to control game started state

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Model _model;

    private List<string> deck = new List<string>();
    private List<string> userHand = new List<string>();
    private List<string> opponentHand = new List<string>();

    public EventHandler Clicked;
    public delegate void RequestForData(ref List<string> deck, ref List<string> userHand, ref List<string> opponentHand);
    public event RequestForData giveMeData;

    public View(Model model)
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _model = model;
    }

    protected override void Initialize()
    {
        startButtonPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        startButton = Content.Load<Texture2D>("start_button");
        _font = Content.Load<SpriteFont>("File");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var mouseState = Mouse.GetState();
        if (isStartButtonVisible && mouseState.LeftButton == ButtonState.Pressed)
        {
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            Rectangle startButtonBounds = new Rectangle((int)(startButtonPosition.X - startButton.Width / 2),
                                                        (int)(startButtonPosition.Y - startButton.Height / 2),
                                                        startButton.Width, startButton.Height);

            if (startButtonBounds.Contains(mousePosition))
            {
                isStartButtonVisible = false;
                isGameStarted = true;  // Set flag to true as the game is started
                Clicked?.Invoke(this, EventArgs.Empty);

                if (giveMeData != null)
                {
                    giveMeData(ref deck, ref userHand, ref opponentHand);
                }
            }
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        if (isStartButtonVisible)
        {
            _spriteBatch.Draw(startButton, startButtonPosition, null, Color.White, 0f,
                new Vector2(startButton.Width / 2, startButton.Height / 2), Vector2.One,
                SpriteEffects.None, 0f);
        }

        // Draw cards only if the game has started
        if (isGameStarted)
        {
            DrawCards(userHand, new Vector2(0, 100), "User Hand:");
            DrawCards(opponentHand, new Vector2(0, 200), "Opponent Hand:");
            DrawCards(deck, new Vector2(0, 300), "Deck:");
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawCards(List<string> cards, Vector2 position, string label)
    {
        _spriteBatch.DrawString(_font, label, position, Color.Black);
        position.X += 80;  // Offset for label

        foreach (string card in cards)
        {
            _spriteBatch.DrawString(_font, card, position, Color.Black);
            position.X += 100; // Space between cards
        }
    }
}
