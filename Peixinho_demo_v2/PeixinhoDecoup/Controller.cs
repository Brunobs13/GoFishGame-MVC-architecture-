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
        //ligar a evento na view
        _view.Clicked += HandleClick;
        _view.giveMeData += _model.RequestData;
    }
    public void Run()
    {
        _view.Run();//Ativar View
    }
    private void HandleClick(object sender, EventArgs e)//metodo para gerir evento
    {
        // gerir o evento do click
        Console.WriteLine("Controller é notificado do click");
        //recebeu notificação manda executar açao
        _model.ExecuteAction();
    }
}
