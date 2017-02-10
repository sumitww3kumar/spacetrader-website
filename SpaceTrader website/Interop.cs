using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;

namespace SpaceTrader
{
    public static class Interop
    {
        public static T Get<T>(this FSharpOption<T> option, Func<T> or = null)
        {
            if (FSharpOption<T>.get_IsSome(option))
            {
                return option.Value;
            }
            else
            {
                return or != null ? or.Invoke() : default(T);
            }
        }
    }
}