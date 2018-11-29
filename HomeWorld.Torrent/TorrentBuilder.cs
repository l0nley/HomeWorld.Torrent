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

        public Torrent Build()
        {
            var encoding = _encoding ?? Encoding.UTF8;
            var pieces = new BString(_pieces);
            var info = new TorrentInfo
            {
                Name = new BString(_name, encoding),
                PieceLength = new BNumber(_pieceLength),
                Pieces = pieces
            };
            foreach (var (key, value) in _info)
            {
                info.Extensions.Add(key, value);
            }
            foreach (var (key, value) in _files)
            {
                var pathlist = new BList
                {
                    Objects = new List<IBEncodedObject>(_files.Count)
                };
                foreach (var part in key.Split('/'))
                {
                    var s = new BString(part, encoding);
                    pathlist.Objects.Add(s);
                }
                info.Files.Add((pathlist, new BNumber(value)));
            }

            var torrent = new Torrent
            {
                Encoding = _encoding,
                Announce = _announce,
                Info = info
            };

            foreach (var (key, value) in _main)
            {
                torrent.Extensions.Add(key, value);
            }

            return torrent;
        }

       

        public static TorrentBuilder FromExisting(Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            var enc = torrent.Encoding ?? Encoding.UTF8;
            var builder = new TorrentBuilder(enc)
            {
                _name = torrent.Info.Name.ToString(),
                _announce = torrent.Announce,
                _pieceLength = torrent.Info.PieceLength,
                _pieces = new byte[torrent.Info.Pieces.Bytes.Length]
            };
            Array.Copy(torrent.Info.Pieces.Bytes, builder._pieces, torrent.Info.Pieces.Bytes.Length);
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
                var path = string.Join(Path.PathSeparator, pth.Select(_ => ((BString)_).ToString()));
                builder._files.Add(path, len);
            }

            return builder;
        }
    }
}
