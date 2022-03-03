// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.RendererApi
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.RenderAPI
{
    [SuppressUnmanagedCodeSecurity]
    internal sealed class RendererApi
    {
        public static void IFC(HRESULT hr)
        {
            if (hr.Int >= 0)
                return;
            RenderException.ErrorCode code = (RenderException.ErrorCode)hr.Int;
            switch (hr.Int)
            {
                case -2147221503:
                case -2147221502:
                    throw new OutOfMemoryException();
                case -2147221494:
                    throw new RenderException(code, "Generic failure.");
                case -2147221493:
                    throw new RenderException(code, "The object is in a \"busy\" state and is not available to process the request.");
                case -2147221492:
                    throw new RenderException(code, "The object is not in a usable state to process the request.");
                case -2147221484:
                    throw new RenderException(code, "The Context has not been initialized.");
                case -2147221474:
                    throw new RenderException(code, "The object was used in the incorrect context.");
                case -2147221473:
                    throw new RenderException(code, "The Context has been marked to only allow read-only operations.  For example, this may be in the middle of a read-only callback.");
                case -2147221472:
                    throw new RenderException(code, "The threading model has already be determined by a previous call to SpInit() and can no longer be changed.");
                case -2147221471:
                    throw new RenderException(code, "Unable to use the IGMM_STANDARD messaging model because it is either unsupported or cannot be installed.");
                case -2147221464:
                    throw new RenderException(code, "Can not mix an invalid coordinate mapping, for example having a non-relative child of a relative parent.");
                case -2147221454:
                    throw new RenderException(code, "Could not find a MSGID for one of the requested messages.  This will be represented by a '0' in the MSGID field for that message.");
                case -2147221444:
                    throw new RenderException(code, "The operation is not legal because the specified Gadget does not have a GS_BUFFERED style.");
                case -2147221434:
                    throw new RenderException(code, "The specific Gadget has started the destruction and can not be be modified in this manner.");
                case -2147221433:
                    throw new RenderException(code, "The specific object is locked and may not be modified.");
                case -2147221432:
                    throw new InvalidOperationException("The operation is not supported.");
                case -2147221424:
                    throw new RenderException(code, "The specified optional component has not yet been initialized with InitGadgetComponent().");
                case -2147221414:
                    throw new RenderException(code, "The specified object was not found.");
                case -2147221413:
                    throw new RenderException(code, "The ObjectID is already in use.");
                case -2147221412:
                    throw new FileNotFoundException("The specified file could not be found");
                case -2147221411:
                    throw new ArgumentOutOfRangeException("", "The argument is out of range");
                case -2147221404:
                    throw new RenderException(code, "The specified parmeters are mismatched for the current object state.");
                case -2147221394:
                    throw new RenderException(code, "GDI+ was unable to be loaded.  It may not be installed on the system or may not be properly initialized.");
                case -2147221393:
                    throw new RenderException(code, "Direct3D was unable to be loaded.  It may not be installed on the system or may not be properly initialized.");
                case -2147221384:
                    throw new RenderException(code, "The specified class was already registered.");
                case -2147221383:
                    throw new RenderException(code, "The specified message was not found during class registration.");
                case -2147221382:
                    throw new RenderException(code, "The specified message was not implemented during class registration.");
                case -2147221381:
                    throw new RenderException(code, "The implementation of the specific class has not yet been registered.");
                case -2147221380:
                    throw new RenderException(code, "Sending the message failed.");
                case -2147221379:
                    throw new RenderException(code, "The message data is too large.");
                case -2147221374:
                    throw new RenderException(code, "The specified object does not have any content.");
                case -2147221373:
                    throw new RenderException(code, "The specified object is not properly setup to store the data.");
                case -2147221364:
                    throw new RenderException(code, "Generic failure from Win32 that did not SetLastError().");
                case -2147221363:
                    throw new RenderException(code, "Generic failure from GDI+.");
                case -2147221362:
                    throw new RenderException(code, "Generic failure from driver or rendering.");
                case -2147221354:
                    throw new RenderException(code, "Unable to connect to remote renderer.");
                default:
                    Marshal.ThrowExceptionForHR(hr.Int);
                    break;
            }
        }
    }
}
