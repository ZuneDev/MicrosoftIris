// Decompiled with JetBrains decompiler
// Type: KeyValueEntry`2
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

public struct KeyValueEntry<K, V>
{
    private K _key;
    private V _value;

    public KeyValueEntry(K key, V value)
    {
        this._key = key;
        this._value = value;
    }

    public K Key => this._key;

    public V Value => this._value;
}
