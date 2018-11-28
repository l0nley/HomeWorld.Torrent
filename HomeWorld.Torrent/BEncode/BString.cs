using System;
using System.Linq;
using System.Text;

namespace HomeWorld.Torrent.BEncode
{
    public class BString : IBEncodedObject
    {
        public BEncodeType Type => BEncodeType.String;

        internal byte[] AsciiBytes { get; set; }
        private int? _hashCode = null;

        internal BString()
        {
        }

        public BString(string str, Encoding incomingStringEncoding)
        {
            incomingStringEncoding = incomingStringEncoding ?? throw new ArgumentNullException(nameof(incomingStringEncoding));
            AsciiBytes = Encoding.Convert(incomingStringEncoding, Encoding.ASCII, incomingStringEncoding.GetBytes(str));
        }

        public static implicit operator string(BString str)
        {
            return str?.ToString();
        }

        public override string ToString()
        {
            return Encoding.Default.GetString(Encoding.Convert(Encoding.ASCII, Encoding.Default, AsciiBytes));
        }

        public string ToString(Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            return encoding.GetString(Encoding.Convert(Encoding.ASCII, encoding, AsciiBytes));
        }

        public override bool Equals(object obj)
        {
            return AsciiBytes.SequenceEqual(((BString)obj).AsciiBytes);
        }

        public override int GetHashCode()
        {
            if (_hashCode != null)
            {
                return _hashCode.Value;
            }

            unchecked
            {
                var prime = 16777619u;
                var hash = 2166136261;
                foreach (var byt in AsciiBytes)
                {
                    hash ^= byt;
                    hash *= prime;
                }
                _hashCode = (int)hash;
            }

            return _hashCode.Value;
        }

        public static bool operator ==(BString str1, BString str2)
        {
            if (ReferenceEquals(str1, str2))
            {
                return true;
            }
            var str1Null = ReferenceEquals(str1, null);
            var str2Null = ReferenceEquals(str2, null);
            if (str1Null && !str2Null)
            {
                return false;
            }
            if (!str1Null && str2Null)
            {
                return false;
            }

            if (str1.GetHashCode() == str2.GetHashCode())
            {
                return str1.Equals(str2);
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(BString str1, BString str2)
        {
            return !(str1 == str2);
        }
    }
}
