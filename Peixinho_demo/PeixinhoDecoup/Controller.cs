using System;
    
namespace PeixinhoDecoup;

public class Controller
{
    private View _view;
    private Model _model;

    public Controller()
    {
        _model = new Model();
        _view = new View(_model);
       
        _view.StartButtonClicked += StartGame;
        _view.PlayerCardClicked += OnPlayerCardClicked;
        _view.giveMeHands += _model.RequestPlayer1Hand;
        
    }
    public void Run()
    {
        _view.Run();//Ativar View
    }
    private void StartGame(object sender, EventArgs e)//metodo para gerir evento
    {
        // gerir o evento do click
        Console.WriteLine("Controller é notificado do click");
        //recebeu notificação manda executar açao
        _model.StarGame();
    }

    private void OnPlayerCardClicked(object sender, CardEventArgs e)
    {
        _model.checkmatch(e.CardName);
    }
}
