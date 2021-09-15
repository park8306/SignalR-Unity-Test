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
    public string baseURL = "https://localhost:5001/command";
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

        Connect();
    }
    void OnReceiveMessage(Command command, string jsonStr)
    {
        lock (mainThreadFn)
        {
            mainThreadFn.Add(() =>
            {
                OnReceiveCommand(command, jsonStr);
            });
        }
    }

    private void OnReceiveCommand(Command command, string jsonStr)
    {
        switch (command)
        {
            case Command.ResultLogin:
                GetComponent<GameSimulator>().ResultLogin(jsonStr);
                break;
            default:
                Debug.LogError($"{command}:아직 구현하지 안은 메시지입니다");
                break;
        }
    }

    private async void Connect()
    {
        connection.On<Command, string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();


        GetComponent<GameSimulator>().RequestLogin();
    }
    public string message = "Hello!";

    public void SendToServer(RequestMsg request)
    {
        request.userID = UserData.Instance.userinfo.Id;
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