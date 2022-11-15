using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salix.StackTracer.TestClasses;

internal class TestableMethods
{
    internal void Normal()
    {
        this.Throwing();
    }
    internal decimal Math(string causingZero)
    {
        var divider = decimal.Parse(causingZero);
        var floored = System.Math.Floor(divider);
        return 256M/floored; // throw when floored = 0
    }

    internal void WithSimpleParameters(int someId, string someName)
    {
        this.Throwing();
    }
    internal void WithNullableParameters(int? optionalId, bool? triState, string? optionalName)
    {
        this.Throwing();
    }

    internal void WithComplexParameters(StackTraceFrame someFrame)
    {
        this.Throwing();
    }

    internal void GenericMethod<TParm>(TParm typedParm)
    {
        this.Throwing();
    }

    internal void Throwing()
    {
        throw new ApplicationException("Here you go!");
    }
}
