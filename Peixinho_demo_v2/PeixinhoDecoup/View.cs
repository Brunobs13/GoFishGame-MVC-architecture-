using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Collections.Generic;

namespace PeixinhoDecoup;

public class View : Game
{
    private Texture2D startButton;
    Vector2 startButtonPosition;
    private bool isStartButtonVisible = true;
    
    private bool wasMousePressedLastFrame = false;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Model _model;
    
    List<int> rList = new List<int>();//lista para debug
    
    public EventHandler Clicked;
    public delegate void RequestForData(ref List<int> randomList);
    public event RequestForData giveMeData;
    

    public View(Model model)
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _model = model;
        
        //_model.ActionExecuted += HandleActionExecuted;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        startButtonPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // TODO: use this.Content to load your game content here
        startButton = Content.Load<Texture2D>("start_button");
        
        _font = Content.Load<SpriteFont>("File"); //carrega fonte para texto
    }
    
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // Verifica click do rato
        if (isStartButtonVisible && Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            // obtem a posição do click
            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            // cria um retangulo com o tamanho do botão
            Rectangle startButtonBounds = new Rectangle((int)(startButtonPosition.X - startButton.Width / 2), (int)(startButtonPosition.Y - startButton.Height / 2), startButton.Width, startButton.Height);
            // se a posição está dentro do botão start
            if (startButtonBounds.Contains(mousePosition))
            {
                // notifica Controller
                Console.WriteLine("Click!");
                Clicked?.Invoke(this, EventArgs.Empty);//dispara o evento

                // esconder o botao start
                isStartButtonVisible = false;
            }
            if (giveMeData != null)
            {
                Console.WriteLine("View pede lista (não sabe a quem)");
                giveMeData(ref rList);
                Console.WriteLine("View obtem nova lista");
            }
        }
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Green);

        // TODO: Add your drawing code here
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

        //imprime lista de números aleatórios no interface
        if (rList.Count != 0)
        {
            var x = 100;
            foreach (int item in rList)
            {
                _spriteBatch.DrawString(_font, item.ToString(), new Vector2(x, 100), Color.Black);
                x += 20;
            }
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    /*
    private void HandleActionExecuted(object sender, EventArgs e)
    {
        //confirmação de notificação recebida
        Console.WriteLine("View notificada da alteração de estado.");
        
    }
    */
    
}    


