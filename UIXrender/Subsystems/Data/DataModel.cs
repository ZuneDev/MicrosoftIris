using System;
using System.Collections.Generic;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Data;

// Managed model behind the SpData* family (UIX/Microsoft/Iris/OS/NativeApi.cs) -- the
// native side of Iris's data-binding/query system, i.e. how a markup-declared binding
// reaches a provider's query result.
//
// Mirrors Microsoft.Iris.DataProviderQueryStatus exactly (UIX/Microsoft/Iris/DataProviderQueryStatus.cs).
public enum DataProviderQueryStatus
{
    Idle,
    RequestingData,
    ProcessingData,
    Complete,
    Error,
}

// The base object both queries and provider-produced result objects derive from -- backs
// SpDataBaseObjectGet/SetProperty and SpDataBaseObjectGet/SetInternalHandle.
internal class DataBaseObject
{
    private readonly Dictionary<string, UIXVariant> _properties = new(StringComparer.Ordinal);

    public ulong TypeHandle { get; set; }

    // Opaque to us: the framework's own handle for the object this one shadows.
    public ulong InternalHandle { get; set; }

    public bool TryGetProperty(string name, out UIXVariant value)
    {
        if (name == null)
        {
            value = default;
            return false;
        }
        return _properties.TryGetValue(name, out value);
    }

    public void SetProperty(string name, UIXVariant value)
    {
        if (name != null)
            _properties[name] = value;
    }
}

internal sealed class DataQuery : DataBaseObject
{
    public DataQuery(string providerName, ulong queryTypeHandle, ulong resultTypeHandle, ulong queryHandle)
    {
        ProviderName = providerName;
        QueryTypeHandle = queryTypeHandle;
        ResultTypeHandle = resultTypeHandle;
        InternalHandle = queryHandle;
        TypeHandle = queryTypeHandle;
    }

    public string ProviderName { get; }
    public ulong QueryTypeHandle { get; }
    public ulong ResultTypeHandle { get; }

    public bool Enabled { get; set; }
    public DataProviderQueryStatus Status { get; set; } = DataProviderQueryStatus.Idle;
    public UIXVariant Result { get; set; }
    public bool Initialized { get; private set; }

    public void NotifyInitialized() => Initialized = true;

    // A refresh on a disabled query is a no-op in the original's vocabulary (Enabled is
    // exactly the "should this query run" switch), so status only advances when enabled.
    // Nothing here fetches real data -- there is no provider implementation on this side
    // of the boundary; the provider lives in managed framework code and pushes results
    // back in through SpDataQuerySetResultProperty.
    public void Refresh()
    {
        if (!Enabled)
        {
            Status = DataProviderQueryStatus.Idle;
            return;
        }

        Status = DataProviderQueryStatus.RequestingData;
    }
}

// Backs SpDataProviderConstructQuery/SpDataProviderReportDataMapping.
internal sealed class DataProviderFactory
{
    private readonly List<DataMapping> _mappings = new();

    public IReadOnlyList<DataMapping> Mappings => _mappings;

    public void ReportMapping(DataMapping mapping) => _mappings.Add(mapping);
}

// Managed projection of NativeApi.NativeDataMappingEntry -- read as a struct of marshaled
// strings/handles at the boundary, kept as an ordinary object here.
internal sealed class DataMapping
{
    public string ProviderName { get; init; }
    public string Source { get; init; }
    public string Target { get; init; }
    public string PropertyName { get; init; }
    public ulong PropertyTypeHandle { get; init; }
    public string PropertyTypeName { get; init; }
    public ulong UnderlyingCollectionTypeHandle { get; init; }
    public string UnderlyingCollectionTypeName { get; init; }
    public UIXVariant DefaultValue { get; init; }
}
