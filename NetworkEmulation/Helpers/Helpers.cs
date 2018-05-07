using System;
namespace Helpers
{
    public static class Helpers
    {
        public static int HashCode(this object o) {
            var dry = Math.Abs(o.GetHashCode());
            return 10000 + dry % 45535;
        }
    }
}
