﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.SessionInputHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.UI
{
    public delegate void SessionInputHandler(InputInfo originalEvent, EventRouteStages handledStage);
}
