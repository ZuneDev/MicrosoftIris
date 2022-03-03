// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.DataBufferTracker
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal class DataBufferTracker
    {
        private Vector m_alTracking;

        internal DataBufferTracker(object objUser) => this.m_alTracking = new Vector();

        ~DataBufferTracker() => this.Dispose(false);

        internal void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        private void Dispose(bool fInDispose)
        {
            if (fInDispose && this.m_alTracking != null && this.m_alTracking.Count > 0)
            {
                foreach (DataBufferTracker.TrackingInfo trackingInfo in this.m_alTracking)
                    trackingInfo.DataBuffer.Dispose();
                this.m_alTracking.Clear();
            }
            this.m_alTracking = null;
        }

        internal int Count => this.m_alTracking.Count;

        internal string DEBUG_Description => (string)null;

        internal object DEBUG_User => (object)null;

        internal event EventHandler TrackerEmpty;

        protected virtual void OnTrackerEmpty(object sender, EventArgs args)
        {
            if (this.TrackerEmpty == null)
                return;
            this.TrackerEmpty(sender, args);
        }

        internal void Track(DataBuffer dataBuffer) => this.Track(dataBuffer, null, null);

        internal void Track(
          DataBuffer dataBuffer,
          DataBufferTracker.CleanupEventHandler handler,
          object contextData)
        {
            DataBufferTracker.TrackingInfo trackingInfo = new DataBufferTracker.TrackingInfo(dataBuffer, Users.Valid, handler, contextData);
            dataBuffer.DataConsumed += new EventHandler(this.OnDataConsumed);
            dataBuffer.ReleaseLocalData += new EventHandler(this.OnReleaseLocalData);
            this.m_alTracking.Add(trackingInfo);
        }

        internal void Release(DataBuffer dataBuffer, DataBufferTracker.Users userFlags)
        {
            DataBufferTracker.TrackingInfo trackingInfo1 = null;
            foreach (DataBufferTracker.TrackingInfo trackingInfo2 in this.m_alTracking)
            {
                if (trackingInfo2.DataBuffer == dataBuffer)
                {
                    trackingInfo1 = trackingInfo2;
                    break;
                }
            }
            if (trackingInfo1 == null)
                return;
            uint userFlags1 = (uint)trackingInfo1.UserFlags;
            Bits.ClearFlag(ref userFlags1, (uint)userFlags);
            trackingInfo1.UserFlags = (DataBufferTracker.Users)userFlags1;
            if (userFlags1 != 0U)
                return;
            if (trackingInfo1.CleanupHandler != null)
                trackingInfo1.CleanupHandler(this, new DataBufferTracker.CleanupEventArgs(trackingInfo1.CleanupHandlerData));
            this.m_alTracking.Remove(trackingInfo1);
            trackingInfo1.DataBuffer.DataConsumed -= new EventHandler(this.OnDataConsumed);
            trackingInfo1.DataBuffer.ReleaseLocalData -= new EventHandler(this.OnReleaseLocalData);
            trackingInfo1.DataBuffer.Dispose();
            trackingInfo1.DataBuffer = null;
            trackingInfo1.CleanupHandler = null;
            trackingInfo1.CleanupHandlerData = null;
            if (this.m_alTracking.Count != 0)
                return;
            this.OnTrackerEmpty(this, EventArgs.Empty);
        }

        private void OnDataConsumed(object oSender, EventArgs args) => this.Release(oSender as DataBuffer, Users.Consumed);

        private void OnReleaseLocalData(object oSender, EventArgs args) => this.Release(oSender as DataBuffer, Users.LocalData);

        internal void DEBUG_SetDescription(string stDescription)
        {
        }

        internal void DEBUG_DumpTrackingList()
        {
        }

        [System.Flags]
        internal enum Users : uint
        {
            Consumed = 1,
            LocalData = 2,
            Valid = LocalData | Consumed, // 0x00000003
        }

        internal class CleanupEventArgs : EventArgs
        {
            public object ContextData;

            internal CleanupEventArgs(object contextData) => this.ContextData = contextData;
        }

        internal delegate void CleanupEventHandler(
          object sender,
          DataBufferTracker.CleanupEventArgs args);

        private class TrackingInfo
        {
            private static uint s_idSeed;
            internal DataBuffer DataBuffer;
            internal DataBufferTracker.Users UserFlags;
            internal uint Id;
            internal DataBufferTracker.CleanupEventHandler CleanupHandler;
            internal object CleanupHandlerData;

            internal TrackingInfo(
              DataBuffer dataBuffer,
              DataBufferTracker.Users userFlags,
              DataBufferTracker.CleanupEventHandler handler,
              object contextData)
            {
                this.DataBuffer = dataBuffer;
                this.UserFlags = userFlags;
                this.Id = s_idSeed++;
                this.CleanupHandler = handler;
                this.CleanupHandlerData = contextData;
            }
        }
    }
}
