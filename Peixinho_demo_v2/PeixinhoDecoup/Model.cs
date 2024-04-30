using System;
using System.Collections.Generic;
    
namespace PeixinhoDecoup;

public class Model
{
    List<int> randomList = new List<int>();
    public void ExecuteAction()
    {
        Console.WriteLine("model executa ação por ordem do Controller");
        Random random = new Random();
        //gera lista de numeros aleatórios
        for (int i = 0; i < 10; i++)
        {
            randomList.Add(random.Next(51));
        }
        //Console.WriteLine("randomList"+randomList.Count);
    }
    
    public void RequestData(ref List<int> list)
    {
        Console.WriteLine("Por ordem do controller Model disponibliza a lista (mas não sabe para quem) ");
        list = new List<int>();
        //faz deepcopy para a nova lista
        foreach (int item in randomList)
       {
           list.Add(item);
       }
       //Console.WriteLine("copiedList"+list.Count);
        
    }

}