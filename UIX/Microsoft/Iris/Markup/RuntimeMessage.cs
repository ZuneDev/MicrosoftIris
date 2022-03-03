// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.RuntimeMessage
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal static class RuntimeMessage
    {
        public const string AccessibilityAccessWhenNotEnabled = "Accessibility: Script modifications to the 'Accessible' object ('{0}' property) detected even though an Accessibility client is not is use. Use 'if (Accessible.Enabled) {{ ... }}' to bypass Accessible property access in this case";
        public const string AnchorLayoutAnchorItemNotFound = "Anchor layout: {0} cannot find the '{1}' child";
        public const string AnchorLayoutInconsistentEdges = "All AnchorEdges must refer to actual positions on AnchorLayoutInput {0}.";
        public const string AnchorContributesActual = "AnchorLayoutInput {0} cannot contribute to width or height.";
        public const string AnimationInvalidKeyframeSingleType = "Animation must have at least 2 keyframes of each type. Attempted to play an animation that only has {0} keyframe of type '{1}'.";
        public const string AnimationInvalidKeyframeNoZeroForType = "Animation must have a keyframe at time 0.0 for each type. Attempted to play an animation that has no start keyframe for type '{0}'";
        public const string AnimationInvalidKeyframeCount = "Animations must have at least 2 keyframes to play";
        public const string DepersistInvalidFile = "Invalid compiled UIX file";
        public const string DepersistWrongFileVersion = "Compiled UIX file '{0}' was compiled for the runtime with version {1}, but the current runtime is version {2}";
        public const string DisposeAttemptOnNonDisposableObject = "Attempt to dispose an object '{0}' that isn't disposable";
        public const string DisposeAttemptUnownedObject = "Attempt to dispose an object '{0}' that '{1}' doesn't own";
        public const string ExplicitResourceRedirectFailure = "Resource {0} not found, but should have been located by a markup redirect";
        public const string FileNotFound = "File not found: '{0}'";
        public const string FontResourceNotFound = "Font Resource {1} not found in module {0}";
        public const string HostCannotInvokeMethods = "Host '{0}' is currently not hosting a UI and therefore cannot invoke methods";
        public const string HostIsNotUnloadable = "UnloadAll may only be called on Unloadable hosts";
        public const string HostRequestReplacementTypeMustBeUI = "RequestSource failed: Referrenced type '{0}' is not a UI";
        public const string HostRequestSourceUriLoadFailed = "RequestSource failed: Unable to load '{0}'";
        public const string HostRequestUIDoesNotMatchTypeRestriction = "RequestSource failed: Found '{0}' within '{1}', but, it is not a '{2}'";
        public const string HostRequestUINotFoundWithinMarkupLoadResult = "RequestSource failed: Unable to find '{0}' within '{1}'";
        public const string HwndHostBackgroundSolidColor = "HwndHost.Background must be a solid color";
        public const string InterpreterChildAddOperation = "Child Add";
        public const string InterpreterConstructionOperation = "Construction";
        public const string InterpreterConversionFailed = "Script runtime failure: Type conversion failed while attempting to convert to '{0}' ({1})";
        public const string InterpreterDictionaryAddOperation = "Dictionary Add";
        public const string InterpreterDictionaryContainsOperation = "Dictionary Contains";
        public const string InterpreterDynamicConstructionTypeMismatch = "Script runtime failure: Dynamic construction type override failed. Attempting to construct '{0}' in place of '{1}'";
        public const string InterpreterInitializeOperation = "Initialize";
        public const string InterpreterInvalidCast = "Script runtime failure: Invalid type cast while attempting to cast an instance with a runtime type of '{0}' to '{1}'";
        public const string InterpreterListAddOperation = "List Add";
        public const string InterpreterMethodInvokeOperation = "Method Invoke";
        public const string InterpreterNullReferenceMember = "Script runtime failure: Null-reference while attempting a '{0}' of '{1}' on a null instance";
        public const string InterpreterNullReference = "Script runtime failure: Null-reference while attempting a '{0}'";
        public const string InterpreterObjectAlreadyDisposed = "Script runtime failure: Attempting a '{0}' of '{1}' on an object '{2}' that has already been disposed";
        public const string InterpreterPropertyGetOperation = "Property Get";
        public const string InterpreterPropertySetOperation = "Property Set";
        public const string InterpreterReplacementTypeInvalidPropertyValue = "Script runtime failure: Incompatible value for property '{0}' supplied (expecting values of type '{1}' but got '{2}') while constructing runtime replacement type '{3}' (original type '{4}')";
        public const string InterpreterTypeConversionOperation = "Type Conversion";
        public const string InterpreterVerifyTypeCastOperation = "Verify Type Cast";
        public const string InvalidResourceScheme = "Invalid resource protocol: '{0}'";
        public const string InvalidResourceUri = "Invalid resource uri: '{0}'";
        public const string KeyHandlerNeedToTrackInvokedKeys = "KeyHandler needs to be marked TrackInvokedKeys=\"true\" in order to call GetInvokedKeys()";
        public const string MarkupTypeScriptErrorsWhileBuildingNamedContent = "Script runtime failure: Scripting errors have prevented '{0}' named content from being constructed";
        public const string MarkupTypeScriptErrorsWhileInitializing = "Script runtime failure: Scripting errors have prevented '{0}' from properly initializing and will affect its operation";
        public const string MarkupTypeScriptsDisabledDueToErrors = "Script runtime failure: Scripting has been disabled for '{0}' due to runtime scripting errors";
        public const string NoValidFocusTarget = "No valid focus target found. Input from a keyboard or remote will be ignored until focus is restored";
        public const string RepeaterInvalidContent = "Repeater failed to find content to repeat. ContentName was '{0}'";
        public const string RepeaterInvalidInlineContent = "Repeater unable to create inline content";
        public const string RepeaterInvalidDividerContent = "Repeater failed to find divider content to repeat (DividerName was '{0}')";
        public const string RepeaterNoContent = "Repeater has no content to repeat";
        public const string ReplacementTypeMissingRequiredProperty = "Runtime UI replacement to '{0}' failed since required property '{1}' was never provided a value";
        public const string ReplacementTypeInvalidProperty = "Runtime UI replacement to '{0}' failed since a property named '{1}' was specified but doesn't exist on '{0}'";
        public const string ReplacementTypeInvalidPropertyValue = "Runtime UI replacement to '{0}' failed since the value specified ({1}) for the '{2}' property is incompatible (type expected is '{3}')";
        public const string ResourceAcquireFailed = "Failed to acquire resource '{0}'";
        public const string ResourceAcquireFailedNoAsync = "Failed to acquire resource '{0}'.  Resources that cannot be fetched synchronously are not valid in this context";
        public const string ResourceDownloadFailed = "Failed to complete download from '{0}'";
        public const string ResourceInvalidUri = "Invalid URI: '{0}'";
        public const string ResourceHttpHostConnectionFailed = "Unable to connect to web host: '{0}'";
        public const string ResourceNotFound = "Resource not found: res://{0}!{1}";
        public const string SchemaArgumentInvalidParameter = "Script runtime failure: Invalid '{0}' value  for '{1}'";
        public const string SchemaArgumentInvalidParameterListContents = "Script runtime failure: Invalid value '{0}' within list '{1}'";
        public const string SchemaArgumentNullParameter = "Script runtime failure: Invalid 'null' value for '{0}'";
        public const string SchemaArgumentOutOfRangeParameter = "Script runtime failure: Invalid '{0}' value is out of range for '{1}'";
        public const string SelectionOutsideList = "Selected Index {0} is not a valid index in SourceList of size {1}";
        public const string SelectionOperationNotSupported = "Calling {0} is not supported on a SelectionManager in single selection modes.";
        public const string SelectionCannotModifySelection = "Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.";
        public const string TextEditableComplexFormattingDisallowed = "Text: Complex formatting unsupported on text that is editable";
        public const string TextScrollModelAttachedToMultipleHandlers = "TextScrollModel can't be attached to multiple TextEditingHandlers";
        public const string TooManyUnloadableHosts = "Maximum number of Unloadable Hosts reached";
    }
}
