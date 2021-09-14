using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    private float x;
    private float y;
    private static HubConnection connection;
    public string severURL = "https://localhost:44333/chat";
    public string userName = "UserA";
    void Start()
    {
        Debug.Log("Hello World!");
        connection = new HubConnectionBuilder()
            .WithUrl(severURL)
            .Build();
        connection.Closed += async (error) =>
        {
            await Task.Delay(Random.Range(0, 5) * 1000);
            await connection.StartAsync();
        };

        Connect();
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Send($"{objPosition.x}, {objPosition.y}");
        //objPosition.z = transform.position.z;
        //transform.position = objPosition;
    }

    private async void Connect()
    {
        connection.On<string, string>("ClientReceiveMessage", OnReceiveMessage);

        try
        {
            await connection.StartAsync();

            Debug.Log("Connection started");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    void OnReceiveMessage(string name, string message)
    {
        Debug.Log($"{name}: {message}");

        string[] posString = message.Split(',');
        if (posString.Length != 2)
            return;

        float.TryParse(posString[0], out float x);
        float.TryParse(posString[1], out float y);
        mainThreadFn.Add(
                () => SetPosition(x, y)
            );
    }

    private async void Send(string msg)
    {
        try
        {
            await connection.InvokeAsync("ServerReceiveMessage", userName, msg);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void SetPosition(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    List<Action> mainThreadFn = new List<Action>();
    private void Update()
    {
        lock (mainThreadFn)
        {
            foreach (var item in mainThreadFn)
                item();
            mainThreadFn.Clear();
        }
    }
}