using System;
using System.Globalization;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Variables
{
    public partial class UVTHelper
    {
        public static long EncodeFloat(float v) => unchecked(BitConverter.SingleToInt32Bits(v));

        public static float DecodeFloat(long v) =>
            BitConverter.Int32BitsToSingle(unchecked((int)v));

        public static long EncodeInt(int v) => v;

        public static int DecodeInt(long v) => unchecked((int)v);

        public static long EncodeBool(bool v) => v ? 1L : 0L;

        public static bool DecodeBool(long v) => v != 0;

        public static long EncodeInt2(int2 v) => ((long)(uint)v.x << 32) | (uint)v.y;

        public static int2 DecodeInt2(long v) => new(unchecked((int)(v >> 32)), unchecked((int)v));

        public static long EncodeUlong(ulong value) => unchecked((long)value);

        public static ulong DecodeUlong(long value) => unchecked((ulong)value);

        public static long ConvertToLong(bool value) => EncodeBool(value);

        public static long ConvertToLong(float value) => EncodeFloat(value);

        public static long ConvertToLong(int value) => EncodeInt(value);

        public static long ConvertToLong(int2 value) => EncodeInt2(value);

        public static long ConvertToLong(short value) => EncodeInt(value);

        public static long ConvertToLong(sbyte value) => EncodeInt(value);

        public static long ConvertToLong(byte value) => EncodeInt(value);

        public static long ConvertToLong(ulong value) => EncodeUlong(value);

        public static bool ConvertToBool(long value) => DecodeBool(value);

        public static float ConvertToFloat(long value) => DecodeFloat(value);

        public static int ConvertToInt(long value) => DecodeInt(value);

        public static int2 ConvertToInt2(long value) => DecodeInt2(value);

        public static short ConvertToShort(long value) => (short)DecodeInt(value);

        public static sbyte ConvertToSByte(long value) => (sbyte)DecodeInt(value);

        public static byte ConvertToByte(long value) => (byte)DecodeInt(value);

        public static ulong ConvertToUlong(long value) => DecodeUlong(value);

        public static bool TryConvertValue(
            string value,
            UpdateValueType updateValueType,
            out long encoded
        )
        {
            encoded = 0;

            if (value.StartsWith("(long)"))
            {
                value = value.Replace("(long)", "");
                if (long.TryParse(value, out encoded))
                    return true;
            }

            ValueFormat format = GetValueFormat(updateValueType);

            switch (format)
            {
                case ValueFormat.Bool:
                {
                    if (byte.TryParse(value, out byte b))
                    {
                        encoded = EncodeBool(b != 0);
                        return true;
                    }

                    if (bool.TryParse(value, out bool bb))
                    {
                        encoded = EncodeBool(bb);
                        return true;
                    }

                    return false;
                }
                case ValueFormat.Float:
                {
                    if (
                        !float.TryParse(
                            value,
                            NumberStyles.Float | NumberStyles.AllowThousands,
                            CultureInfo.InvariantCulture,
                            out float f
                        )
                    )
                        return false;

                    if (float.IsNaN(f) || float.IsInfinity(f))
                        return false;

                    TryLimit(updateValueType, ref f);

                    encoded = EncodeFloat(f);
                    return true;
                }

                case ValueFormat.Int:
                {
                    if (
                        !int.TryParse(
                            value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out int i
                        )
                    )
                        return false;

                    TryLimit(updateValueType, ref i);

                    encoded = EncodeInt(i);
                    return true;
                }

                case ValueFormat.Int2:
                {
                    var parts = value.Split(',');
                    if (parts.Length != 2)
                        return false;

                    if (!int.TryParse(parts[0], out int x))
                        return false;

                    if (!int.TryParse(parts[1], out int y))
                        return false;

                    encoded = EncodeInt2(new int2(x, y));
                    return true;
                }
                case ValueFormat.Short:
                {
                    if (
                        !short.TryParse(
                            value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out short s
                        )
                    )
                        return false;

                    TryLimit(updateValueType, ref s);

                    encoded = EncodeInt(s);
                    return true;
                }

                case ValueFormat.SByte:
                {
                    if (
                        !sbyte.TryParse(
                            value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out sbyte sb
                        )
                    )
                        return false;

                    TryLimit(updateValueType, ref sb);

                    encoded = EncodeInt(sb);
                    return true;
                }

                case ValueFormat.Byte:
                {
                    if (
                        !byte.TryParse(
                            value,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out byte b
                        )
                    )
                        return false;

                    TryLimit(updateValueType, ref b);

                    encoded = EncodeInt(b);
                    return true;
                }
            }

            return false;
        }

        public static bool TryConvertValueToString(
            long encoded,
            UpdateValueType updateValueType,
            out string value
        )
        {
            value = string.Empty;

            ValueFormat format = GetValueFormat(updateValueType);

            switch (format)
            {
                case ValueFormat.Bool:
                {
                    bool b = DecodeBool(encoded);
                    value = b ? "true" : "false";
                    return true;
                }

                case ValueFormat.Float:
                {
                    float f = DecodeFloat(encoded);

                    if (float.IsNaN(f) || float.IsInfinity(f))
                        return false;

                    value = f.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                case ValueFormat.Int:
                {
                    int i = DecodeInt(encoded);
                    value = i.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                case ValueFormat.Int2:
                {
                    int2 v = DecodeInt2(encoded);
                    value = $"{v.x},{v.y}";
                    return true;
                }

                case ValueFormat.Short:
                {
                    short s = (short)DecodeInt(encoded);
                    value = s.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                case ValueFormat.SByte:
                {
                    sbyte sb = (sbyte)DecodeInt(encoded);
                    value = sb.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                case ValueFormat.Byte:
                {
                    byte b = (byte)DecodeInt(encoded);
                    value = b.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
            }

            return false;
        }
    }
}
