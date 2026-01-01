using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    /// <summary>
    /// NetworkMessage is the payload of a NetworkEnvelope. It contains the length, the command and the actual data depending on the command.
    /// </summary>
    public  class NetworkMessage
    {
        public static Dictionary<string, string> CommandToClassMapping = new Dictionary<string, string>
        {
            { "version",        "BitcoinLib.Network.VersionMessage" },
            { "verack",         "BitcoinLib.Network.VerAckMessage" },
            { "getheaders",     "BitcoinLib.Network.GetHeadersMessage" },
            { "headers",        "BitcoinLib.Network.HeadersMessage" },
            { "ping",           "BitcoinLib.Network.PingMessage" },
            { "pong",           "BitcoinLib.Network.PongMessage" },
            { "merkleblock",    "BitcoinLib.Network.MerkleBlockMessage" },
        };

        public string _command;
        public NetworkMessage(string command)
        {
            _command = command;
        }

        public virtual byte[] Serialize()
        {
            return new byte[0];
        }

        public static string GetClassNameForCommand(string command)
        {
            if (CommandToClassMapping.ContainsKey(command))
            {
                return CommandToClassMapping[command];
            }
            else
            {
                throw new Exception("Unknown command: " + command);
            }
        }
    }
}
