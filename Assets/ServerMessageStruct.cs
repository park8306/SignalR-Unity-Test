
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Blazor.DB { }

[Serializable]
public partial class Userinfo
{
    public int Id;
    public int Gold;
    public int Highscore;
}

[Serializable]
public partial class Account
{
    public int Id;
    public string Deviceid;
    public DateTime Lastlogin;
    public DateTime CreateTime;
    public DateTime ModificationTime;
}