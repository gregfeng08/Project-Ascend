using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServerSubscriber : MonoBehaviour
{
    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;

    // latest payload from server
    private Uri _wsUri;
    public string WebsiteEndpoint;
    public string LatestJson;

    async void Start()
    {
        _wsUri = new Uri(WebsiteEndpoint);

        _cts = new CancellationTokenSource();
        _ws = new ClientWebSocket();
        await ConnectAndListen();
    }

    async Task ConnectAndListen()
    {
        try
        {
            await _ws.ConnectAsync(_wsUri, _cts.Token);
            var buffer = new ArraySegment<byte>(new byte[8192]);

            while (_ws.State == WebSocketState.Open)
            {
                var sb = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await _ws.ReceiveAsync(buffer, _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", _cts.Token);
                        break;
                    }
                    sb.Append(Encoding.UTF8.GetString(buffer.Array, 0, result.Count));
                }
                while (!result.EndOfMessage);

                LatestJson = sb.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("WS error: " + e.Message + " — retrying soon");
            await Task.Delay(TimeSpan.FromSeconds(3));

            if (!_cts.IsCancellationRequested)
                await ConnectAndListen();
        }
    }

    void OnDestroy()
    {
        try { _cts.Cancel(); } catch { }
        try { _ws?.Dispose(); } catch { }
    }
}
