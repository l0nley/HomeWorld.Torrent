using BencodeNET.Objects;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace HomeWorld.Torrent
{
    public class MetadataInfoSection
    {
        private const string InvalidInfoSectionContent = "Metadata information section contains invalid entries";
        private const string PieceLengthInvalid = "pieceLength cannot be less or equal zero";
        private const string FileSizeInvalid = "fileSize of item is invalid";
        private const string InvalidBufferLength = "Invalid SHA1 buffer size";
        private const int SHA1Size = 20;

        private List<(string, int)> _files;
        private byte[] _sha;
        public ConcurrentDictionary<string, IBObject> Extensions { get; }

        public string Name { get; private set; }
        public int PieceLength { get; private set; }

        public int PieceCount { get; private set; }

        private MetadataInfoSection()
        {
            Extensions = new ConcurrentDictionary<string, IBObject>();
        }

        public int FilesCount => _files.Count;

        public (string, int) FileAt(int index) => _files[index];

        public ReadOnlySpan<byte> ShaAt(int index) => new ReadOnlySpan<byte>(_sha, SHA1Size * index, SHA1Size);
        public void SetSha(int index, ReadOnlySpan<byte> bytes)
        {
            if(bytes.Length != SHA1Size)
            {
                throw new ArgumentException(InvalidBufferLength, nameof(bytes));
            }
            var span = new Span<byte>(_sha, index * SHA1Size, SHA1Size);
            bytes.CopyTo(span);
        }

        ~MetadataInfoSection()
        {
            if (_sha != null)
            {
                ArrayPool<byte>.Shared.Return(_sha);
            }
        }

        public static MetadataInfoSection FromBDictionary(BDictionary dictionary)
        {
            try
            {
                dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                var entry = new MetadataInfoSection();
                bool? singleFile = null;
                var name = new Lazy<string>(() =>
                {
                    if (dictionary.TryGetValue("name", out IBObject value))
                    {
                        return value?.ToString();
                    }

                    return null;
                });
                foreach (var (key, value) in dictionary)
                {
                    var skey = key.ToString();
                    switch (skey)
                    {
                        case "name":
                            entry.Name = name.Value;
                            break;
                        case "piece length":
                            entry.PieceLength = (BNumber)value;
                            break;
                        case "length":
                            if (singleFile == false)
                            {
                                throw new DataMisalignedException(InvalidInfoSectionContent);
                            }
                            singleFile = true;
                            entry._files = new List<(string, int)>(1)
                            {
                                (name.Value, (BNumber)value)
                            };
                            break;
                        case "pieces":
                            var bs = (BString)value;
                            if (bs.Value.Count % SHA1Size > 0)
                            {
                                throw new DataMisalignedException(InvalidInfoSectionContent);
                            }
                            entry._sha = ArrayPool<byte>.Shared.Rent(bs.Value.Count);
                            entry.PieceCount = bs.Value.Count / 20;
                            var idx = 0;
                            foreach (var item in bs.Value)
                            {
                                entry._sha[idx] = item;
                                idx++;
                            }
                            break;
                        case "files":
                            if (singleFile == true)
                            {
                                throw new DataMisalignedException(InvalidInfoSectionContent);
                            }
                            singleFile = false;
                            BList lst = (BList)value;
                            entry._files = new List<(string, int)>(lst.Count);
                            foreach (BDictionary item in lst)
                            {
                                var path = string.Join(Path.PathSeparator, ((BList)item["path"]).AsStrings());
                                entry._files.Add((path, (BNumber)item["length"]));
                            }
                            break;
                        default:
                            entry.Extensions.AddOrUpdate(skey, value, (a, b) => throw new DataMisalignedException(InvalidInfoSectionContent));
                            break;
                    }
                }
                if (entry._files == null ||
                    entry._sha.Length == 0 ||
                    entry.PieceLength == 0)
                {
                    throw new DataMisalignedException(InvalidInfoSectionContent);
                }
                return entry;
            }
            catch (Exception e)
            {
                throw new DataMisalignedException(InvalidInfoSectionContent, e);
            }
        }

        public static MetadataInfoSection Create(string name, int pieceLength, IReadOnlyCollection<(string, int)> files)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (pieceLength <= 0)
            {
                throw new ArgumentException(PieceLengthInvalid, nameof(pieceLength));
            }

            var entry = new MetadataInfoSection
            {
                PieceLength = pieceLength,
                Name = name,
                _files = new List<(string, int)>(files.Count)
            };

            var totalSize = 0;
            foreach (var item in files)
            {
                if (item.Item2 < 0)
                {
                    throw new ArgumentException(FileSizeInvalid, nameof(files));
                }
                entry._files.Add(item);
                totalSize += item.Item2;
            }

            var pieceCount = (int)Math.Ceiling(totalSize / (double)pieceLength);
            entry._sha = ArrayPool<byte>.Shared.Rent(pieceCount * SHA1Size);
            return entry;
        }
    }
}
