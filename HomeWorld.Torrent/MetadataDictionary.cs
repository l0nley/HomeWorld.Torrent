using BencodeNET.Objects;
using System;
using System.Collections.Concurrent;

namespace HomeWorld.Torrent
{
    public class MetadataDictionary
    {
        private const string InvalidMetadataContent = "Metadata contains invalid entries";

        public Uri Announce { get; private set; }
        public MetadataInfoSection Info { get; private set; }
        public ConcurrentDictionary<string, IBObject> Extensions { get; }

        private MetadataDictionary()
        {
            Extensions = new ConcurrentDictionary<string, IBObject>();
        }

        public static MetadataDictionary Create(Uri announce, MetadataInfoSection info)
        {
            return new MetadataDictionary
            {
                Announce = announce ?? throw new ArgumentNullException(nameof(announce)),
                Info = info ?? throw new ArgumentNullException(nameof(info))
            };
        }

        public static MetadataDictionary FromBDictionary(BDictionary dictionary)
        {
            try
            {
                var entry = new MetadataDictionary();
                foreach (var (key, value) in dictionary)
                {
                    var skey = key.ToString();
                    switch (skey)
                    {
                        case "announce":
                            entry.Announce = new Uri(value.ToString());
                            break;
                        case "info":
                            entry.Info = MetadataInfoSection.FromBDictionary((BDictionary)value);
                            break;
                        default:
                            entry.Extensions.AddOrUpdate(skey, value, (a, b) => throw new DataMisalignedException(InvalidMetadataContent));
                            break;

                    }
                }
                if (entry.Info == null || entry.Announce == null)
                {
                    throw new DataMisalignedException(InvalidMetadataContent);
                }

                return entry;
            }
            catch (Exception e)
            {
                throw new DataMisalignedException(InvalidMetadataContent, e);
            }
        }
    }
}
