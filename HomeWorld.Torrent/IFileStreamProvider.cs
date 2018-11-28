using System.IO;

namespace HomeWorld.Torrent
{
    public interface IFileStreamProvider
    {
        Stream GetStream(string relativePath);
    }
}
