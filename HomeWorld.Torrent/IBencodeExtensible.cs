using HomeWorld.Torrent.BEncode;
using System.Collections.Generic;

namespace HomeWorld.Torrent
{
    public interface IBencodeExtensible
    {
        Dictionary<BString, IBEncodedObject> Extensions { get; }
    }
}
