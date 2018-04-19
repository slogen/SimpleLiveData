using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Scm.Sys
{
    /// <summary>
    /// Support for RFC4122 (https://www.ietf.org/rfc/rfc4122.txt) UUID
    /// </summary>
    public static class Uuid
    {
        private static readonly byte[] OrderFlip =
        {
            3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15
        };

        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        public static Guid Dns { get; } = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
        public static Guid Default() => Guid.NewGuid();

        private static byte[] EnsureGuidMsb(byte[] bytes)
        {
            return BitConverter.IsLittleEndian
                ? Enumerable.Range(0, 16).Select(i => bytes[OrderFlip[i]]).ToArray()
                : bytes;
        }

        private static byte[] Hash(Guid nameSpace, byte[] bytes, int index, int length, HashAlgorithm algorithm)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (length > bytes.Length - index)
                throw new ArgumentException("Not enough bytes");
            // quote rfc4122.txt:
            /* o Compute the hash of the name space ID concatenated with the name. */
            algorithm.Initialize();
            using (var cs = new CryptoStream(Stream.Null, algorithm, CryptoStreamMode.Write))
            {
                /* put the namespace ID in network byte order. */
                var namespaceBytes = nameSpace.ToByteArrayRfc4122Msb();
                cs.Write(namespaceBytes, 0, namespaceBytes.Length);
                cs.Write(bytes, index, length);
            }

            return algorithm.Hash;
        }

        public static Guid NamespaceFromHash(byte[] hash, ushort version)
        {
            if ((version & 0x0F) != version)
                throw new ArgumentException($"{nameof(version)} limited to 4 bits");
            if (!BitConverter.IsLittleEndian)
                throw new NotSupportedException("Implementation below is only valid on little endian");
            /* Quote RFC
             
Set octets zero through 3 of the time_low field to octets zero through 3 of the hash.

   o  Set octets zero and one of the time_mid field to octets 4 and 5 of
      the hash.

   o  Set octets zero and one of the time_hi_and_version field to octets
      6 and 7 of the hash.

   o  Set the four most significant bits (bits 12 through 15) of the
      time_hi_and_version field to the appropriate 4-bit version number
      from Section 4.1.3.

   o  Set the clock_seq_hi_and_reserved field to octet 8 of the hash.

   o  Set the two most significant bits (bits 6 and 7) of the
      clock_seq_hi_and_reserved to zero and one, respectively.

   o  Set the clock_seq_low field to octet 9 of the hash.

   o  Set octets zero through five of the node field to octets 10
      through 15 of the hash.

   o  Convert the resulting UUID to local byte order. */
            var guidBytes = EnsureGuidMsb(hash);
            guidBytes[7] = (byte) ((guidBytes[7] & 0x0F) | ((version << 4) & 0xF0));
            return new Guid(guidBytes);
        }

        public static Guid Namespace(this Guid nameSpace, byte[] bytes, int index, int length, ushort version,
            HashAlgorithm algorithm)
        {
            return NamespaceFromHash(Hash(nameSpace, bytes, index, length, algorithm), version);
        }

        private static HashAlgorithm HashByVersion(ushort version)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases -- throws if not handled
            switch (version)
            {
                case 3:
                    return MD5.Create();
                case 5:
                    return SHA1.Create();
            }

            throw new NotSupportedException($"Version {version} not supported");
        }

        public static Guid Namespace(this Guid nameSpace, string name, ushort version = 5, Encoding encoding = null)
        {
            /*   o  Choose either MD5 [4] or SHA-1 [8] as the hash algorithm; If backward compatibility is not an issue, SHA-1 is preferred. */
            using (var hash = HashByVersion(version))
            {
                /* o  Convert the name to a canonical sequence of octets (as defined by the standards or conventions of its name space); */
                var bytes = (encoding ?? DefaultEncoding).GetBytes(name);
                // Proceed to calculate
                return Namespace(nameSpace, bytes, 0, bytes.Length, version, hash);
            }
        }

        public static byte[] ToByteArrayRfc4122Msb(this Guid guid)
        {
            if (!BitConverter.IsLittleEndian)
                throw new NotSupportedException("Implementation below is only valid on little endian");
            var b = guid.ToByteArray();
            return new[]
            {
                b[3], b[2], b[1], b[0], b[5], b[4], b[7], b[6], b[8], b[9], b[10], b[11], b[12], b[13], b[14], b[15]
            };
        }
    }
}