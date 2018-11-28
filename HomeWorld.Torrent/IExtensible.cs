using HomeWorld.Torrent.BEncode;
using System.Collections.Generic;

namespace HomeWorld.Torrent
{
    public interface IExtensible
    {
        Dictionary<BString, IBEncodedObject> Extensions { get; }
    }
}
