// *****************************************************************************************************************
// Project          : NavyBlue
// File             : PBKDF2.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:52
// *****************************************************************************************************************
// <copyright file="PBKDF2.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary
{
    // Come from FluentSharp
    // Code based on example from: Password Minder Internals http://msdn.microsoft.com/en-us/magazine/cc163913.aspx
    // implementation of PKCS#5 v2.0
    // Password Based Key Derivation Function 2
    // http://www.rsasecurity.com/rsalabs/pkcs/pkcs-5/index.html
    // For the HMAC function, see RFC 2104
    // http://www.ietf.org/rfc/rfc2104.txt

    /// <summary>
    ///     Class PBKDF2Utility.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PBKDF")]
    public static class PBKDF2Utility
    {
        // SHA-256 has a 512-bit block size and gives a 256-bit output
        /// <summary>
        ///     The block size in bytes
        /// </summary>
        private const int BLOCK_SIZE_IN_BYTES = 64;

        /// <summary>
        ///     The default PBKD f2 bytes
        /// </summary>
        private const int DEFAULT_PBKDF2_BYTES = 64;

        /// <summary>
        ///     The default PBKD f2 interactions
        /// </summary>
        private const int DEFAULT_PBKDF2_INTERACTIONS = 100; // 20000;

        /// <summary>
        ///     The hash size in bytes
        /// </summary>
        private const int HASH_SIZE_IN_BYTES = 32;

        /// <summary>
        ///     The ipad
        /// </summary>
        private const byte IPAD = 0x36;

        /// <summary>
        ///     The opad
        /// </summary>
        private const byte OPAD = 0x5C;

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <param name="howManyBytes">The how many bytes.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytes(string payload, byte[] salt, int iterations, int howManyBytes)
        {
            return GetBytes(
                Encoding.UTF8.GetBytes(payload),
                salt, iterations, howManyBytes);
        }

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <param name="howManyBytes">The how many bytes.</param>
        /// <returns>System.Byte[].</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static byte[] GetBytes(byte[] payload, byte[] salt, int iterations, int howManyBytes)
        {
            // round up

            uint cBlocks = (uint)((howManyBytes + HASH_SIZE_IN_BYTES - 1) / HASH_SIZE_IN_BYTES);

            // seed for the pseudo-random fcn: salt + block index
            byte[] saltAndIndex = new byte[salt.Length + 4];
            Array.Copy(salt, 0, saltAndIndex, 0, salt.Length);

            byte[] output = new byte[cBlocks * HASH_SIZE_IN_BYTES];
            int outputOffset = 0;

            SHA256Managed innerHash = new SHA256Managed();
            SHA256Managed outerHash = new SHA256Managed();

            // HMAC says the key must be hashed or padded with zeros
            // so it fits into a single block of the hash in use
            if (payload.Length > BLOCK_SIZE_IN_BYTES)
            {
                payload = innerHash.ComputeHash(payload);
            }

            byte[] key = new byte[BLOCK_SIZE_IN_BYTES];
            Array.Copy(payload, 0, key, 0, payload.Length);

            byte[] innerKey = new byte[BLOCK_SIZE_IN_BYTES];
            byte[] outerKey = new byte[BLOCK_SIZE_IN_BYTES];
            for (int i = 0; i < BLOCK_SIZE_IN_BYTES; ++i)
            {
                innerKey[i] = (byte)(key[i] ^ IPAD);
                outerKey[i] = (byte)(key[i] ^ OPAD);
            }

            // for each block of desired output
            for (int iBlock = 0; iBlock < cBlocks; ++iBlock)
            {
                // seed HMAC with salt & block index
                IncrementBigEndianIndex(saltAndIndex, salt.Length);
                byte[] u = saltAndIndex;

                for (int i = 0; i < iterations; ++i)
                {
                    // simple implementation of HMAC-SHA-256
                    innerHash.Initialize();
                    innerHash.TransformBlock(innerKey, 0,
                        BLOCK_SIZE_IN_BYTES, innerKey, 0);
                    innerHash.TransformFinalBlock(u, 0, u.Length);

                    byte[] temp = innerHash.Hash;

                    outerHash.Initialize();
                    outerHash.TransformBlock(outerKey, 0,
                        BLOCK_SIZE_IN_BYTES, outerKey, 0);
                    outerHash.TransformFinalBlock(temp, 0, temp.Length);

                    u = outerHash.Hash; // U = result of HMAC

                    // xor result into output buffer
                    XorByteArray(u, 0, HASH_SIZE_IN_BYTES,
                        output, outputOffset);
                }

                outputOffset += HASH_SIZE_IN_BYTES;
            }

            byte[] result = new byte[howManyBytes];
            Array.Copy(output, 0, result, 0, howManyBytes);
            return result;
        }

        /// <summary>
        ///     Hashes the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>System.String.</returns>
        public static string Hash(string payload, string salt)
        {
            byte[] bytes = GetBytes(payload.GetBytesOfUTF8(), salt.GetBytesOfUTF8(), DEFAULT_PBKDF2_INTERACTIONS, DEFAULT_PBKDF2_BYTES);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     Increments the index of the big endian.
        /// </summary>
        /// <param name="buf">The buf.</param>
        /// <param name="offset">The offset.</param>
        /// <exception cref="System.OverflowException"></exception>
        private static void IncrementBigEndianIndex(IList<byte> buf, int offset)
        {
            // treat the four bytes starting at buf[offset]
            // as a big endian integer, and increment it
            unchecked
            {
                if (0 == ++buf[offset + 3])
                    if (0 == ++buf[offset + 2])
                        if (0 == ++buf[offset + 1])
                            if (0 == ++buf[offset + 0])
                                throw new OverflowException();
            }
        }

        /// <summary>
        ///     Xors the byte array.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="srcOffset">The source offset.</param>
        /// <param name="cb">The cb.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="destOffset">The dest offset.</param>
        private static void XorByteArray(IReadOnlyList<byte> src, int srcOffset, int cb, IList<byte> dest, int destOffset)
        {
            int end = checked(srcOffset + cb);
            while (srcOffset != end)
            {
                dest[destOffset] ^= src[srcOffset];
                ++srcOffset;
                ++destOffset;
            }
        }
    }
}