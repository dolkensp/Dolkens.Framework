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
            Fluent.Transfer
                .From("http://25.webseed.robertsspaceindustries.com/GameBuilds/sc-alpha-2.6.0/523988-c/StarCitizen/Data/Textures-part0.pak")
                .To("textures-part0.pak")
                .Via("temp.pak")
                .Start();
        }
    }
}
