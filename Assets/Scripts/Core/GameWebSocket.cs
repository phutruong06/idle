using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using System.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;
using System.Dynamic;
using deVoid.Utils;
using IdleGunner.UI;

[System.Serializable]
internal class SerializeWebSocketCollection : SerializableDictionaryBase<string, WebSocket> { }

public class AppWebSocket : MonoBehaviour
{
    public static class Paths
    {
        /// <summary>
        /// Path for ping the connection
        /// </summary>
        public const string ping = "/";

        public const string fetchTask = "/tasks";
    }


    [SerializeField] private string m_appWebSocketServer = "127.0.0.1";

#if UNITY_EDITOR
    [Header("TESTING!")]
    [SerializeField] private string m_path = "/";
    [SerializeField] private string m_send = "";
    [Button("Connect Once!", EButtonEnableMode.Playmode)]
    private async void ConnectOnce()
    {
        var websocket = await CreateConnectionOnce(m_path);
        await websocket.Connect();
    }
    [Button("Connect!", EButtonEnableMode.Playmode)]
    private async void Connect()
    {
        var websocket = await CreateConnection(m_path);
        await websocket.Connect();
        await websocket.SendText(m_send);
    }
    [Button("Send Message!", EButtonEnableMode.Playmode)]
    private async void Send()
    {
        TryGetConnection(m_path, out var websocket);
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(m_send);
        }
    }
#endif

    private static string gameServerIp;
    private static SerializeWebSocketCollection websocketConnections = new SerializeWebSocketCollection();

#if UNITY_EDITOR
    [SerializeField] private SerializeWebSocketCollection _websocketConnections = new SerializeWebSocketCollection();
#endif

    private void OnEnable()
    {
        gameServerIp = m_appWebSocketServer;
        websocketConnections.Clear();
        websocketConnections = new SerializeWebSocketCollection();
    }
    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        try
        {
            foreach (var webSocket in websocketConnections)
            {
                webSocket.Value.DispatchMessageQueue();
                Debug.Log($"Websocket to path:'{webSocket.Key}' is {webSocket.Value.State}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
#endif

#if UNITY_EDITOR
        _websocketConnections = websocketConnections;
#endif
    }
    private async void OnApplicationQuit()
    {
        try
        {
            foreach (var webSocket in websocketConnections)
            {
                await webSocket.Value.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public static async Task<WebSocket> CreateConnection(string path)
    {
        if(TryGetConnection(path, out var socket))
        {
            return socket;
        }

        Debug.Log($"Creating connection to 'wss://{gameServerIp}{path}'");

#if UNITY_EDITOR
        var webSocket = new WebSocket($"ws://{gameServerIp}{path}");
#else
        var webSocket = new WebSocket($"wss://{gameServerIp}{path}");
#endif

        Debug.Log($"Created connection to 'wss://{gameServerIp}{path}'");
        webSocket.OnOpen += () =>
        {
            Debug.Log($"Connected to '{gameServerIp}:{path}'");
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            websocketConnections.Remove(path);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log($"Connection to socket closed with close code '{e}'");
            websocketConnections.Remove(path);
        };

        webSocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Server Response: " + message);
        };

        websocketConnections.Add(path, webSocket);
        return webSocket;
    }
    public static async Task<WebSocket> CreateConnectionOnce(string path)
    {
        if (TryGetConnection(path, out var socket))
        {
            return socket;
        }

        Debug.Log($"Creating connection once to 'wss://{gameServerIp}{path}'");

#if UNITY_EDITOR
        var webSocket = new WebSocket($"ws://{gameServerIp}{path}");
#else
        var webSocket = new WebSocket($"wss://{gameServerIp}{path}");
#endif

        Debug.Log($"Created connection once to 'wss://{gameServerIp}{path}'");

        webSocket.OnOpen += () =>
        {
            Debug.Log($"Connected once to '{gameServerIp}:{path}'");
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            websocketConnections.Remove(path);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log($"Connection to socket closed with close code '{e}'");
            websocketConnections.Remove(path);
        };

        webSocket.OnMessage += async (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);

            await webSocket.Close();
        };

        websocketConnections.Add(path, webSocket);
        return webSocket;
    }
    public static WebSocket GetConnection(string path)
    {
        return websocketConnections[path];
    }
    public static bool TryGetConnection(string path, out WebSocket webSocket)
    {
        return websocketConnections.TryGetValue(path, out webSocket);
    }
    public static async Task CloseConnection(string path)
    {
        if(TryGetConnection(path, out var conn))
        {
            await conn.Close();
            websocketConnections.Remove(path);
        }
        else
        {
            Debug.LogWarning($"There no connection with the path: '{path}'");
        }
    }
}

public static class WebSocketPopup
{
    private static Dictionary<WebSocket, PopupBase> hub = new Dictionary<WebSocket, PopupBase>();

    public static void SetPopupOnConnect<T>(this WebSocket webSocket, T popup) where T : PopupBase
    {
        hub.Add(webSocket, popup);

        webSocket.OnOpen += OnOpenRegisterEvent;
    }
    public static void SetRemovePopupOnConnect<T>(this WebSocket webSocket, T popup) where T : PopupBase
    {
        webSocket.OnOpen -= OnOpenRegisterEvent;
    }


    public static void SetShowPopupOnErrort<T>(this WebSocket webSocket, T popup) where T : PopupBase
    {

    }
    public static void SetShowPopupOnCloset<T>(this WebSocket webSocket, T popup) where T : PopupBase
    {

    }
    public static void SetShowPopupOnMessaget<T>(this WebSocket webSocket, T popup) where T : PopupBase
    {

    }

    private static void OnOpenRegisterEvent()
    {
    }
}

public static class DynamicHelper
{
    public static bool DoesPropertyExist(dynamic settings, string name)
    {
        if (settings is ExpandoObject eo)
            return (eo as IDictionary<string, object>).ContainsKey(name);

        return settings.GetType().GetProperty(name) != null;
    }
}