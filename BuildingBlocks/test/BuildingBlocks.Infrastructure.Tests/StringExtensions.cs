using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}