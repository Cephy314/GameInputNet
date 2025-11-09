using System;
using System.Runtime.InteropServices;

namespace GameInputDotNet.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AppLocalDeviceId : IEquatable<AppLocalDeviceId>
{
    private const string HexUpper = "0123456789ABCDEF";
    private const string HexLower = "0123456789abcdef";

    private fixed byte _value[Constants.APP_LOCAL_DEVICE_ID_SIZE];

    public static AppLocalDeviceId FromBytes(ReadOnlySpan<byte> source)
    {
        AppLocalDeviceId id = default;
        id.Set(source);
        return id;
    }

    public static AppLocalDeviceId Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Parse(value.AsSpan());
    }

    public static AppLocalDeviceId Parse(ReadOnlySpan<char> value)
    {
        if (!TryParse(value, out var id))
        {
            throw new FormatException(
                $"AppLocalDeviceId must be provided as {Constants.APP_LOCAL_DEVICE_ID_SIZE * 2} hexadecimal characters.");
        }

        return id;
    }

    public static bool TryParse(string? value, out AppLocalDeviceId id)
    {
        return value is not null && TryParse(value.AsSpan(), out id);
    }

    public static bool TryParse(ReadOnlySpan<char> value, out AppLocalDeviceId id)
    {
        Span<byte> buffer = stackalloc byte[Constants.APP_LOCAL_DEVICE_ID_SIZE];
        if (!TryParseInternal(value, buffer))
        {
            id = default;
            return false;
        }

        id = FromBytes(buffer);
        return true;
    }

    public ReadOnlySpan<byte> AsSpan()
    {
        fixed (byte* ptr = _value)
        {
            return new ReadOnlySpan<byte>(ptr, Constants.APP_LOCAL_DEVICE_ID_SIZE);
        }
    }

    public void CopyTo(Span<byte> destination)
    {
        if (destination.Length < Constants.APP_LOCAL_DEVICE_ID_SIZE)
            throw new ArgumentException(
                $"Destination must be at least {Constants.APP_LOCAL_DEVICE_ID_SIZE} bytes", nameof(destination));

        AsSpan().CopyTo(destination);
    }

    public void Set(ReadOnlySpan<byte> source)
    {
        if (source.Length != Constants.APP_LOCAL_DEVICE_ID_SIZE)
            throw new ArgumentException($"Source must be exactly {Constants.APP_LOCAL_DEVICE_ID_SIZE} bytes",
                nameof(source));

        fixed (byte* ptr = _value)
        {
            source.CopyTo(new Span<byte>(ptr, Constants.APP_LOCAL_DEVICE_ID_SIZE));
        }
    }

    public bool Equals(AppLocalDeviceId other)
    {
        return AsSpan().SequenceEqual(other.AsSpan());
    }

    public override bool Equals(object? obj)
    {
        return obj is AppLocalDeviceId other && Equals(other);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.AddBytes(AsSpan());
        return hash.ToHashCode();
    }

    public static bool operator ==(AppLocalDeviceId left, AppLocalDeviceId right) => left.Equals(right);

    public static bool operator !=(AppLocalDeviceId left, AppLocalDeviceId right) => !left.Equals(right);

    public bool TryFormat(Span<char> destination, out int charsWritten, bool lowerCase = false)
    {
        const int requiredLength = Constants.APP_LOCAL_DEVICE_ID_SIZE * 2;
        if (destination.Length < requiredLength)
        {
            charsWritten = 0;
            return false;
        }

        WriteHex(destination[..requiredLength], this, lowerCase);
        charsWritten = requiredLength;
        return true;
    }

    public string ToHexString(bool lowerCase = false)
    {
        return string.Create(Constants.APP_LOCAL_DEVICE_ID_SIZE * 2, (Value: this, lowerCase),
            static (span, state) => WriteHex(span, state.Value, state.lowerCase));
    }

    public override string ToString()
    {
        return ToHexString();
    }

    public bool IsEmpty
    {
        get
        {
            foreach (var b in AsSpan())
            {
                if (b != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    private static bool TryParseInternal(ReadOnlySpan<char> value, Span<byte> destination)
    {
        int nibble = -1;
        int byteIndex = 0;

        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c) || c == '-' || c == '_')
            {
                continue;
            }

            var parsed = ParseHexNibble(c);
            if (parsed < 0)
            {
                return false;
            }

            if (nibble < 0)
            {
                nibble = parsed;
                continue;
            }

            if (byteIndex >= destination.Length)
            {
                return false;
            }

            destination[byteIndex++] = (byte)((nibble << 4) | parsed);
            nibble = -1;
        }

        if (nibble != -1 || byteIndex != destination.Length)
        {
            return false;
        }

        return true;
    }

    private static int ParseHexNibble(char c)
    {
        if (c is >= '0' and <= '9') return c - '0';
        if (c is >= 'a' and <= 'f') return c - 'a' + 10;
        if (c is >= 'A' and <= 'F') return c - 'A' + 10;
        return -1;
    }

    private static void WriteHex(Span<char> destination, AppLocalDeviceId value, bool lowerCase)
    {
        ReadOnlySpan<char> alphabet = lowerCase ? HexLower.AsSpan() : HexUpper.AsSpan();
        ReadOnlySpan<byte> bytes = value.AsSpan();

        for (int i = 0; i < bytes.Length; i++)
        {
            var current = bytes[i];
            destination[(i * 2)] = alphabet[current >> 4];
            destination[(i * 2) + 1] = alphabet[current & 0xF];
        }
    }
}
