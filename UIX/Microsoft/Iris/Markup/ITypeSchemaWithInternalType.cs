using System;

namespace Microsoft.Iris.Markup;

public interface ITypeSchemaWithInternalType
{
    Type InternalType { get; }
}
