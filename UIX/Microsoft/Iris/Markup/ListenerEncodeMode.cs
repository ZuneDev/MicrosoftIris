// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ListenerEncodeMode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.Validation;

namespace Microsoft.Iris.Markup
{
    internal class ListenerEncodeMode
    {
        public TriggerRecord TriggerContainer;
        private uint _totalListenersOnBaseType;
        private MarkupTypeSchema _typeSchema;

        public ListenerEncodeMode(MarkupTypeSchema typeSchema)
        {
            _typeSchema = typeSchema;
            if (typeSchema.MarkupTypeBase == null)
                return;
            _totalListenersOnBaseType = typeSchema.MarkupTypeBase.TotalListenerCount;
        }

        public uint SequentialListenerIndex(uint localListenerIndex) => _totalListenersOnBaseType + localListenerIndex;

        public uint ScriptId => _typeSchema.EncodeScriptOffsetAsId(TriggerContainer.ActionCode.EncodeStartOffset);

        public uint RefreshId => _typeSchema.EncodeScriptOffsetAsId(TriggerContainer.SourceExpression.EncodeStartOffset);

        public bool RunOnNonTailTrigger => TriggerContainer.SourceExpression.ExpressionType != ExpressionType.Call || ((ValidateExpressionCall)TriggerContainer.SourceExpression).FoundMemberType != SchemaType.Event;
    }
}
