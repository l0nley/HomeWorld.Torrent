using HomeWorld.Torrent.BEncode;
using System;
using System.Linq;
using System.Text;

namespace HomeWorld.Torrent
{
    public class TorrentParser
    {
        private const string MalformedTorrent = "Mailformed torrent";

        public Encoding DefaultStringEncoding { get; }

        public TorrentParser() : this(Encoding.UTF8)
        {
        }

        public TorrentParser(Encoding defaultStringEncoding)
        {
            DefaultStringEncoding = defaultStringEncoding ?? throw new ArgumentNullException(nameof(defaultStringEncoding));
        }

        public Torrent Parse(ReadOnlySpan<byte> bytes, int offset)
        {
            var reader = new BEncodeReader();
            var dictionary = (BDictionary)reader.ReadElement(bytes, ref offset);
            var torrent = new Torrent();
            var stringEncoding = Encoding.UTF8;
            if (dictionary.TryGetValue(Constants.EncodingKey, out IBEncodedObject enc))
            {
                stringEncoding = Encoding.GetEncoding((BString)enc);
                torrent.Encoding = stringEncoding;
            }
            else
            {
                stringEncoding = DefaultStringEncoding;
            }

            foreach (var (key, value) in dictionary)
            {
                if (key == Constants.AnnounceKey)
                {
                    var announce = (BString)value;
                    if (string.IsNullOrEmpty(announce))
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    torrent.Announce = new Uri((BString)value);
                }
                else if (key == Constants.InfoKey)
                {
                    var dic = (BDictionary)value;
                    torrent.Info = ParseInfoSection(dic);
                }
                else if (key == Constants.EncodingKey)
                {
                    // do nothing;
                }
                else
                {
                    torrent.Extensions.Add(key, value);
                }
            }

            if (torrent.Announce == null || torrent.Info == null)
            {
                throw new ParsingException(MalformedTorrent);
            }

            return torrent;
        }

        private TorrentInfo ParseInfoSection(BDictionary dic)
        {
            var info = new TorrentInfo();
            bool? singleFile = null;
            foreach (var (key, value) in dic)
            {
                if (key == Constants.InfoNameKey)
                {
                    info.Name = (BString)value;
                }
                else if (key == Constants.InfoLengthKey)
                {
                    if (singleFile == false)
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    singleFile = true;
                    info.Files.Add((null, (BNumber)value));
                }
                else if (key == Constants.InfoPieceLengthKey)
                {
                    info.PieceLength = (BNumber)value;
                }
                else if (key == Constants.InfoPiecesKey)
                {
                    info.Pieces = (BString)value;
                }
                else if (key == Constants.InfoFilesKey)
                {
                    if (singleFile == true)
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    singleFile = false;
                    info.Files.AddRange(((BList)value).Select(_ =>
                    {
                        var dict = (BDictionary)_;
                        return ((BList)dict[Constants.InfoFilePathKey], (BNumber)dict[Constants.InfoFileLengthKey]);
                    }));
                }
                else
                {
                    info.Extensions.Add(key, value);
                }
            }

            if(info.Files.Count <= 0  || info.PieceLength <=0 || info.Pieces == null)
            {
                throw new ParsingException(MalformedTorrent);
            }

            return info;
        }
    }
}
