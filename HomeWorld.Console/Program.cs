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
            System.Console.ReadLine();
        }
    }
}
