using System.Text;

namespace luacsharp.util
{
    public static class ConvertUtil
    {
        public static string Bytes2String(byte[] bytes)
        {
            return Encoding.UTF7.GetString(bytes);
        }
    }
}