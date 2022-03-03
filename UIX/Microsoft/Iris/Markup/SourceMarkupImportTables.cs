// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.SourceMarkupImportTables
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class SourceMarkupImportTables
    {
        public Vector ImportedLoadResults = new Vector();
        public Vector ImportedTypes = new Vector();
        public Vector ImportedConstructors = new Vector();
        public Vector ImportedProperties = new Vector();
        public Vector ImportedEvents = new Vector();
        public Vector ImportedMethods = new Vector();

        public MarkupImportTables PrepareImportTables()
        {
            MarkupImportTables markupImportTables = new MarkupImportTables();
            if (ImportedTypes.Count > 0)
            {
                TypeSchema[] typeSchemaArray = new TypeSchema[ImportedTypes.Count];
                for (int index = 0; index < ImportedTypes.Count; ++index)
                    typeSchemaArray[index] = (TypeSchema)ImportedTypes[index];
                markupImportTables.TypeImports = typeSchemaArray;
            }
            if (ImportedConstructors.Count > 0)
            {
                ConstructorSchema[] constructorSchemaArray = new ConstructorSchema[ImportedConstructors.Count];
                for (int index = 0; index < ImportedConstructors.Count; ++index)
                    constructorSchemaArray[index] = (ConstructorSchema)ImportedConstructors[index];
                markupImportTables.ConstructorImports = constructorSchemaArray;
            }
            if (ImportedProperties.Count > 0)
            {
                PropertySchema[] propertySchemaArray = new PropertySchema[ImportedProperties.Count];
                for (int index = 0; index < ImportedProperties.Count; ++index)
                    propertySchemaArray[index] = (PropertySchema)ImportedProperties[index];
                markupImportTables.PropertyImports = propertySchemaArray;
            }
            if (ImportedMethods.Count > 0)
            {
                MethodSchema[] methodSchemaArray = new MethodSchema[ImportedMethods.Count];
                for (int index = 0; index < ImportedMethods.Count; ++index)
                    methodSchemaArray[index] = (MethodSchema)ImportedMethods[index];
                markupImportTables.MethodImports = methodSchemaArray;
            }
            if (ImportedEvents.Count > 0)
            {
                EventSchema[] eventSchemaArray = new EventSchema[ImportedEvents.Count];
                for (int index = 0; index < ImportedEvents.Count; ++index)
                    eventSchemaArray[index] = (EventSchema)ImportedEvents[index];
                markupImportTables.EventImports = eventSchemaArray;
            }
            return markupImportTables;
        }
    }
}
