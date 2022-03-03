// Decompiled with JetBrains decompiler
// Type: KeyValueEntry
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

public struct KeyValueEntry
{
    private object _key;
    private object _value;

    public KeyValueEntry(object key, object value)
    {
        this._key = key;
        this._value = value;
    }

    public object Key => this._key;

    public object Value => this._value;
}
