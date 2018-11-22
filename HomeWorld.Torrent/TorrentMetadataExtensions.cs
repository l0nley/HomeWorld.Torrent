using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace HomeWorld.Torrent
{
    public static class TorrentMetadataExtensions
    {
        public static TorrentMetadata ReadTorrentFile(this Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            try
            {
                var parser = new BencodeParser();
                var dict = parser.Parse<BDictionary>(stream);
                var torrent = new TorrentMetadata();
                foreach (var item in dict)
                {
                    var readableKey = item.Key.ToString();
                    if (Parsers.TryGetValue(readableKey, out Action<IBObject, TorrentMetadata> action))
                    {
                        action(item.Value, torrent);
                    }
                    else
                    {
                        torrent.Extensions.Add(readableKey, item.Value);
                    }
                }
                return torrent;
            }
            catch (Exception e)
            {
                throw new DataMisalignedException("Torrent was not recognized", e);
            }
        }


     
        public static bool ValidatePiece(this Stream piece, int index, TorrentMetadata meta)
        {
            meta = meta ?? throw new ArgumentNullException(nameof(meta));
            piece = piece ?? throw new ArgumentNullException(nameof(piece));
            if(index < 0 || index >= meta.SHAs.Count)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }
            
            using (var calc = SHA1.Create())
            {
                var chunked = new ReadonlyChunkStream(piece, meta.PieceLength);
                var hash = calc.ComputeHash(chunked);
                return hash.SequenceEqual(meta.SHAs[index]);
            }
        }

        public static Task<IEnumerable<int>> ValidateFileAsync(this Stream stream, TorrentMetadata meta)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            meta = meta ?? throw new ArgumentNullException(nameof(meta));
            return Task.Run(() =>
            {
                var currentIndex = 0;
                var lstIndexes = new List<int>();
                while(stream.Position != stream.Length && currentIndex < meta.SHAs.Count)
                {
                    if(stream.ValidatePiece(currentIndex, meta) ==  false)
                    {
                        lstIndexes.Add(currentIndex);
                    }
                    currentIndex++;
                }
                return (IEnumerable<int>)lstIndexes;
            });
        }

        private static Dictionary<string, Action<IBObject, TorrentMetadata>> Parsers = new Dictionary<string, Action<IBObject, TorrentMetadata>>
        {
            {"announce", (a,b) => b.Announce = new Uri(((BString)a).ToString())},
            {"announce-list", ExtractAnounceList},
            {"comment",(a,b) => b.Comment = ((BString)a).ToString()},
            {"created by", (a,b) => b.CreatedBy = ((BString)a).ToString()},
            {"publisher", (a,b) => b.PublisherName = ((BString)a).ToString()},
            {"publisher-url", (a,b) => b.PublisherUri = new Uri(((BString)a).ToString())},
            {"creation date", (a,b) => b.Created = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc).AddSeconds((BNumber)a) },
            {"info", ExtractInfo}
        };

        private static void ExtractInfo(IBObject obj, TorrentMetadata file)
        {
            var dict = (BDictionary)obj;
            bool? singleFile = null;
            foreach (var item in dict)
            {
                var key = item.Key.ToString();
                switch (key)
                {
                    case "length":
                        if (singleFile == false)
                        {
                            throw new DataMisalignedException("Torrent already have key \"files\" key");
                        }
                        singleFile = true;
                        var fname = string.Empty;
                        if (dict.TryGetValue("name", out IBObject nameVal))
                        {
                            fname = ((BString)nameVal).ToString();
                        }
                        file.Files.Add(fname, (BNumber)item.Value);
                        break;
                    case "name":
                        break;
                    case "files":
                        if (singleFile == true)
                        {
                            throw new DataMisalignedException("Torrent already have \"length\" key");
                        }
                        singleFile = false;
                        var dicLst = (BList)item.Value;
                        foreach (var dicListItem in dicLst)
                        {
                            var sdic = (BDictionary)dicListItem;
                            file.Files.Add(((BString)sdic["path"]).ToString(), (BNumber)sdic["length"]);
                        }
                        break;
                    case "piece length":
                        file.PieceLength = (BNumber)item.Value;
                        break;
                    case "pieces":
                        var shas = (BString)item.Value;
                        if (shas.Value.Count % 20 > 0)
                        {
                            throw new DataMisalignedException("SHA1 hash section is invalid");
                        }
                        var arr = shas.Value.ToArray();
                        for (var i = 0; i < shas.Value.Count / 20; i++)
                        {
                            var chunk = new byte[20];
                            Array.Copy(arr, i * 20, chunk, 0, 20);
                            file.SHAs.Add(chunk);
                        }
                        break;
                    default:
                        file.Extensions.Add($"info.{item.Key}", item.Value);
                        break;
                }
            }
            if (singleFile == null)
            {
                throw new DataMisalignedException("Information about torrent contents was not found");
            }
        }

        private static void ExtractAnounceList(IBObject obj, TorrentMetadata file)
        {
            var lst = (BList)obj;
            foreach (var item in lst)
            {
                var subList = (BList)item;
                foreach (var subItem in subList)
                {
                    file.AnnounceList.Add(new Uri(subItem.ToString()));
                }
            }
        }
    }
}
