using Dolkens.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime? test = null;

            var isBigger = DateTime.Today > test;

            
            Fluent.Transfer
                .From("http://25.webseed.robertsspaceindustries.com/GameBuilds/sc-alpha-2.6.0/523988-c/StarCitizen/Data/Textures-part0.pak")
                .To("textures-part0.pak")
                .Via("temp.pak")
                .WithChunks()
                .Start();

            MethodDelegate<Object> testMethod = (Object[] args2) => { return new { Foo = "bar", Hello = "world", Random = DateTime.Now.Ticks }; };

            var test1 = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, testMethod, 123);

            var test2 = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, testMethod, 123);

            CacheUtils.DeleteCachedData(CacheUtils.DefaultSettings, testMethod, 123);

            var test3 = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, testMethod, 123);

            
        }
    }
}
