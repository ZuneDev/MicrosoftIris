// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Window
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Iris
{
    public class Window : INotifyPropertyChanged
    {
        private WindowPositionPolicy _initialPositionPolicy;
        private UIForm _form;

        internal Window(UIForm form)
        {
            _form = form;
            _form.CloseRequested += new FormCloseRequestedHandler(OnCloseRequested);
            _form.SessionConnect += new FormSessionConnectHandler(OnSessionConnect);
            _form.PropertyChanged += new FormPropertyChangedHandler(OnPropertyChanged);
        }

        public WindowSize InitialClientSize
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                Size initialClientSize = _form.InitialClientSize;
                return new WindowSize(initialClientSize.Width, initialClientSize.Height);
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.InitialClientSize = new Size(value.Width, value.Height);
            }
        }

        public WindowSize ClientSize
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                Size clientSize = _form.ClientSize;
                return new WindowSize(clientSize.Width, clientSize.Height);
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.ClientSize = new Size(value.Width, value.Height);
            }
        }

        public WindowPosition Position
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                Point position = _form.Position;
                return new WindowPosition(position.X, position.Y);
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.Position = new Point(value.X, value.Y);
            }
        }

        public WindowState WindowState
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.WindowState;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.WindowState = value;
            }
        }

        public bool Visible
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.Visible;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.Visible = value;
            }
        }

        public bool Active => _form.ActivationState;

        public bool MouseActive => !_form.MouseIsIdle;

        public string GetSavedPosition() => GetSavedPosition(false);

        public string GetSavedPosition(bool disallowMinimized)
        {
            UIDispatcher.VerifyOnApplicationThread();
            FormPlacement finalPlacement = _form.FinalPlacement;
            if (finalPlacement.ShowState == 2U && disallowMinimized)
                finalPlacement.ShowState = 1U;
            return InvariantString.Format("{0},{1},{2},{3},{4},{5},{6}", finalPlacement.ShowState, finalPlacement.NormalPosition.X, finalPlacement.NormalPosition.Y, finalPlacement.NormalPosition.Width, finalPlacement.NormalPosition.Height, finalPlacement.MaximizedLocation.X, finalPlacement.MaximizedLocation.Y);
        }

        public void SetSavedInitialPosition(string cookie) => SetSavedInitialPositionWorker(cookie, 0U);

        public void SetSavedInitialPosition(string cookie, WindowState stateOverride)
        {
            uint showStateOverride;
            switch (stateOverride)
            {
                case WindowState.Normal:
                    showStateOverride = 1U;
                    break;
                case WindowState.Minimized:
                    showStateOverride = 2U;
                    break;
                case WindowState.Maximized:
                    showStateOverride = 3U;
                    break;
                default:
                    throw new ArgumentException("Invalid WindowState value");
            }
            SetSavedInitialPositionWorker(cookie, showStateOverride);
        }

        private void SetSavedInitialPositionWorker(string cookie, uint showStateOverride)
        {
            UIDispatcher.VerifyOnApplicationThread();
            string[] strArray = cookie != null ? cookie.Split(',') : throw new ArgumentNullException(nameof(cookie));
            bool flag = false;
            if (strArray.Length == 7)
            {
                try
                {
                    FormPlacement formPlacement;
                    formPlacement.ShowState = uint.Parse(strArray[0], NumberFormatInfo.InvariantInfo);
                    formPlacement.NormalPosition = new Rectangle(int.Parse(strArray[1], NumberFormatInfo.InvariantInfo), int.Parse(strArray[2], NumberFormatInfo.InvariantInfo), int.Parse(strArray[3], NumberFormatInfo.InvariantInfo), int.Parse(strArray[4], NumberFormatInfo.InvariantInfo));
                    formPlacement.MaximizedLocation = new Point(int.Parse(strArray[5], NumberFormatInfo.InvariantInfo), int.Parse(strArray[6], NumberFormatInfo.InvariantInfo));
                    if (showStateOverride != 0U)
                        formPlacement.ShowState = showStateOverride;
                    if (formPlacement.NormalPosition.Width != 0)
                    {
                        if (formPlacement.NormalPosition.Height != 0)
                            _form.InitialPlacement = formPlacement;
                    }
                }
                catch (FormatException ex)
                {
                    flag = true;
                }
                catch (OverflowException ex)
                {
                    flag = true;
                }
            }
            else
                flag = true;
            if (flag)
                throw new ArgumentOutOfRangeException("Invalid saved window position.");
        }

        public string Caption
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.Text;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.Text = value;
            }
        }

        public bool AlwaysOnTop
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.AlwaysOnTop;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.AlwaysOnTop = value;
            }
        }

        public bool ShowInTaskbar
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.ShowInTaskbar;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.ShowInTaskbar = value;
            }
        }

        public bool ShowWindowFrame
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.ShowWindowFrame;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.ShowWindowFrame = value;
            }
        }

        public bool ShowShadow
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.ShowShadow;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.ShowShadow = value;
            }
        }

        public MaximizeMode MaximizeMode
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.MaximizeMode;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.MaximizeMode = value;
            }
        }

        public bool RespectsStartupSettings
        {
            get => _form.RespectsStartupSettings;
            set => _form.RespectsStartupSettings = value;
        }

        public WindowPositionPolicy InitialPositionPolicy
        {
            get => _initialPositionPolicy;
            set
            {
                _form.StartCentered = (value & WindowPositionPolicy.CenterOnWorkArea) != WindowPositionPolicy.None;
                _form.StartInWorkArea = (value & WindowPositionPolicy.ConstrainToWorkArea) != WindowPositionPolicy.None;
                _initialPositionPolicy = value;
            }
        }

        public IntPtr Handle
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.__WindowHandle;
            }
        }

        public void SetBackgroundColor(WindowColor color)
        {
            UIDispatcher.VerifyOnApplicationThread();
            _form.SetBackgroundColor(color.GetInternalColor());
        }

        public int MouseIdleTimeout
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.MouseIdleTimeout;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.MouseIdleTimeout = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public bool HideMouseOnIdle
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _form.HideMouseOnIdle;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                _form.HideMouseOnIdle = value;
            }
        }

        public void RequestLoad(string source) => RequestLoad(source, null);

        public void RequestLoad(string source, PropertyValue[] properties)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Vector<UIPropertyRecord> properties1 = null;
            if (properties != null)
            {
                properties1 = new Vector<UIPropertyRecord>(properties.Length);
                foreach (PropertyValue property in properties)
                    properties1.Add(new UIPropertyRecord()
                    {
                        Name = property.Name,
                        Value = property.Value
                    });
            }
            _form.RequestLoad(source, properties1);
        }

        public void SetIcon(string moduleName, int resourceID)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));
            _form.SetIcon(moduleName, (uint)resourceID, IconFlags.All);
        }

        public void SetShadowEdgeImages(bool fActiveEdges, Image[] images)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (images == null || images.Length != 4)
                throw new ArgumentException("Must provide exactly 4 images: LTRB edges", nameof(images));
            for (int index = 0; index < 4; ++index)
            {
                if (images[index] == null)
                    throw new ArgumentException("Edge image cannot be null", "image[idx]:" + index.ToString());
                if (string.IsNullOrEmpty(images[index].Source))
                    throw new ArgumentException("Edge image cannot have null or empty Source", "image[idx]:" + index.ToString());
            }
            UIImage[] images1 = new UIImage[4];
            for (int index = 0; index < 4; ++index)
                images1[index] = images[index].UIImage;
            _form.SetShadowImages(fActiveEdges, images1);
        }

        public object SaveKeyFocus()
        {
            UIDispatcher.VerifyOnApplicationThread();
            return _form.SaveKeyFocus();
        }

        public void RestoreKeyFocus(object handle)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (handle is SavedKeyFocus state)
                _form.RestoreKeyFocus(state);
            else if (handle != null)
                throw new ArgumentException("RestoreKeyFocus expects an object given out from SaveKeyFocus");
        }

        public void Close()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _form.Close();
        }

        public void ForceClose()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _form.ForceClose();
        }

        private void OnCloseRequested(FormCloseReason nReason, ref bool block)
        {
            if (CloseRequested == null)
                return;
            WindowCloseRequestedEventArgs args = new WindowCloseRequestedEventArgs();
            CloseRequested(this, args);
            block = args.Block;
        }

        public event WindowCloseRequestedHandler CloseRequested;

        private void OnSessionConnect(object sender, bool fIsConnected)
        {
            if (SessionConnected == null)
                return;
            SessionConnected(sender, fIsConnected);
        }

        public event SessionConnectedHandler SessionConnected;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
