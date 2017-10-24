using Dolkens.Framework.Caching;
using Dolkens.Framework.Telemetry;
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
            try
            {
                using (new TimedSection("Custom.Section"))
                {
                    String.Format("Final {0}", "section");

                    using (new TimedSection("Custom.Section"))
                    {
                        var test1 = TimedSection.Run(String.Format, "Hello {0}", "world");

                        TimedSection.Run(Program.Test, "Hello {0}", "world");

                        throw new NotImplementedException("Another Exception Here");
                    }
                }
            }
            catch (Exception)
            {
                var test2 = TimedSection.Run(String.Format, "Foo {0}", "bar");
            }

            var test3 = TimedSection.Run(String.Format, "Abra {0}", "kadabra");
        }

        static void Test()
        {
            throw new NotImplementedException("HGi");
        }
    }
}
