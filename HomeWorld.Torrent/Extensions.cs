using BencodeNET.Objects;
using BencodeNET.Parsing;
using System;
using System.IO;

namespace HomeWorld.Torrent
{
    public static class Extensions
    {
        public static BDictionary AsBDictionary(this Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            var parser = new BencodeParser();
            return parser.Parse<BDictionary>(stream);
        }
    }
}
