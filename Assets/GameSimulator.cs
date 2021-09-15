using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSimulator : MonoBehaviour
{
    CommandHub commandHub;
    // Start is called before the first frame update
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();
    }

    public void RequestLogin()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }

    private void SendToServer(RequestLogin request)
    {
        commandHub.SendToServer(request);
    }

    public void ResultLogin(string jsonStr)
    {
        ResultLogin resultLogin = JsonUtility.FromJson<ResultLogin>(jsonStr);
        print(resultLogin.userinfo.Gold);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
