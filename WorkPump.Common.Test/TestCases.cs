using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPump.Common.Test
{
    public static class TestCases
    {
        public static readonly IEnumerable<ulong> UInt64
            = new[]
            {
                ulong.MinValue,
                1UL,
                10UL,
                100UL,
                ulong.MaxValue
            };
    }
}
