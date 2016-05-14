﻿using System.Net.Sockets;
using W2Open.Common;
using W2Open.Common.Utility;

namespace W2Open.GameState
{
    public class CPlayerConnection
    {
        public ushort Index { get; set; }
        public EState State { get; set; }

        public TcpClient Tcp { get; private set; }
        public CRecvPacket RecvPacket { get; private set; }

        public CGameStateController GameState;

        public CPlayerConnection(CGameStateController gameState, TcpClient tcpClient)
        {
            Index = 0;
            Tcp = tcpClient;
            State = EState.WAITING_TO_LOGIN;
            RecvPacket = new CRecvPacket(NetworkBasics.MAXL_PACKET);
            GameState = gameState;
        }

        /// <summary>
        /// Represents the actual state of the player in the game.
        /// </summary>
        public enum EState
        {
            /// <summary>
            /// Setted when the system asked to shutdown the player.
            /// </summary>
            CLOSED = 0,
            /// <summary>
            /// Waiting to be inserted in the GameController. The player have just been created and don't sent the INIT_CODE packet yet.
            /// </summary>
            WAITING_TO_LOGIN,
            /// <summary>
            /// Setted when the login process have success. The player is in the character selecion step.
            /// </summary>
            SEL_CHAR,
            /// <summary>
            /// Setted when the player picks a character and enters the game world.
            /// </summary>
            AT_WORLD,
        }

        /// <summary>
        /// Send a given game packet to the player.
        /// </summary>
        public void SendPacket<T>(T packet) where T : struct
        {
            NetworkStream stream = Tcp.GetStream();

            GameState.Statistics.SendedPackets++;

            if (stream.CanWrite)
            {
                CRecvPacket buffer = new CRecvPacket(W2Marshal.GetBytes(packet));

                W2PacketSecurity.Encrypt(buffer);

                stream.Write(buffer.RawBuffer, 0, buffer.RawBuffer.Length);
            }
        }

        public bool HaveValidIndex()
        {
            return Index >= 1 && Index <= NetworkBasics.MAX_PLAYER;
        }
    }
}