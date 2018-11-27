using HomeWorld.Torrent.BEncode;
using System.Text;

namespace HomeWorld.Torrent
{
    internal static class Constants
    {
        public static readonly BString InfoNameKey = new BString("name", Encoding.ASCII);
        public static readonly BString InfoPieceLengthKey = new BString("piece length", Encoding.ASCII);
        public static readonly BString InfoLengthKey = new BString("length", Encoding.ASCII);
        public static readonly BString InfoFilePathKey = new BString("path", Encoding.ASCII);
        public static readonly BString InfoFileLengthKey = InfoLengthKey;
        public static readonly BString InfoFilesKey = new BString("files", Encoding.ASCII);
        public static readonly BString InfoPiecesKey = new BString("pieces", Encoding.ASCII);
        public static readonly BString AnnounceKey = new BString("announce", Encoding.ASCII);
        public static readonly BString InfoKey = new BString("info", Encoding.ASCII);
        public static readonly BString EncodingKey = new BString("encoding", Encoding.ASCII);
    }
}
