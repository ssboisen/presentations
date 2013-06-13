using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core;

namespace BatchProducer
{
    public static class FsharpExtensions
    {
        public static FSharpOption<T> AsSome<T>(this T input)
        {
            return new FSharpOption<T>(input);
        }
    }
}
