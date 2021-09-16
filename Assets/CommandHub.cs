using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    GameSimulator gameSimulator;
    void Start()
    {
        Debug.Log("Hello World!");

        connection = new HubConnectionBuilder()
            .WithUrl(baseURL)
            .Build();
        connection.Closed += async (error) =>
        {
            await Task.Delay(1000);
            await connection.StartAsync();
        };
        gameSimulator = GetComponent<GameSimulator>();
        Connect();
    }
    void OnReceiveMessage(Command command, string jsonStr)
    {
        lock (mainThreadFn)
        {
            mainThreadFn.Add(() =>
            {
                gameSimulator.OnReceiveCommand(command, jsonStr);
            });
        }
    }
    private async void Connect()
    {
        connection.On<Command, string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();

        gameSimulator.RequestLogin();
    }
    public string message = "Hello!";

    public void SendToServer(RequestMsg request)
    {
        request.userID = UserData.Instance.userinfo.id;
        string json = JsonConvert.SerializeObject(request);
        connection.InvokeAsync("SeverReceiveMessage", request.command, json);
    }

    List<Action> mainThreadFn = new List<Action>();

    private void Update()
    {
        lock (mainThreadFn)
        {
            if (mainThreadFn.Count > 0)
            {
                foreach (var item in mainThreadFn)
                {
                    item();
                }
                mainThreadFn.Clear();
            }
        }
    }
}