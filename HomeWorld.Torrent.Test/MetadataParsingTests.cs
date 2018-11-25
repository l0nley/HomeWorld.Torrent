using System.IO;
using Xunit;

namespace HomeWorld.Torrent.Test
{
    public class MetadataParsingTests
    {
        [Fact]
        public void RealTorrentParsed()
        {
            using (var file = File.OpenRead("invincible.torrent"))
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
            }
        }
    }
}
