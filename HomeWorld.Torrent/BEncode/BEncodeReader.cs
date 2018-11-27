using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWorld.Torrent.BEncode
{
    public class BEncodeReader
    {
        public IBEncodedObject ReadElement(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            try
            {
                if (cursor >= bytes.Length)
                {
                    return null;
                }

                var key = bytes[cursor];
                switch (key)
                {
                    case (byte)'i':
                        return ReadNumber(bytes, ref cursor);
                    case (byte)'l':
                        return ReadList(bytes, ref cursor);
                    case (byte)'d':
                        return ReadDictionary(bytes, ref cursor);
                    default:
                        return ReadString(bytes, ref cursor);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException($"Error during parsing encoded value at {cursor}", e);
            }
        }

        private BDictionary ReadDictionary(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            cursor++;
            var dict = new Dictionary<BString, IBEncodedObject>();
            while (bytes[cursor] != (byte)'e')
            {
                var key = ReadString(bytes, ref cursor);
                var value = ReadElement(bytes, ref cursor);
                dict.Add(key, value);
            }
            cursor++;
            return new BDictionary
            {
                Dictionary = dict
            };
        }

        private BList ReadList(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = ++cursor;
            var lst = new List<IBEncodedObject>();
            while (bytes[cursor] != (byte)'e')
            {
                lst.Add(ReadElement(bytes, ref cursor));
            }
            cursor++;
            return new BList
            {
                Objects = lst
            };
        }

        private BString ReadString(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = cursor;
            // reading until size end. no bound check
            while (bytes[cursor] != ':')
            {
                cursor++;
            }
            var selected = bytes.Slice(start, cursor - start);
            var stringLength = int.Parse(Encoding.ASCII.GetString(selected));
            cursor++;
            var stringBytes = bytes.Slice(cursor, stringLength);
            cursor += stringLength;
            return new BString
            {
                AsciiBytes = stringBytes.ToArray()
            };
        }

        private BNumber ReadNumber(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = ++cursor;
            // reading until stop. no bound check
            while (bytes[cursor] != 'e')
            {
                cursor++;
            }
            var selected = bytes.Slice(start, cursor - start);
            cursor++;
            return new BNumber
            {
                AsciiValue = Encoding.ASCII.GetString(selected)
            };
        }
    }
}
