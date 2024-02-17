namespace Magicube.Data.Abstractions.SqlBuilder.Models {
    public class TableSchemaModel {
        public string DataType      { get; set; }
        public string TableName     { get; set; }
        public string ColumnName    { get; set; }
        public bool   AutoIncrement { get; set; }
        public string DefaultValue  { get; set; }
        public bool   IsPrimaryKey  { get; set; }
        public bool   UniqueKey     { get; set; }
        public bool   Nullable      { get; set; }
        public int    Length        { get; set; }
    }
}
