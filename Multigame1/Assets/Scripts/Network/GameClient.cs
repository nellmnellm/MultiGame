using Game.Client.Network;
using System.Net.Sockets;
using Game.Core.Network;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Client
{
    public class GameClient : MonoBehaviour
    {
        [SerializeField] ConnectionSettings _connectionSettings;

        void Start()
        {
            RpcRegistry.Collect(Assembly.GetExecutingAssembly());

            Task connection = Task.Run(async () =>
            {
                Debug.Log($"[{nameof(GameClient)}] : 시작. 서버 접속 시도...");
                TcpServerSession serverSession = await TcpServerSession.ConnectAsync(_connectionSettings.serverIp, _connectionSettings.serverPort);
                serverSession.OnPacketReceived += packet => Debug.Log($"[{nameof(GameClient)}] : Packet {packet.PacketId} 수신");
                serverSession.OnPacketReceived += RpcHandler.OnRpcPacketReceived;
                Debug.Log($"[{nameof(GameClient)}] : 서버 접속 완료");

                //UI_Login uiLogin = new UI_Login(serverSession);
                //bool loginResult = await uiLogin.LoginAsync("luke", "1234");
                //
                //if (loginResult == false)
                //{
                //    serverSession.Dispose();
                //    return;
                //}
                //
                //UI_Chat uiChat = new UI_Chat(serverSession);
                //await uiChat.SendMessageLoopAsync();
            });
        }
    }
}