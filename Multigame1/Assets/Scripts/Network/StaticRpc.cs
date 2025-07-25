using Game.Core.Network;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Client.Network
{
    public static class StaticRpc
    {
        /// <summary>
        /// Server 가 TcpClientSession.Send 할때, 
        /// ClientRpcRequest 패킷에
        /// id = 1, json 포맷으로 바꾼 int count = 5 를 담아서 보내주면
        /// Client 가 받아서 이함수 count 인자에 5 넣고 호출
        /// </summary>
        /// <param name="count"></param>
        [RpcImplementation(1, RpcImplementationTarget.Client)]
        public static void PrintCountdown(int count)
        {
            Task.Run(async () =>
            {
                for (int i = count; i > 0; i--)
                {
                    Debug.Log($"Countdown : {i}");
                    await Task.Delay(1000);
                }

                Debug.Log($"Countdown : Finished");
            });
        }
    }
}