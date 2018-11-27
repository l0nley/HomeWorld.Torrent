using HomeWorld.Torrent.BEncode;
using System.Text;

namespace HomeWorld.Torrent
{
    public static class ExtensionKeys
    {
        public static readonly BString Comment = new BString("comment", Encoding.ASCII);
        public static readonly BString AnnounceList = new BString("announce-list", Encoding.ASCII);
        public static readonly BString CreatedBy = new BString("created by", Encoding.ASCII);
        public static readonly BString CreationDate = new BString("creation date", Encoding.ASCII);
        public static readonly BString Publisher = new BString("publisher", Encoding.ASCII);
        public static readonly BString PublisherUrl = new BString("publisher-url", Encoding.ASCII);
    }
}
