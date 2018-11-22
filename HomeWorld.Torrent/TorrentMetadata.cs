using BencodeNET.Objects;
using System;
using System.Collections.Generic;

namespace HomeWorld.Torrent
{
    public class TorrentMetadata
    {
        public Uri Announce { get; internal set; }
        public List<Uri> AnnounceList { get; } 
        public string CreatedBy { get; internal set; }
        public DateTime Created { get; internal set; }
        public string Comment { get; internal set; }
        public string PublisherName { get; internal set; }
        public Uri PublisherUri { get; internal set; }
        public Dictionary<string, IBObject> Extensions { get; }
        public Dictionary<string, int> Files { get; }
        public List<byte[]> SHAs { get; }
        public int PieceLength { get; internal set; }
        
        protected internal TorrentMetadata()
        {
            Extensions = new Dictionary<string, IBObject>();
            AnnounceList = new List<Uri>();
            Files = new Dictionary<string, int>();
            SHAs = new List<byte[]>();
        }
    }
}
