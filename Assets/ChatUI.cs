using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text textBaseItem;
    public Text currentChaanel;
    public InputField chatInputField;
    public InputField newChaanelInputField;
    public Button sendButton;
    public Button changeChannelButton;
    GameSimulator gameSimulator;

    void Awake()
    {
        // 채팅 메시지 받으면 화면 출력하자
        textBaseItem.gameObject.SetActive(false);
        sendButton.onClick.AddListener(RequestSendMessage);
        changeChannelButton.onClick.AddListener(RequestChangeChannel);


        gameSimulator = GetComponentInParent<GameSimulator>();
        var commandInfos = gameSimulator.commandInfos;
        commandInfos[Command.ResultSendMessage] = new CommandInfo("메시지 보내기", null, ResultSendMessage);
        commandInfos[Command.ResultChangeChannel] = new CommandInfo("채널 변경", null, ResultChangeChannel);
    }

    public bool allowSendEnter;
    private void Update()
    {
        if (allowSendEnter && (chatInputField.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            RequestSendMessage();
            allowSendEnter = false;
        }
        else
            allowSendEnter = chatInputField.isFocused;
    }

    private void SendToServer(RequestMsg request)
    {
        gameSimulator.SendToServer(request);
    }

    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }
    private void RequestSendMessage()
    {
        RequestSendMessage request = new RequestSendMessage();
        request.newMessage = chatInputField.text;
        chatInputField.text = string.Empty;
        SendToServer(request);
    }

    private void ResultSendMessage(string jsonStr)
    {
        ResultSendMessage result = JsonConvert.DeserializeObject<ResultSendMessage>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        string sendName = result.senderName;
        string message = result.message;
        TestAddMessage($"{sendName} : {message}");
    }

   

    private void RequestChangeChannel()
    {
        RequestChangeChannel request = new RequestChangeChannel();
        request.newChannelName = newChaanelInputField.text;
        chatInputField.text = string.Empty;
        SendToServer(request);
    }

    private void ResultChangeChannel(string jsonStr)
    {
        ResultChangeChannel result = JsonConvert.DeserializeObject<ResultChangeChannel>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        string newChannelName = result.newChannelName;
        currentChaanel.text = newChannelName;
    }

    void TestAddMessage(string message)
    {
        Text newText = Instantiate(textBaseItem, textBaseItem.transform.parent);
        newText.text = message;
        newText.gameObject.SetActive(true);

        ContentSizeFitter csf = newText.GetComponent<ContentSizeFitter>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}
