using System.Collections.Generic;

namespace ChargebackSdkForNet
{
    public class ResponseContent
    {
        private readonly string _contentType;

        private readonly List<byte> _byteData;

        public ResponseContent(string contentType, List<byte> byteData)
        {
            _contentType = contentType;
            _byteData = byteData;
        }

        public string GetContentType()
        {
            return _contentType;
        }

        public List<byte> GetByteData()
        {
            return _byteData;
        }
    }
}