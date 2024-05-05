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
        // Connect View events
        _view.Clicked += HandleClick;
       
        _view.giveMeData += RequestDataFromModel;
   
    }

    public void Run()
    {
        _view.Run();  // Start the View
    }

    private void HandleClick(object sender, EventArgs e)
    {
        // You might want to handle UI changes or other logic here when the button is clicked
        Debug.WriteLine("Start buton clicked");
    }

    private void RequestDataFromModel(ref List<string> deck, ref List<string> userHand, ref List<string> opponentHand)
    {
        _model.RequestData(ref deck, ref userHand, ref opponentHand);
    }
}
