using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework
{
    public class BenchmarkSection : IDisposable
    {
        private String _name;
        private Stopwatch _sw;

        public BenchmarkSection(String name)
        {
            this._name = name;
            this._sw = new Stopwatch();
            this._sw.Start();
        }

        public void Dispose()
        {
            this._sw.Stop();
            Debug.WriteLine("[{0}]: {1:#,###0}", this._name, this._sw.ElapsedMilliseconds);
        }
    }
}
