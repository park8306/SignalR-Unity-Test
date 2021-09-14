using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    void Start()
    {
        Debug.Log("Hello World!");

        transform.Find("Button").GetComponent<Button>().onClick.AddListener(Send);

        connection = new HubConnectionBuilder()
            .WithUrl(baseURL)
            .Build();
        connection.Closed += async (error) =>
        {
            await Task.Delay(1000);
            await connection.StartAsync();
        };

        Connect();
    }
    void OnReceiveMessage(string message)
    {
        lock (mainThreadFn)
        {
            mainThreadFn.Add(() =>
            {
                Debug.Log($"{message}" + "!!!!");
                Debug.Log(transform.name);
                Debug.Log(transform.position);
            });
        }
    }

    private async void Connect()
    {
        connection.On<string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();
    }
    public string message = "Hello!";
    private void Send()
    {
        connection.InvokeAsync("ServerReceiveMessage", message);
    }
    //
    List<Action> mainThreadFn = new List<Action>();
    private void Update()
    {
        lock (mainThreadFn)
        {
            if(mainThreadFn.Count>0)
            { 
                foreach (var item in mainThreadFn)
                    item();
                mainThreadFn.Clear();
            }
        }
    }
}