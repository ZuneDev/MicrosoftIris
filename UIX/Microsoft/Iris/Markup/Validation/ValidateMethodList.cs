// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateMethodList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateMethodList : Microsoft.Iris.Markup.Validation.Validate
    {
        private ArrayList _methodList;
        private MethodSchema[] _foundMethods;

        public ValidateMethodList(SourceMarkupLoader owner, int line, int column)
          : base(owner, line, column)
          => _methodList = new ArrayList();

        public void AppendToEnd(ValidateMethod expression) => _methodList.Add(expression);

        public ArrayList Methods => _methodList;

        public void Validate(ValidateClass validateOwner, ValidateContext context)
        {
            int length = 0;
            for (int index1 = 0; index1 < _methodList.Count; ++index1)
            {
                ValidateMethod method = (ValidateMethod)_methodList[index1];
                method.Validate(validateOwner, context);
                if (method.HasErrors)
                    MarkHasErrors();
                if (method.MethodExport != null && !method.HasOverrideKeyword)
                    ++length;
                if (context.CurrentPass == LoadPass.PopulatePublicModel)
                {
                    for (int index2 = 0; index2 < index1; ++index2)
                    {
                        if (ValidateMethod.IsExactMatch(method.MethodExport, ((ValidateMethod)_methodList[index2]).MethodExport))
                        {
                            method.ReportError("Method '{0}' was already defined in this class with the same signature", method.MethodExport.Name);
                            MarkHasErrors();
                        }
                    }
                }
            }
            if (context.CurrentPass != LoadPass.PopulatePublicModel)
                return;
            MethodSchema[] methodSchemaArray = new MethodSchema[length];
            int num = 0;
            for (MarkupTypeSchema typeExport = validateOwner.TypeExport; typeExport != null; typeExport = typeExport.Base as MarkupTypeSchema)
            {
                foreach (MarkupMethodSchema method in typeExport.Methods)
                    num = Math.Max(num, method.VirtualId + 1);
            }
            int index = 0;
            foreach (ValidateMethod method in _methodList)
            {
                if (method.MethodExport != null && !method.HasOverrideKeyword)
                {
                    if (!method.HasVirtualKeyword)
                    {
                        methodSchemaArray[index] = method.MethodExport;
                    }
                    else
                    {
                        MarkupMethodSchema markupMethodSchema = MarkupMethodSchema.BuildVirtualThunk(validateOwner.ObjectType, method.MethodExport);
                        methodSchemaArray[index] = markupMethodSchema;
                        markupMethodSchema.SetVirtualId(num);
                        method.MethodExport.SetVirtualId(num);
                        ++num;
                    }
                    ++index;
                }
            }
            _foundMethods = methodSchemaArray;
        }

        public MethodSchema[] FoundMethods => _foundMethods;
    }
}
