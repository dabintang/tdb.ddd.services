using SqlSugar;

namespace tdb.demo.webapi.DB
{
    /// <summary>
    /// 表table1的实体
    /// </summary>
    [SugarTable("table1")]
    public class Table1
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int ID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; } = "";
    }
}
