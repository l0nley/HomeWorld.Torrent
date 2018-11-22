using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HomeWorld.Torrent;

namespace HomeWorld.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            TorrentMetadata meta = null;
            using (var stream = File.OpenRead("soldier.torrent"))
            {
                meta = stream.ReadTorrentFile();
            }

            using (var fl = File.OpenRead(meta.Files.First().Key))
            {
                var result = await fl.ValidateFileAsync(meta);
            }

            System.Console.ReadLine();
        }
    }
}
