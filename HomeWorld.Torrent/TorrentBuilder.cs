using HomeWorld.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HomeWorld.Torrent
{
    public class TorrentBuilder
    {
        private readonly Encoding _encoding;
        private readonly Dictionary<BString, IBEncodedObject> _main;
        private readonly Dictionary<BString, IBEncodedObject> _info;
        private readonly Dictionary<string, long> _files;

        private long _pieceLength;
        private string _name;
        private Uri _announce;
        private byte[] _pieces;

        public TorrentBuilder(Encoding stringEncoding)
        {
            _encoding = stringEncoding ?? throw new ArgumentNullException(nameof(stringEncoding));
            _main = new Dictionary<BString, IBEncodedObject>();
            _info = new Dictionary<BString, IBEncodedObject>();
            _files = new Dictionary<string, long>();
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetAnnounce(Uri announce)
        {
            _announce = announce ?? throw new ArgumentNullException(nameof(announce));
        }

        public void SetExtension(BString key, IBEncodedObject value)
        {
            if (_main.ContainsKey(key))
            {
                _main[key] = value;
            }
            else
            {
                _main.Add(key, value);
                _main.Add(key, value);
            }
        }

        public void SetInfoExtension(BString key, IBEncodedObject value)
        {
            if (_info.ContainsKey(key))
            {
                _info[key] = value;
            }
            else
            {
                _info.Add(key, value);
            }
        }

        public void SetPieceLength(uint length)
        {
            _pieceLength = length;
        }

        public void AddFile(string relativePath, long length)
        {
            _files.Add(relativePath, length);
        }

        public void CalculatePieces(IFileStreamProvider provider)
        {
            throw new NotImplementedException();
        }

        public static Torrent Build(TorrentBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            var pieces = new BString
            {
                AsciiBytes = builder._pieces
            };
            var info = new TorrentInfo
            {
                Name = new BString(builder._name, Encoding.Default),
                PieceLength = new BNumber(builder._pieceLength),
                Pieces = pieces
            };
            foreach (var (key, value) in builder._info)
            {
                info.Extensions.Add(key, value);
            }
            foreach (var (key, value) in builder._files)
            {
                var pathlist = new BList
                {
                    Objects = new List<IBEncodedObject>(builder._files.Count)
                };
                foreach (var part in key.Split('/'))
                {
                    var s = new BString(part, Encoding.Default);
                    pathlist.Objects.Add(s);
                }
                info.Files.Add((pathlist, new BNumber(value)));
            }

            var torrent = new Torrent
            {
                Encoding = builder._encoding,
                Announce = builder._announce,
                Info = info
            };

            foreach (var (key, value) in builder._main)
            {
                torrent.Extensions.Add(key, value);
            }

            return torrent;
        }

        public static void Save(Stream stream, Torrent torrent)
        {
            var dic = GetDictionary(torrent);
            var writer = new BEncodeWriter();
            writer.WriteElement(stream, dic);
        }

        private static BDictionary GetDictionary(Torrent torrent)
        {
            var dic = new BDictionary
            {
                Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
            };
            dic.Dictionary.Add(Constants.AnnounceKey, new BString(torrent.Announce.ToString(), Encoding.Default));
            dic.Dictionary.Add(Constants.EncodingKey, new BString(torrent.Encoding.WebName, Encoding.Default));
            foreach (var (key, value) in torrent.Extensions)
            {
                dic.Dictionary.Add(key, value);
            }
            var infoDic = new BDictionary
            {
                Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
            };
            foreach (var (key, value) in torrent.Info.Extensions)
            {
                infoDic.Dictionary.Add(key, value);
            }
            if (torrent.Info.Name != null)
            {
                infoDic.Dictionary.Add(Constants.InfoNameKey, torrent.Info.Name);
            }
            infoDic.Dictionary.Add(Constants.InfoPieceLengthKey, torrent.Info.PieceLength);
            infoDic.Dictionary.Add(Constants.InfoPiecesKey, torrent.Info.Pieces);
            if (torrent.Info.Files.Count == 1)
            {
                var (path, len) = torrent.Info.Files[0];
                infoDic.Dictionary.Add(Constants.InfoLengthKey, len);
            }
            else
            {
                var lst = new BList
                {
                    Objects = new List<IBEncodedObject>(torrent.Info.Files.Count)
                };
                foreach (var (key, value) in torrent.Info.Files)
                {
                    var fdic = new BDictionary
                    {
                        Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
                        {
                            { Constants.InfoFilePathKey, key },
                            { Constants.InfoFileLengthKey, value }
                        }
                    };
                    lst.Objects.Add(fdic);
                }
                infoDic.Dictionary.Add(Constants.InfoFilesKey, lst);
            }

            dic.Dictionary.Add(Constants.InfoKey, infoDic);

            return dic;
        }

        public static TorrentBuilder Load(Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            var enc = torrent.Encoding ?? Encoding.UTF8;
            var builder = new TorrentBuilder(enc)
            {
                _name = GetString(torrent.Info.Name, enc),
                _announce = torrent.Announce,
                _pieceLength = torrent.Info.PieceLength,
                _pieces = new byte[torrent.Info.Pieces.AsciiBytes.Length]
            };
            Array.Copy(torrent.Info.Pieces.AsciiBytes, builder._pieces, torrent.Info.Pieces.AsciiBytes.Length);
            foreach (var (key, value) in torrent.Extensions)
            {
                builder._main.Add(key, value);
            }
            foreach (var (key, value) in torrent.Info.Extensions)
            {
                builder._info.Add(key, value);
            }
            foreach (var (pth, len) in torrent.Info.Files)
            {
                var path = string.Join(Path.PathSeparator, pth.Select(_ => GetString((BString)_, torrent.Encoding)));
                builder._files.Add(path, len);
            }

            return builder;
        }

        private static string GetString(BString value, Encoding encoding)
        {
            return Encoding.Default.GetString(
                Encoding.Convert(encoding, Encoding.Default, encoding.GetBytes(value.ToString(encoding))));
        }
    }
}
