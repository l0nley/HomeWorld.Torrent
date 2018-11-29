using System;
using System.IO;
using System.Text;

namespace HomeWorld.Torrent.BEncode
{
    public class BEncodeWriter
    {
        public void WriteElement(Stream stream, IBEncodedObject elem)
        {
            switch(elem.Type)
            {
                case BEncodeType.Dictionary:
                    WriteDictionary(stream, (BDictionary)elem);
                    break;
                case BEncodeType.List:
                    WriteList(stream, (BList)elem);
                    break;
                case BEncodeType.Number:
                    WriteNumber(stream, (BNumber)elem);
                    break;
                case BEncodeType.String:
                    WriteString(stream, (BString)elem);
                    break;
                default:
                    throw new NotSupportedException($"Element type {elem.Type.ToString()} is not supported");
                 
            }
        }

        public void WriteString(Stream stream, BString str)
        {
            var slen = str.Bytes.Length.ToString();
            var len = Encoding.ASCII.GetBytes(slen);
            stream.Write(len);
            stream.WriteByte((byte)':');
            stream.Write(str.Bytes);
        }

        public void WriteNumber(Stream stream, BNumber number)
        {
            var bytes = Encoding.ASCII.GetBytes(number.AsciiValue);
            stream.WriteByte((byte)'i');
            stream.Write(bytes);
            stream.WriteByte((byte)'e');
        }

        public void WriteList(Stream stream, BList list)
        {
            stream.WriteByte((byte)'l');
            foreach(var element in list.Objects)
            {
                WriteElement(stream, element);
            }
            stream.WriteByte((byte)'e');
        }

        public void WriteDictionary(Stream stream, BDictionary dic)
        {
            stream.WriteByte((byte)'d');
            foreach(var (key, value) in dic)
            {
                WriteString(stream, key);
                WriteElement(stream, value);
            }
            stream.WriteByte((byte)'e');
        }
    }
}
