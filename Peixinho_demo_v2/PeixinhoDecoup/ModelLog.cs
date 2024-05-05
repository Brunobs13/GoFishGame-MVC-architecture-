using System;

namespace PeixinhoDecoup;

public class ModelLog
{
    public delegate void ModelLogChangedEventHandler(string log);
    public event ModelLogChangedEventHandler ModelLogChanged;
    
    public void InitializeDeckError()
    {
        
        string log = "Erro ao inicializar o baralho de jogo!";
        Console.WriteLine(log);
        ModelLogChanged?.Invoke(log);
    }
}