using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.WebSocket
{
    static class EncodeDecode
    {
        internal static byte[] EncodeMessage(byte[] message)
        {
            byte[] bytesRaw = message;
            byte[] frame = new byte[10];

            int indexStartRawData;
            int length = bytesRaw.Length;

            frame[0] = (byte)130;
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            byte[] response = new byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        internal static byte[] DecodeMessage(byte[] bytes)
        {
            byte secondByte = bytes[1];
            int dataLength = secondByte & 127;
            int indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;

            IEnumerable<byte> keys = bytes.Skip(indexFirstMask).Take(4);
            int indexFirstDataByte = indexFirstMask + 4;

            byte[] decoded = new byte[bytes.Length - indexFirstDataByte];
            for (int i = indexFirstDataByte, j = 0; i < bytes.Length; i++, j++)
            {
                decoded[j] = (byte)(bytes[i] ^ keys.ElementAt(j % 4));
            }

            return decoded;
        }
    }
}