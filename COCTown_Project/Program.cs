using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COCTown_Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            GameManager scene = new GameManager();
            scene.Run();
        }
    }
}
