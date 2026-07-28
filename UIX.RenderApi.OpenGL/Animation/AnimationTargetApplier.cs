using System;
using Microsoft.Iris.Render.OpenGL.Scene;

namespace Microsoft.Iris.Render.OpenGL.Animation
{
    /// <summary>
    /// Writes a resolved <see cref="AnimValue"/> onto a target render object's named
    /// animatable property, honoring an optional channel mask. Property names/types match
    /// the original render objects (Position/Size/Scale/Alpha/Rotation/… on visuals,
    /// CameraEye/At/Up/Zn on cameras, Offset/ColorMask on gradients, dynamic on effects).
    /// </summary>
    internal static class AnimationTargetApplier
    {
        public static void Apply(IAnimatable target, string property, string? mask, AnimValue value)
        {
            // Effects use dynamic (custom) properties with no readable current value, so
            // they only support a full write (masks are not applied to them).
            if (target is GLEffect effect)
            {
                ApplyToEffect(effect, property, value);
                return;
            }

            AnimationTypeMask channelMask = AnimationTypeMask.FromString(mask);
            float[] channels;
            if (channelMask.ChannelCount == 0)
            {
                channels = new[] { value.X, value.Y, value.Z, value.W };
            }
            else
            {
                if (!TryGetChannels(target, property, out channels))
                    return;
                for (int i = 0; i < channelMask.ChannelCount; i++)
                {
                    AnimationTypeChannel ch = channelMask[i];
                    if (ch != AnimationTypeChannel.O)
                        channels[(int)ch - 1] = value.GetChannel(i);
                }
            }

            SetChannels(target, property, channels);
        }

        private static void ApplyToEffect(GLEffect effect, string property, AnimValue v)
        {
            switch (v.Type)
            {
                case AnimationInputType.Float: effect.SetProperty(property, v.AsFloat()); break;
                case AnimationInputType.Vector2: effect.SetProperty(property, v.AsVector2()); break;
                case AnimationInputType.Vector3: effect.SetProperty(property, v.AsVector3()); break;
                default: effect.SetProperty(property, v.AsVector4()); break;
            }
        }

        private static bool TryGetChannels(IAnimatable target, string property, out float[] channels)
        {
            channels = new float[4];
            switch (target)
            {
                case GLVisual visual:
                    switch (property)
                    {
                        case "Position": Store(channels, visual.Position); return true;
                        case "Size": channels[0] = visual.Size.X; channels[1] = visual.Size.Y; return true;
                        case "Scale": Store(channels, visual.Scale); return true;
                        case "Alpha": channels[0] = visual.Alpha; return true;
                        case "CenterPoint": Store(channels, visual.CenterPoint); return true;
                        case "Rotation":
                            Store(channels, visual.Rotation.Axis);
                            channels[3] = visual.Rotation.Angle;
                            return true;
                        default: return false;
                    }
                case GLCamera camera:
                    switch (property)
                    {
                        case "CameraEye": Store(channels, camera.Eye); return true;
                        case "CameraAt": Store(channels, camera.At); return true;
                        case "CameraUp": Store(channels, camera.Up); return true;
                        case "CameraZn": channels[0] = camera.Zn; return true;
                        default: return false;
                    }
                default:
                    return false;
            }
        }

        private static void SetChannels(IAnimatable target, string property, float[] c)
        {
            switch (target)
            {
                case GLVisual visual:
                    switch (property)
                    {
                        case "Position": visual.Position = new Vector3(c[0], c[1], c[2]); break;
                        case "Size": visual.Size = new Vector2(c[0], c[1]); break;
                        case "Scale": visual.Scale = new Vector3(c[0], c[1], c[2]); break;
                        case "Alpha": visual.Alpha = c[0]; break;
                        case "CenterPoint": visual.CenterPoint = new Vector3(c[0], c[1], c[2]); break;
                        case "Rotation": visual.Rotation = new AxisAngle(new Vector3(c[0], c[1], c[2]), c[3]); break;
                        case "Orientation": visual.Rotation = QuaternionToAxisAngle(c[0], c[1], c[2], c[3]); break;
                    }
                    break;
                case GLCamera camera:
                    switch (property)
                    {
                        case "CameraEye": camera.Eye = new Vector3(c[0], c[1], c[2]); break;
                        case "CameraAt": camera.At = new Vector3(c[0], c[1], c[2]); break;
                        case "CameraUp": camera.Up = new Vector3(c[0], c[1], c[2]); break;
                        case "CameraZn": camera.Zn = c[0]; break;
                    }
                    break;
            }
        }

        private static void Store(float[] channels, Vector3 v)
        {
            channels[0] = v.X;
            channels[1] = v.Y;
            channels[2] = v.Z;
        }

        private static AxisAngle QuaternionToAxisAngle(float x, float y, float z, float w)
        {
            w = w < -1f ? -1f : (w > 1f ? 1f : w);
            float angle = 2f * (float)Math.Acos(w);
            float s = (float)Math.Sqrt(Math.Max(0f, 1f - w * w));
            Vector3 axis = s < 1e-4f ? new Vector3(0f, 0f, 1f) : new Vector3(x / s, y / s, z / s);
            return new AxisAngle(axis, angle);
        }
    }
}
