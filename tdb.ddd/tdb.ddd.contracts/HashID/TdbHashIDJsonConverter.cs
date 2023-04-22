using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using tdb.ddd.contracts;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 把 ID hash加密的json转换器
    /// </summary>
    public class TdbHashIDJsonConverter : JsonConverter<long>
    {
        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var str = JsonSerializer.Deserialize<string>(ref reader, options);
                return TdbHashID.DecodeSingleLong(str ?? "");
            }
            catch (Exception ex)
            {
                throw new TdbException("传入的ID不对", ex);
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, TdbHashID.EncodeLong(value), options);
        }
    }
}
