using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameSimulator : MonoBehaviour
{
    class CommandInfo
    {
        public string name;
        public Command requestCommand;

        public Command resultCommand;
        public UnityAction requestFn;
        public UnityAction<string> resultFn;

        public CommandInfo(string name, Command requestCommand, Command resultCommand, UnityAction requestFn, UnityAction<string> resultFn)
        {
            this.name = name;
            this.requestCommand = requestCommand;
            this.resultCommand = resultCommand;
            this.requestFn = requestFn;
            this.resultFn = resultFn;
        }
    }

    CommandHub commandHub;
    public Button baseButton;
    List<CommandInfo> commandInfo = new List<CommandInfo>();
    // Start is called before the first frame update
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();
        baseButton = GetComponentInChildren<Button>();
        commandInfo.AddRange(new CommandInfo[] {
            new CommandInfo("로그인", Command.RequestLogin, Command.ResultLogin, RequestLogin, ResultLogin),
            new CommandInfo("보상", Command.RequestReward, Command.ResultReward, RequestReward, ResultReward)
        });

        foreach (var item in commandInfo)
        {
            var newButton = Instantiate(baseButton, baseButton.transform.parent);
            newButton.GetComponentInChildren<Text>().text = item.name;
            newButton.onClick.AddListener(item.requestFn);
        }
        baseButton.gameObject.SetActive(false);
    }
    #region 로그인
    public void RequestLogin()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }
    public void ResultLogin(string jsonStr)
    {
        ResultLogin result = JsonUtility.FromJson<ResultLogin>(jsonStr);
        if (ReturnErrorExist(result.result))
            return;
        

        print(result.userinfo.Gold);
        UserData.Instance.userinfo = result.userinfo;
        UserData.Instance.account = result.account;
    }

    private bool ReturnErrorExist(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogError(result);
            return true;
        }
        return false;
    }
    #endregion 로그인
    private void SendToServer(RequestMsg request)
    {
        commandHub.SendToServer(request);
    }

    #region 보상 요청

    public string rewardType = "100Gold";
    void RequestReward()
    {
        RequestReward request = new RequestReward();
        request.rewardType = rewardType;
        SendToServer(request);
    }
    void ResultReward(string jsonStr)
    {
        ResultReward result = JsonConvert.DeserializeObject<ResultReward>(jsonStr);
        if(result.result != ErrorCode.Succeed)
        {
            Debug.LogError(result.result);
            return;
        }

        print(result.rewardGold);
        print(result.currentGold);
        UserData.Instance.userinfo.Gold = result.currentGold;
    }
    #endregion
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
