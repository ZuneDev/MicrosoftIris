# Microsoft Iris UI

## What is Microsoft Iris?
Microsoft Iris (sometimes called UIX) was an internal UI framework for Windows, developed for internal use by various Microsoft Windows software. The code in this repository was originally obtained using dotPeek to decompile `UIX.dll` included in v4.8 of the Zune desktop software (which utilized Iris).

Unlike most modern UI frameworks, Iris does not provide default control styles-- library consumers are expected to define their own styles. However, some later versions of Iris included `UIXControls.dll`, a resource library containing some limited styles for built-in controls. UI layouts are defined in XML-based `.uix` files, which are nearly identical in structure to [Media Center Markup Language (MCML)](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/windows-media-center-sdk/bb189388(v=msdn.10)). UIX versions 3.x and later compiled the XML-based layouts to a proprietary binary format that is interpreted at runtime by the UIX library.

Parts of the code suggest that Iris may have supported the Xbox 360 (referred to by its codename, Xenon).