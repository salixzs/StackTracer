using System.Globalization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Salix.StackTracer.TestClasses;
#pragma warning restore IDE0130 // Namespace does not match folder structure

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
internal sealed class TestableMethods
{
    internal void Normal() => this.Throwing();

    internal decimal Math(string causingZero)
    {
        var divider = decimal.Parse(causingZero, NumberStyles.Integer, CultureInfo.DefaultThreadCurrentCulture);
        var floored = System.Math.Floor(divider);
        return 256M / floored; // throw when floored = 0
    }

    internal void WithSimpleParameters(int someId, string someName) => this.Throwing();

    internal void WithNullableParameters(int? optionalId, bool? triState, string? optionalName) =>
        this.Throwing();

    internal void WithComplexParameters(StackTraceFrame someFrame) => this.Throwing();

    internal void GenericMethod<TParm>(TParm typedParm) => this.Throwing();

#pragma warning disable CA2201 // Do not raise reserved exception types
    internal void Throwing() => throw new ApplicationException("Here you go!");
#pragma warning restore CA2201 // Do not raise reserved exception types
}
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
