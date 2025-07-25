using Game.Core.Network;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using UnityEditor.Sprites;
using UnityEngine;

namespace Game.Client.Network
{
    public static class RpcHandler
    {
        /// <summary>
        /// Server 가 Rpc 요청하면 해당 ClientRpc 찾아서 호출
        /// </summary>
        /// <param name="id"> ClientRpcId </param>
        /// <param name="jsonData"> parameters </param>
        /// <returns></returns>
        public static bool TryInvoke(uint id, string jsonData)
        {
            if (RpcRegistry.TryGetClientRpc(id, out MethodInfo methodInfo) == false)
                return false;

            var parameters = methodInfo.GetParameters();

            try
            {
                object?[] args = parameters.Length switch
                {
                    0 => Array.Empty<object>(),
                    1 => new[] { JsonConvert.DeserializeObject(jsonData, parameters[0].ParameterType) },
                    _ => throw new NotSupportedException($"Rpc with {parameters.Length} paramters is not supported yet")
                };

                if (methodInfo.IsStatic)
                {
                    methodInfo.Invoke(null, args);
                }
                else
                {
                    // TODO : Network instance 관리 시스템이 별도로 필요함.
                    throw new NotImplementedException("instance method 용 Rpc 는 아직 구현되지 않았습니다.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                throw new Exception(ex.Message);
            }
        }

        public static void OnRpcPacketReceived(IPacket packet)
        {
            switch (packet.PacketId)
            {
                case PacketId.ClientRpcRequest:
                    var p = ((ClientRpcRequest)packet);

                    if (TryInvoke(p.RpcId, p.JsonData) == false)
                        throw new InvalidDataException($"{p.RpcId} id 는 유효하지 않은 Rpc id 입니다");
                    break;
                case PacketId.ClientRpcReponse:
                    // 얘는 서버용
                    break;
                case PacketId.ServerRpcRequest:
                    // 얘는 서버용
                    break;
                case PacketId.ServerRpcReponse:
                    // 얘는 서버에서응답 받은거니까 구현해야함
                    break;
                default:
                    throw new ArgumentException($"{packet.PacketId} 패킷은 Rpc 용 패킷이 아닙니다.");
            }
        }
    }
}