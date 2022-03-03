// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateScripts
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateScripts
    {
        private ValidateProperty _scriptsProperty;
        private bool _hasErrors;

        public ValidateScripts(ValidateProperty scriptsProperty) => _scriptsProperty = scriptsProperty;

        public void Validate(ValidateContext context)
        {
            if (_scriptsProperty.Value != null && !_scriptsProperty.IsCodeValue)
            {
                _scriptsProperty.ReportError("Expecting <Script> block");
                MarkHasErrors();
            }
            else
            {
                ValidateCode next = (ValidateCode)_scriptsProperty.Value;
                int num = 0;
                while (next != null)
                {
                    next.Validate(new TypeRestriction(VoidSchema.Type), context);
                    if (!next.HasErrors)
                    {
                        context.RegisterAction(next);
                        next.MarkAsNotEmbedded();
                        foreach (ValidateExpression triggerExpression in next.DeclaredTriggerExpressions)
                            context.RegisterTrigger(triggerExpression, next);
                    }
                    next = next.Next;
                    ++num;
                }
            }
        }

        public bool HasErrors => _hasErrors;

        private void MarkHasErrors() => _hasErrors = true;
    }
}
