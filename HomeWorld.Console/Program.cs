using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
