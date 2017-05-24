using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Comparers
{
    public class OrdinalStringComparer : IComparer<String>
    {
        public static OrdinalStringComparer Comparer { get; private set; } = new OrdinalStringComparer { };

        public Int32 Compare(String x, String y)
        {
            return String.Compare(
                String.Join(" ", x.Split(' ').Select(s => $"{s.ToInt64()} {s}")),
                String.Join(" ", y.Split(' ').Select(s => $"{s.ToInt64()} {s}")));
        }
    }
}
