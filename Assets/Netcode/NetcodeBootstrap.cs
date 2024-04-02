using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.NetCode;

[Preserve]
public class NetcodeBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWordlName)
    {
        AutoConnectPort = 7777;
        return base.Initialize(defaultWordlName);
    }
}
