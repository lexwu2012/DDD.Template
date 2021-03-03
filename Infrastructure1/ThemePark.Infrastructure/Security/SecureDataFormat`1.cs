using ThemePark.Infrastructure.Security.DataProtection;
using ThemePark.Infrastructure.Security.Serializer;
using ThemePark.Infrastructure.Security.Serializer.Encoder;

namespace ThemePark.Infrastructure.Security
{
    public class SecureDataFormat<TData> : ISecureDataFormat<TData>
    {
        private readonly IDataSerializer<TData> _serializer;
        private readonly IDataProtector _protector;
        private readonly ITextEncoder _encoder;

        public SecureDataFormat(IDataSerializer<TData> serializer, IDataProtector protector, ITextEncoder encoder)
        {
            this._serializer = serializer;
            this._protector = protector;
            this._encoder = encoder;
        }

        public string Protect(TData data)
        {
            return this._encoder.Encode(this._protector.Protect(this._serializer.Serialize(data)));
        }

        public TData Unprotect(string protectedText)
        {
            try
            {
                if (protectedText == null)
                    return default(TData);
                byte[] protectedData = this._encoder.Decode(protectedText);
                if (protectedData == null)
                    return default(TData);
                byte[] data = this._protector.Unprotect(protectedData);
                if (data == null)
                    return default(TData);
                return this._serializer.Deserialize(data);
            }
            catch
            {
                return default(TData);
            }
        }
    }
}
