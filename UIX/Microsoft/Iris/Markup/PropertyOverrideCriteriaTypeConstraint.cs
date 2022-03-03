// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.PropertyOverrideCriteriaTypeConstraint
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup
{
    internal class PropertyOverrideCriteriaTypeConstraint : PropertyOverrideCriteria
    {
        private TypeSchema _use;
        private TypeSchema _constraint;

        public PropertyOverrideCriteriaTypeConstraint(TypeSchema use, TypeSchema constraint)
        {
            _use = use;
            _constraint = constraint;
        }

        public override Result Verify(PropertyOverrideCriteria baseCriteria)
        {
            PropertyOverrideCriteriaTypeConstraint criteriaTypeConstraint = (PropertyOverrideCriteriaTypeConstraint)baseCriteria;
            if (!criteriaTypeConstraint.Constraint.IsAssignableFrom(_use))
                return Result.Fail(string.Format("Type parameter property '{0}' is of type '{1}' which is not compatible with the base type constraint '{2}'", "Use", _use.Name, criteriaTypeConstraint.Constraint.Name));
            return !criteriaTypeConstraint.Constraint.IsAssignableFrom(_constraint) ? Result.Fail(string.Format("Type parameter property '{0}' is of type '{1}' which is not compatible with the base type constraint '{2}'", "Constraint", _constraint.Name, criteriaTypeConstraint.Constraint.Name)) : Result.Success;
        }

        public TypeSchema Use => _use;

        public TypeSchema Constraint => _constraint;
    }
}
