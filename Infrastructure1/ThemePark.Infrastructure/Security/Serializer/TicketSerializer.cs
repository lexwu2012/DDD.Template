using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;

namespace ThemePark.Infrastructure.Security.Serializer
{
    /// <summary>
    /// 实现 <see cref="AuthenticationTicket"/> 数据序列化
    /// </summary>
    public class TicketSerializer : IDataSerializer<AuthenticationTicket>
    {
        private const int FormatVersion = 2;

        public virtual byte[] Serialize(AuthenticationTicket model)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    using (var writer = new BinaryWriter(gzipStream))
                        Write(writer, model);
                }
                return memoryStream.ToArray();
            }
        }

        public virtual AuthenticationTicket Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(gzipStream))
                        return Read(reader);
                }
            }
        }

        public static void Write(BinaryWriter writer, AuthenticationTicket model)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            writer.Write(FormatVersion);

            var identity = model.Identity;
            writer.Write(identity.AuthenticationType);
            WriteWithDefault(writer, identity.NameClaimType, DefaultValues.NameClaimType);
            WriteWithDefault(writer, identity.RoleClaimType, DefaultValues.RoleClaimType);

            writer.Write(identity.Claims.Count());
            foreach (Claim claim in identity.Claims)
            {
                WriteWithDefault(writer, claim.Type, identity.NameClaimType);
                writer.Write(claim.Value);
                WriteWithDefault(writer, claim.ValueType, DefaultValues.StringValueType);
                WriteWithDefault(writer, claim.Issuer, DefaultValues.LocalAuthority);
                WriteWithDefault(writer, claim.OriginalIssuer, claim.Issuer);
            }

            PropertiesSerializer.Write(writer, model.Properties);
        }

        public static AuthenticationTicket Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (reader.ReadInt32() != FormatVersion)
                return null;

            var authenticationType = reader.ReadString();
            var nameType = ReadWithDefault(reader, DefaultValues.NameClaimType);
            var roleType = ReadWithDefault(reader, DefaultValues.RoleClaimType);
            var length = reader.ReadInt32();
            var claimArray = new Claim[length];
            for (var index = 0; index != length; ++index)
            {
                var type = ReadWithDefault(reader, nameType);
                var value = reader.ReadString();
                var valueType = ReadWithDefault(reader, DefaultValues.StringValueType);
                var issuer = ReadWithDefault(reader, DefaultValues.LocalAuthority);
                var originalIssuer = ReadWithDefault(reader, issuer);
                claimArray[index] = new Claim(type, value, valueType, issuer, originalIssuer);
            }

            var identity = new ClaimsIdentity(claimArray, authenticationType, nameType, roleType);
            var properties = PropertiesSerializer.Read(reader);
            return new AuthenticationTicket(identity, properties);
        }

        private static void WriteWithDefault(BinaryWriter writer, string value, string defaultValue)
        {
            if (string.Equals(value, defaultValue, StringComparison.Ordinal))
                writer.Write(DefaultValues.DefaultStringPlaceholder);
            else
                writer.Write(value);
        }

        private static string ReadWithDefault(BinaryReader reader, string defaultValue)
        {
            var a = reader.ReadString();
            if (string.Equals(a, DefaultValues.DefaultStringPlaceholder, StringComparison.Ordinal))
                return defaultValue;
            return a;
        }

        private static class DefaultValues
        {
            public const string DefaultStringPlaceholder = "\0";
            public const string NameClaimType = ClaimTypes.Name;
            public const string RoleClaimType = ClaimTypes.Role;
            public const string LocalAuthority = "LOCAL AUTHORITY";
            public const string StringValueType = ClaimValueTypes.String;
        }
    }
}