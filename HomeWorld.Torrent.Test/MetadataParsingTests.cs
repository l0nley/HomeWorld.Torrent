using HomeWorld.Torrent.BEncode;
using System;
using System.IO;
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

        
        [Fact]
        public void ReturnsComment()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            Assert.True(trt.TryGetExtension(ExtensionKeys.Comment, out BString value));
            Assert.NotNull(value);
        }

        
        [Fact]
        public void ReturnsPublisherUri()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            Assert.True(trt.TryGetExtension(ExtensionKeys.PublisherUrl, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsPublisher()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            Assert.True(trt.TryGetExtension(ExtensionKeys.Publisher, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsCreationDate()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            Assert.True(trt.TryGetExtension(ExtensionKeys.CreationDate, out BNumber value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsCreator()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            Assert.True(trt.TryGetExtension(ExtensionKeys.CreatedBy, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void SavesFile()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentParser();
            var trt = parser.Parse(new ReadOnlySpan<byte>(contens), 0);
            var builder = TorrentBuilder.Load(trt);
            builder.SetName("русский");
            var modified = TorrentBuilder.Build(builder);
            using(var file = File.Create("output.torrent"))
            {
                TorrentBuilder.Save(file, modified);
            }
        }

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
