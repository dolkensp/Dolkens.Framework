using Dolkens.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, () => { return "Hello World"; });

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
