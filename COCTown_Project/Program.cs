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
            // 유니코드 문자(● 등) 출력 안정화
            Console.OutputEncoding = Encoding.UTF8;
            Console.Clear();

            GameManager scene = new GameManager();
            scene.Run();
        }
    }
}
