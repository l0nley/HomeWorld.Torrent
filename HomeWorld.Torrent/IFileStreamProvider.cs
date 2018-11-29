using System.IO;

namespace HomeWorld.Torrent
{
    public interface IFileStreamProvider
    {
        Stream Resolve(string relativePath, out bool autoDispose);
    }
}
