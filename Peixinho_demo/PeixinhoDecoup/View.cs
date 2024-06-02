using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PeixinhoDecoup;

public class View : Game
{
    private Texture2D startButton;
    Vector2 startButtonPosition;
    private Texture2D tableCard;
    private Vector2 cardPosition;
    
    private bool isStartButtonVisible = true;
    private bool isCardVisible = true;
    
    private int score = 0;
    
    private bool wasMousePressedLastFrame = false;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Model _model;

    private List<Microsoft.Xna.Framework.Rectangle> cardBounds = new List<Microsoft.Xna.Framework.Rectangle>();
    private Microsoft.Xna.Framework.Rectangle cardBound;        
    List<Texture2D> cards;
    
    List<string> cardsNames = new List<string>();
    
    
    List<ArrayList> rList = new List<ArrayList>();
    
    List<CardTexture> cardTextures = new List<CardTexture>();
    private CardTexture _card;

    private CardTexture CardBackTexture ;
    
    private bool matchFound = false;
    private string matchMessage = "";
    
    private bool needRedraw = false;

    RenderTarget2D smallerRenderTarget;
    private int p1Score;
    private int p2Score;
    

    
    public EventHandler StartButtonClicked;
    public event EventHandler<CardEventArgs> PlayerCardClicked;
    public delegate void RequestForData(ref List<ArrayList> list);
    public event RequestForData giveMeHands;
    
    
    private IViewModel gameModel;
    
    
    public View(Model model)
    {
        //gameModel = model;
        //gameModel.MatchChecked += MatchChecked;
        
        
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _model = model;
        
        // tamanho da janela
        _graphics.PreferredBackBufferWidth = 1400;  
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.ApplyChanges();
        
        //_model.ActionExecuted += HandleActionExecuted;
        
    }
    
    /*private void DisplayPlayerScores()
    {
        int p1Score = gameModel.GetPlayerScore(1);
        int p2Score = gameModel.GetPlayerScore(2);

        // Use p1Score and p2Score as needed
        Console.WriteLine($"Player 1 score: {p1Score}");
        Console.WriteLine($"Player 2 score: {p2Score}");
    }*/

    
    private void MatchChecked(object sender, Model.MatchFoundEventArgs e)
    {
        matchMessage = matchFound ? "Match found!" : "Go Fish.";
        Console.WriteLine(matchFound);
        Console.WriteLine(matchMessage);
        //needRedraw = true;
    }
    
  

    protected override void Initialize()
    {
        
        cards = new List<Texture2D>();
        // TODO: Add your initialization logic here
        startButtonPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);

        cardPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        LoadTextures();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        startButton = Content.Load<Texture2D>("buttons/start_button");
        
