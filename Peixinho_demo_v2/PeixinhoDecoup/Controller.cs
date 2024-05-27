using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PeixinhoDecoup;

public class Controller
{
    private View _view;
    private Model _model;
    

    public Controller()
    {
        _model = new Model();
        _view = new View(_model);
        
        _model.modelLog = new ModelLog();
        _model.modelLog.ModelLogChanged += HandleModelLogChanged;
        
        // Ligar aos eventos da view
        _view.Clicked += HandleClick;
        _view.giveMeData += RequestDataFromModel;
    }

    public void Run()
    {
        _view.Run();  // Iniciar a View
    }

    private void HandleClick(object sender, EventArgs e)
    {
        Debug.WriteLine("Start buton clicked");
    }

    private void RequestDataFromModel(ref List<string> deck, ref List<string> userHand, ref List<string> opponentHand)
    {
        _model.RequestData(ref deck, ref userHand, ref opponentHand);
    }
    private void HandleModelLogChanged(string log)
    {

        //estava a dar asneiras... a corrigir- .
       // _view.ShowErrorMessage("Erro ao gerar baralho. Gerar novo baralho? " + ex.Message);
        //Console.WriteLine(log);
        //_view.ShowErrorMessage(log);

        // Tentar reiniciar o model novamente
        //try
        //{
           
            _model = new Model();
  
        //}
        //catch (Exception ex)
       // {
            // Se a reinicialização voltar a manda a view mostrar mensagem
           // _view.ShowErrorMessage("Falha ao reiniciar o modelo. Erro: " + ex.Message);
        //}
    }
}
