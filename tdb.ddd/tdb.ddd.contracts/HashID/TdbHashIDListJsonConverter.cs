using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace tdb.ddd.contracts
{
    /// <summary>
    /// 把ID列表hash加密的json转换器
    /// </summary>
    public class TdbHashIDListJsonConverter : JsonConverter<List<long>>
    {
        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override List<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var lstStr = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                if (lstStr is null)
                {
                    return new List<long>();
                }

                var lstID = new List<long>();
                foreach (var str in lstStr)
                {
                    if (string.IsNullOrWhiteSpace(str) == false)
                    {
                        var id = TdbHashID.DecodeSingleLong(str);
                        lstID.Add(id);
                    }
                }
                return lstID;
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
        public override void Write(Utf8JsonWriter writer, List<long> value, JsonSerializerOptions options)
        {
            var lstStr = new List<string>();
            foreach (var id in value)
            {
                lstStr.Add(TdbHashID.EncodeLong(id));
            }

            JsonSerializer.Serialize(writer, lstStr, options);
        }
    }
}