        _font = Content.Load<SpriteFont>("Buttons/File"); //carrega fonte para texto
    }
    private void LoadTextures()
    {
       
        int x = 30; 
        int y = 30; 
        int spacing = 100;
 
        cardTextures.Clear();
        
        // Set render target
        GraphicsDevice.SetRenderTarget(smallerRenderTarget);
        GraphicsDevice.Clear(Color.Transparent);
        bool flipCard = false;
        foreach (var hand in rList)
        {
            foreach (Card card in hand)
            {
                var cardName = card.CardName;
                if (flipCard)
                {
                    cardName = "Buttons/Back";
                }
                
                Texture2D cardTexture = Content.Load<Texture2D>(cardName);
                cardTexture.Name = card.CardName;
                var cardPosition = new Vector2(x, y);
        
                CardTexture _card = new CardTexture(cardTexture, cardPosition);
                Console.WriteLine(_card.Position);
                if (flipCard)
                {
                    _card.IsClickable = false;
                }
                
                cardTextures.Add(_card);
                x += spacing;
            }

            flipCard = true;//mudar para true para esconder cartas computador
            x = 30;
            y += 700; 
        }


            Texture2D cardBackTexture = Content.Load<Texture2D>("buttons/back");
            Vector2 cardBackPosition = new Vector2(30, 300);
            CardBackTexture =  new CardTexture(cardBackTexture,cardBackPosition);
            CardBackTexture.IsClicked = true;
    }
    
    protected override void Update(GameTime gameTime)
    {
        //DisplayPlayerScores();
        if (needRedraw)
        {
            // Redraw immediately
            Draw(gameTime);
            needRedraw = false;
        }
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        var mouseState = Mouse.GetState();
        var mousePosition = new Point(mouseState.X, mouseState.Y);
        
        if (isStartButtonVisible && mouseState.LeftButton == ButtonState.Pressed)
        {
            Rectangle startButtonBounds = new Rectangle((int)(startButtonPosition.X - startButton.Width / 2), 
                (int)(startButtonPosition.Y - startButton.Height / 2), startButton.Width, startButton.Height);
           
            if (startButtonBounds.Contains(mousePosition))
            {
                StartButtonClicked?.Invoke(this, EventArgs.Empty);//dispara evento inicio jogo
                isStartButtonVisible = false;
            }
            
            if (giveMeHands != null)
            {
                giveMeHands(ref rList);
                LoadTextures();
            }
        }
        cardBounds.Clear();

        if (!isStartButtonVisible && mouseState.LeftButton == ButtonState.Pressed)
        {
            Console.WriteLine($"Mouse position: ({mousePosition.X}, {mousePosition.Y})");
            
            
            foreach (CardTexture cardT in cardTextures)
            {
                Console.WriteLine($"Mouse position: ({mousePosition.X}, {mousePosition.Y})");
                //Console.WriteLine($" Card Bound: ({bound.X}, {bound.Y}) and size ({bound.Width}, {bound.Height})");
                if (cardT.Bounds.Contains(mousePosition) && cardT.IsClickable)
                {
                    giveMeHands(ref rList);
                    
                    // Console.WriteLine(isCardVisible);
                    Console.WriteLine(cardT.Texture.Name);
                    //isCardVisible = false;
                    cardT.IsClicked = true;
                    Console.WriteLine("Mouse is within a cardBound.");
                    PlayerCardClicked?.Invoke(this, new CardEventArgs(cardT.Texture.Name));

                    break;
                }
            }
            LoadTextures();
        }
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear((Color)Color.Green);
        
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, matchMessage, new Vector2(500, 500), Color.Black);
        _spriteBatch.End();
        
        _spriteBatch.Begin();
        
        if (isStartButtonVisible)
        {
            _spriteBatch.Draw(
                startButton,
                startButtonPosition,
                null,
                Color.White,
                0f,
                new Vector2(startButton.Width / 2, startButton.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        _spriteBatch.End();
        
        foreach (CardTexture _cardTexture in cardTextures)
        {
            if(!_cardTexture.IsClicked)
            {
                _spriteBatch.Begin();
                _cardTexture.Draw(_spriteBatch);
                _cardTexture.IsClicked = false;
                _spriteBatch.End();
            }
            
        }
        
        int x = 30; 
        int y = 350;
        int cardInDeck = Deck.GetCardCount();
        
        if (!isStartButtonVisible){
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, "Player 1 Score: "+ p1Score, new Vector2(30, 270), Color.Black);
            _spriteBatch.DrawString(_font, "Player 2 Score: "+p2Score, new Vector2(30, 670), Color.Black);
            _spriteBatch.End();
            for (int i = 0; i <= cardInDeck ; i++)
                {
                    
                    CardBackTexture.Position = new Vector2(x, y);
                    _spriteBatch.Begin();
                    CardBackTexture.Draw(_spriteBatch);
                    //CardBackTexture.IsClicked = false;
                    _spriteBatch.End();
                    x += 3;
                    
                }
        }
        
        base.Draw(gameTime);
        
    }
    
}


public class CardEventArgs : EventArgs
{
    public string CardName { get; private set; }

    public CardEventArgs(string cardName)
    {
        CardName = cardName;
    }
}

