using UnityEngine;
using ServerObjects;
using GPF.ServerObjects;
using GPF;
using GPF.UI.AutoComplete;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class GpfReactShim : MonoBehaviour
{
    public enum Command { SYNC, UNSYNC, SEND }

    public class React2UnityMessage
    {
        public Command cmd;
        public string soid;
        public string msgType;
        public string msgJson;
    }

    [DllImport("__Internal")]
    private static extern void OnServerObjectChange(string json);

    Syncer syncer;

    Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();

    private void Awake()
    {
        // Connect to GPF Backend
        syncer = Syncer.CreateSyncer();
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes())
            {
                if (t.IsAbstract || t.IsInterface)
                    continue;

                if (typeof(ServerObjectMessage).IsAssignableFrom(t))
                    messageTypes[t.FullName] = t;
            }
        }
    }

    private void Start()
    {
        Invoke(nameof(this.TestSync), 5);
    }

    void TestSync()
    {
        var soid = Registry.GetId<MatchPlayerSO>();
        Sync(soid);
    }

    public void OnReactMessage(string react2UnityMessageJson)
    {
        var react2UnityMessage = JsonConvert.DeserializeObject<React2UnityMessage>(react2UnityMessageJson);

        switch (react2UnityMessage.cmd)
        {
            case Command.SYNC:
                Sync(react2UnityMessage.soid);
                break;
            case Command.UNSYNC:
                Unsync(react2UnityMessage.soid);
                break;
            case Command.SEND:
                Send(react2UnityMessage.soid, react2UnityMessage.msgType, react2UnityMessage.msgJson);
                break;
        }
    }

    void Sync(string soid)
    {
        syncer.PreSync(soid);
        syncer.AddListener<ServerObject>(soid, OnServerObjectChanged);
    }

    void Unsync(string soid)
    {
        syncer.RemoveListener<ServerObject>(soid, OnServerObjectChanged);
        syncer.UnSync(soid);
    }

    void Send(string soid, string msgType, string msgJson)
    {
        var type = messageTypes[msgType];
        var msg = JsonConvert.DeserializeObject(msgJson, type) as ServerObjectMessage;
        syncer.Send(soid, msg);
    }

    void OnServerObjectChanged(ServerObject so)
    {
        var json = JsonConvert.SerializeObject(so);
        OnServerObjectChange(json);
    }

    private void OnApplicationPause(bool pause)
    {
        if (syncer != null)
        {
            if (pause)
                syncer.Pause();
            else
                syncer.Resume();
        }
    }

    private async void OnApplicationQuit()
    {
        if (syncer != null)
            await syncer.Disconnect();
    }
}