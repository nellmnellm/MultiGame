using Game.Core.Network;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor.Sprites;

namespace Game.Client.Network
{
    /// <summary>
    /// Client °¡ Server ¿¬°áÀ» °ü¸®ÇÒ¶§ »ç¿ë
    /// </summary>
    class TcpServerSession : TcpSession
    {
        public event Action<IPacket> OnPacketReceived;

        public TcpServerSession(Socket socket, int bufferSize = 16384) : base(socket, bufferSize)
        {
        }

        public static async Task<TcpServerSession> ConnectAsync(string host, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(host, port);

            TcpServerSession session = new TcpServerSession(socket);
            session.Start();
            return session;
        }

        protected override void OnPacket(byte[] body)
        {
            IPacket packet = PacketFactory.FromBytes(body);
            OnPacketReceived?.Invoke(packet);
        }

        public void Send(IPacket packet)
        {
            Send(PacketFactory.ToBytes(packet));
        }
    }
}