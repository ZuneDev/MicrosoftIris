// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.NativeCodeModelMessage
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal static class NativeCodeModelMessage
    {
        public const string NativeChangeNotificationIDInvalid = "ChangeNotification received for ID '0x{0:X8}' which isn't a property or event on '{1}'";
        public const string NativeTypeIDInvalid = "Unable to find type with ID '0x{0:X8}' in '{1}'";
        public const string SchemaAPIFailed = "Schema API failure: A method on '{0}' failed with code '0x{1:X8}'";
        public const string SchemaParameterlessConstructor = "IUIXConstructors must have parameters";
        public const string SchemaCreationFailed = "Unable to create IUIXSchema from '{0}'";
        public const string SchemaDuplicateID = "Duplicate ID '0x{0:X8}' found in schema from '{1}'";
        public const string SchemaFactoryCreationFailed = "Unable to create IUIXSchemaFactory from '{0}'";
        public const string SchemaIDMismatch = "Schema component on ID '0x{0:X8}' doesn't match schema's ID '0x{1:X8}' on '{2}'";
        public const string SchemaInvalidHandleValue = "IUIXObject::GetStateCache retrieved unexpected value";
        public const string SchemaInvalidChangeNotificationID = "Invalid UIXID '0x{0:X8}' passed to NotifyChange";
        public const string SchemaInvalidMarshalAs = "Invalid MarshalAs '{0}' returned from IUIXType::MarshalAs";
        public const string SchemaInterfaceNotImplemented = "Object didn't implement expected interface '{0}'";
        public const string SchemaMultipleMarshalAs = "Invalid MarshalAs '{0}' on type '{1}'.  Only a single MarshalAs value is permitted through a type heirarchy, and '{2}' was already declared.";
        public const string SchemaNativeObjectNull = "NULL object returned from {0}";
        public const string MethodInvokeFailed = "Error 0x{0:X8} occurred invoking method {1}.{2} from {3}.";
        public const string PropertyGetFailed = "Error 0x{0:X8} occurred reading property {1}.{2} from {3}.";
        public const string PropertySetFailed = "Error 0x{0:X8} occurred writing property {1}.{2} from {3}.";
        public const string ToStringFailed = "Error 0x{0:X8} occurred invoking ToString on type {1} from {2}.";
        public const string ConstructFailed = "Error 0x{0:X8} occurred constructing object of type {1} from {2}.";
    }
}
