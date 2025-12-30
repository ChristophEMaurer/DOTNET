using BitcoinLib.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class NetworkEnvelopeTest : UnitTest
    {
        public static void test_chapter_10_ex_2()
        {
            byte[] rawData = Tools.HexStringToBytes("f9beb4d9 76657261636b000000000000 00000000 5df6e0e2".Replace(" ", ""));
            var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(rawData));
            NetworkEnvelope envelope = NetworkEnvelope.Parse(reader, false);

            AssertEqual("verack", envelope._command);
            AssertEqual(new byte[] { }, envelope._payload);
        }

        public static void test_parse()
        {
            
            byte[] rawData = Tools.HexStringToBytes("f9beb4d9 76657261636b000000000000 00000000 5df6e0e2".Replace(" ", ""));
            BinaryReader reader = new System.IO.BinaryReader(new System.IO.MemoryStream(rawData));
            NetworkEnvelope envelope = NetworkEnvelope.Parse(reader, false);
            AssertEqual("verack", envelope._command);
            AssertTrue(envelope._payload.Length == 0);

            rawData = Tools.HexStringToBytes("f9beb4d9 76657273696f6e0000000000 65000000 5f1a69d2 721101000100000000000000bc8f5e5400000000010000000000000000000000000000000000ffffc61b6409208d010000000000000000000000000000000000ffffcb0071c0208d128035cbc97953f80f2f5361746f7368693a302e392e332fcf05050001".Replace(" ", ""));
            reader = new System.IO.BinaryReader(new System.IO.MemoryStream(rawData));
            envelope = NetworkEnvelope.Parse(reader, false);
            AssertEqual("version", envelope._command);
            byte[] bPayload = rawData[24..];
            AssertEqual(envelope._payload, bPayload);
        }

        public static void test_serialize()
        {
            string[] data =
            {
                "f9beb4d9 76657261636b000000000000 00000000 5df6e0e2",
                "f9beb4d9 76657273696f6e0000000000 65000000 5f1a69d2 721101000100000000000000bc8f5e5400000000010000000000000000000000000000000000ffffc61b6409208d010000000000000000000000000000000000ffffcb0071c0208d128035cbc97953f80f2f5361746f7368693a302e392e332fcf05050001"
            };
            foreach (string item in data)
            {
                byte[] rawData = Tools.HexStringToBytes(item.Replace(" ", ""));
                var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(rawData));
                NetworkEnvelope envelope = NetworkEnvelope.Parse(reader, false);

                byte[] bSerialized = envelope.serialize();
                AssertEqual(bSerialized, rawData);
            }
        }
    }
}
