using System;
using System.IO;
using System.Text;
using Xunit;

namespace HomeWorld.Torrent.Test
{
    public class MetadataParsingTests
    {
        private const string FileName = "invincible.torrent";
        [Fact]
        public void RealTorrentParsed()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
        }

        /*
        [Fact]
        public void ReturnsComment()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var result = meta.TryGetComment(out string comment);
                Assert.True(result);
                Assert.NotNull(comment);
            }
        }

        [Fact]
        public void ReturnsEncoding()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                Assert.True(meta.TryGetEncoding(out Encoding enc));
                Assert.NotNull(enc);
            }
        }

        [Fact]
        public void ReturnsPublisherUri()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                Assert.True(meta.TryGetPublisherUrl(out Uri uri));
                Assert.NotNull(uri);
            }
        }

        [Fact]
        public void ReturnsPublisher()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                Assert.True(meta.TryGetPublisher(out string pub));
                Assert.NotNull(pub);
            }
        }

        [Fact]
        public void ReturnsCreationDate()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                Assert.True(meta.TryGetCreationDate(out DateTime dt));
                Assert.True(dt != default(DateTime));
            }
        }

        [Fact]
        public void ReturnsCreator()
        {
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                Assert.True(meta.TryGetCreatedBy(out string creator));
                Assert.NotNull(creator);
            }
        }

        [Fact]
        public void SetsCreatedBy()
        {
            var toSet = Guid.NewGuid().ToString();
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetCreatedBy(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetCreatedBy(out string creator));
                Assert.Equal(toSet, creator);
            }
        }

        [Fact]
        public void SetsCreationDate()
        {
            var toSet = DateTime.UtcNow;
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetCreationDate(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetCreationDate(out DateTime actual));
                // precision is seconds;
                Assert.Equal((long)toSet.Subtract(DateTime.MinValue).TotalSeconds, (long)actual.Subtract(DateTime.MinValue).TotalSeconds);
            }
        }

        [Fact]
        public void SetsPublisher()
        {
            var toSet = Guid.NewGuid().ToString();
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetPublisher(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetPublisher(out string creator));
                Assert.Equal(toSet, creator);
            }
        }

        [Fact]
        public void SetsPublisherUri()
        {
            var toSet = new Uri($"https://microsoft.com/{Guid.NewGuid().ToString()}");
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetPublisherUri(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetPublisherUrl(out Uri puri));
                Assert.Equal(toSet, puri);
            }
        }

        [Fact]
        public void SetsEncoding()
        {
            var toSet = Encoding.UTF32;
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetEncoding(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetEncoding(out Encoding enc));
                Assert.Equal(toSet, enc);
            }
        }

        [Fact]
        public void SetsComment()
        {
            var toSet = Guid.NewGuid().ToString();
            using (var file = GetFile())
            {
                var meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
                var flag = false;
                meta.SetComment(toSet, _ =>
                {
                    flag = true;
                    return toSet;
                });
                Assert.True(flag);
                Assert.True(meta.TryGetComment(out string creator));
                Assert.Equal(toSet, creator);
            }
        }

        [Fact]
        public void AbleToModifyAndWrite()
        {
            const string outputFileName = "file.torrent";
            MetadataDictionary meta = null;
            using(var file = GetFile())
            {
                meta = MetadataDictionary.FromBDictionary(file.AsBDictionary());
            }
            meta.Info.Name = "Something";
            using (var file = File.Create(outputFileName))
            {
                file.WriteBDictionary(meta.ToBDictionary());
            }
            using (var file = File.OpenRead(outputFileName))
            {
                var meta2 = MetadataDictionary.FromBDictionary(file.AsBDictionary());
            }
        }


        */

        private byte[] GetTorrentContents()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
