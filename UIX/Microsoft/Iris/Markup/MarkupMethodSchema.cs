// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup
{
    internal abstract class MarkupMethodSchema : MethodSchema
    {
        private string _name;
        private TypeSchema _returnType;
        private TypeSchema[] _parameterTypes;
        private string[] _parameterNames;
        protected uint _codeOffset = uint.MaxValue;
        private int _virtualId = -1;
        private bool _isVirtualThunk;
        public static readonly string[] s_emptyStringArray = new string[0];
        public static readonly MarkupMethodSchema[] EmptyMethodList = new MarkupMethodSchema[0];

        public static MarkupMethodSchema Build(
          TypeSchema markupTypeBase,
          MarkupTypeSchema owner,
          string name,
          TypeSchema returnType,
          TypeSchema[] parameterTypes,
          string[] parameterNames,
          bool isVirtualThunk)
        {
            MarkupMethodSchema markupMethodSchema = null;
            if (markupTypeBase == ClassSchema.Type || markupTypeBase == EffectSchema.Type)
                markupMethodSchema = new ClassMethodSchema((ClassTypeSchema)owner, name, returnType, parameterTypes, parameterNames);
            else if (markupTypeBase == UISchema.Type)
                markupMethodSchema = new UIClassMethodSchema((UIClassTypeSchema)owner, name, returnType, parameterTypes, parameterNames);
            markupMethodSchema._isVirtualThunk = isVirtualThunk;
            return markupMethodSchema;
        }

        public static MarkupMethodSchema Build(
          TypeSchema markupTypeBase,
          MarkupTypeSchema owner,
          string name,
          TypeSchema returnType,
          TypeSchema[] parameterTypes,
          string[] parameterNames)
        {
            return Build(markupTypeBase, owner, name, returnType, parameterTypes, parameterNames, false);
        }

        public static MarkupMethodSchema BuildVirtualThunk(
          TypeSchema markupTypeBase,
          MarkupMethodSchema virtualMethod)
        {
            return Build(markupTypeBase, (MarkupTypeSchema)virtualMethod.Owner, virtualMethod.Name, virtualMethod.ReturnType, virtualMethod.ParameterTypes, virtualMethod.ParameterNames, true);
        }

        protected MarkupMethodSchema(
          MarkupTypeSchema owner,
          string name,
          TypeSchema returnType,
          TypeSchema[] parameterTypes,
          string[] parameterNames)
          : base(owner)
        {
            _name = name;
            _returnType = returnType;
            _parameterTypes = parameterTypes;
            _parameterNames = parameterNames;
            for (int index = 0; index < _parameterNames.Length; ++index)
                _parameterNames[index] = NotifyService.CanonicalizeString(_parameterNames[index]);
        }

        public override string Name => _name;

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public string[] ParameterNames => _parameterNames;

        public override TypeSchema ReturnType => _returnType;

        public override bool IsStatic => false;

        public bool IsVirtual => _virtualId >= 0;

        public int VirtualId => _virtualId;

        public bool IsVirtualThunk => _isVirtualThunk;

        public uint CodeOffset => _codeOffset;

        public void SetCodeOffset(uint codeOffset) => _codeOffset = codeOffset;

        public void SetVirtualId(int virtualId) => _virtualId = virtualId;

        public override object Invoke(object instance, object[] parameters)
        {
            IMarkupTypeBase markupTypeBase = GetMarkupTypeBase(instance);
            if (markupTypeBase == null)
                return null;
            return _isVirtualThunk ? CallVirt(markupTypeBase, parameters) : CallDirect(markupTypeBase, parameters);
        }

        private object CallVirt(IMarkupTypeBase markupInstance, object[] parameters)
        {
            MarkupTypeSchema typeSchema = (MarkupTypeSchema)markupInstance.TypeSchema;
            MarkupMethodSchema markupMethodSchema = null;
            while (true)
            {
                foreach (MarkupMethodSchema virtualMethod in typeSchema.VirtualMethods)
                {
                    if (virtualMethod.VirtualId == _virtualId)
                    {
                        markupMethodSchema = virtualMethod;
                        break;
                    }
                }
                if (markupMethodSchema == null)
                    typeSchema = (MarkupTypeSchema)typeSchema.Base;
                else
                    break;
            }
            return markupMethodSchema.CallDirect(markupInstance, parameters);
        }

        private object CallDirect(IMarkupTypeBase markupInstance, object[] parameters) => markupInstance.RunScript(_codeOffset, false, new ParameterContext(_parameterNames, parameters));

        protected abstract IMarkupTypeBase GetMarkupTypeBase(object instance);
    }
}
