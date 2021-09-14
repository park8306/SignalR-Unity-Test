using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
internal class JsonIncludeAttribute : Attribute
{
}
#else
using System.Text.Json.Serialization;
#endif
public enum Command
{
    None,
    // 로그인
    RequestLogin    = 1,
    ResultLogin     = 2,

    // 스테이지 엔터/클리어
    RequestStageEnter = 10,
    ResultStageEnter  = 11,
    RequestStageClear = 12,
    ResultStageClear  = 13,

    // 아이템 구입
    RequestBuyItem = 20,
    ResultBuyItem = 21,

    // 
}
//public class MessageStruct : MonoBehaviour
//{


//}
[Serializable]
public class MsgHeader
{
    public MsgHeader(Command _command)
    {
        command = _command;
    }
    [JsonInclude]
    public Command command;
}
[Serializable]
public class RequestMsg : MsgHeader
{
    public RequestMsg(Command _command) : base(_command) { }
}

[Serializable]
public class ResultMsg : MsgHeader
{
    public ResultMsg(Command command) : base(command) { }
    [JsonInclude]
    public int result;
}
public class RequestLogin : RequestMsg
{
    public string deviceID;

    public RequestLogin() : base(Command.RequestLogin) { }
}

public class ResultLogin : RequestMsg
{
    public ResultLogin() : base(Command.ResultLogin) { }
    [JsonInclude]
    public int id;
    [JsonInclude]
    public int gold;
}//