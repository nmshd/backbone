using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enmeshed.Tooling.Extensions;
public static class EnvironmentVariables
{
    public static readonly bool DEBUG_PERFORMANCE = !Environment.GetEnvironmentVariable("DEBUG_PERFORMANCE").IsNullOrEmpty();
}
