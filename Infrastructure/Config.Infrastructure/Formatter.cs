using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Config.Infrastructure
                
{
    public static class Formatter
    {
        static BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public static byte[] ToByteArray(object obj)
        {
            if (obj == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                _binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using (var ms = new MemoryStream(data))
            {
                return (T)_binaryFormatter.Deserialize(ms);
            }
        }
    }
}
