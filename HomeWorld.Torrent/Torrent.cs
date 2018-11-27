using HomeWorld.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWorld.Torrent
{
    public class Torrent
    {
        public Dictionary<BString, IBEncodedObject> Extensions { get; }
        public Uri Announce { get; set; }
        public TorrentInfo Info { get; set; }
        public Encoding Encoding { get; set; }
        public Torrent()
        {
            Extensions = new Dictionary<BString, IBEncodedObject>();
        }

    }
}
