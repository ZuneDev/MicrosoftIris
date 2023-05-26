// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.EventSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup
{
    public abstract class EventSchema : DisposableObject
    {
        private readonly TypeSchema _owner;
        public readonly static EventSchema[] EmptyList = System.Array.Empty<EventSchema>();

        public EventSchema(TypeSchema owner)
        {
            _owner = owner;
            DeclareOwner(owner);
        }

        public TypeSchema Owner => _owner;

        public abstract string Name { get; }

        public override string ToString() => $"{_owner.Name}.{Name}";
    }
}
