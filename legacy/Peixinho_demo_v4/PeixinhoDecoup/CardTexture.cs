using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace PeixinhoDecoup;

public class CardTexture
{
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width/2, Texture.Height/2);
    public bool IsClicked { get; set; }
    public bool IsClickable { get; set; } 
    public float Scale { get; set; } 

    public CardTexture(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
        IsClicked = false;
        IsClickable = true;
        Scale = 0.6f;
    }
    
    private Rectangle CalculateScaledBounds()
    {
        int scaledWidth = (int)(Texture.Width * Scale);
        int scaledHeight = (int)(Texture.Height * Scale);
        return new Rectangle((int)Position.X, (int)Position.Y, scaledWidth, scaledHeight);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.Draw(Texture, Position,Color.White);
        spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
   
    }

    public bool ContainsPoint(Point point)
    {
        return Bounds.Contains(point);
    }
    
    public bool IsClickableArea(Point point, bool isTopmostCard, int visiblePartWidth)
    {
        if (!IsClickable) 
            return false; 
        
        if (isTopmostCard)
        {
            return Bounds.Contains(point); // carta de cima
        }
        else
        {
            //cartas parcialmente cobertas
            Rectangle visibleArea = new Rectangle((int)Position.X, (int)Position.Y, visiblePartWidth, Texture.Height);
            return visibleArea.Contains(point);
        }
    }
}
