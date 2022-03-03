# Microsoft Iris UI

## What is Microsoft Iris?
Microsoft Iris (sometimes called UIX) was an internal UI framework for Windows, developed for internal use by various Microsoft Windows software. The code in this repository was originally obtained using dotPeek to decompile `UIX.dll` included in v4.8 of the Zune desktop software (which utilized Iris).

Unlike most modern UI frameworks, Iris does not provide default control styles-- library consumers are expected to define their own styles. However, some later versions of Iris included `UIXControls.dll`, a resource library containing some limited styles for built-in controls. UI layouts are defined in XML-based `.uix` files, which are nearly identical in structure to [Media Center Markup Language (MCML)](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/windows-media-center-sdk/bb189388(v=msdn.10)). UIX versions 3.x and later compiled the XML-based layouts to a proprietary binary format that is interpreted at runtime by the UIX library.

Parts of the code suggest that Iris may have supported the Xbox 360 (referred to by its codename, Xenon). There is also evidence that native (C++) libraries were used by some in-box applications in Windows Phone 7.

## Why does this repository exist?
Despite being very powerful and flexible, especially compared to other UI frameworks of the time, Microsoft never released Iris for public use. Microsoft even added [`Microsoft.Iris.Application.VerifyTrustedEnvironment()`](https://github.com/ZuneDev/MicrosoftIris/blob/ab33f58c69275df5cb31b557887b8853925371c9/UIX/Microsoft/Iris/Application.cs#L488-L510), which is called on startup of a UIX application that checks to make sure that the calling assembly is signed by Microsoft. This check has been modified to never fail, so that any project can reference UIX, regardless of who signed it (or if it's signed at all).

In the future, other changes may be made to accomodate the modern .NET ecosystem. For example, desktop versions of UIX all target .NET Framework 3.5, while modern .NET should target .NET 5+ (or .NET Standard 1.4/2.0 for non-Core platforms).