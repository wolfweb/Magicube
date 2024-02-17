using Magicube.Core.Models;
using System;
using System.ComponentModel;

namespace Magicube.Data.Abstractions {
	public enum DbPrimaryKeyType {
        [Description("System.Guid")]
        Guid,

        [Description("System.Int32")]
        Int32,

        [Description("System.Int64")]
        Int64,

        [Description("System.String")]
        String
    }

    public interface IDbTable {
		string       Name        { get; set; }
		string       Title       { get; set; }
		string       Description { get; set; }
		long         CreateAt    { get; set; }
		long?        UpdateAt    { get; set; }
		DbField[]    Fields      { get; set; }
		EntityStatus Status      { get; set; }
	}

    public class DbField {
        /// <summary>
        /// 验证字段值有效性
        /// </summary>
        public DbFieldValidator[]   ValidatorProviders{ get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool                 AutoIncrement     { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string               DefaultValue      { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool                 PrimaryKey        { get; set; }

        /// <summary>
        /// 是为唯一
        /// </summary>
        public bool                 UniqueKey         { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool                 Nullable          { get; set; } = true;

        /// <summary>
        /// 绑定类型
        /// </summary>
        public Type                 BindType          { get; set; }

        /// <summary>
        /// 指定数据类型
        /// </summary>
        public string               DbType            { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string               Name              { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int                  Size              { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string               Desc              { get; set; }
        
        [Description("是否可以作为查询条件")]
        public bool                 IsFilter          { get; set; }
        
        [Description("是否可以作为排序")]
        public bool                 IsSort            { get; set; }
    }
}
