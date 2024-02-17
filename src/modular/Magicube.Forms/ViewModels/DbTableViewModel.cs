using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Forms.ViewModels {
    public class DbTableViewModel : EntityViewModel<DbTable, int> {
        public DbTableViewModel(DbTable dbTable = null) : base(dbTable) {
        }

        [DataItems]
        public EntityStatus Status   { get; set; }

        [Display(Name = "创建时间")]
        [DataType(DataType.DateTime)]
        public ValueObject  CreateAt { get; set; }
    }

    public class DbTableSchemaViewModel {
        public int                          Id         { get; set; }
        public DbTableFormSchemaViewModel[] FormSchema { get; set; }
    }

    public class DbTableFormSchemaViewModel {
        public string                      Id         { get; set; }
        public string                      Key        { get; set; }
        public string                      Display    { get; set; }
                                           
        public JObject                     Attributes { get; set; }
        public DbFieldViewModel            DataBinder { get; set; }
        public DbFieldValidatorViewModel[] Validation { get; set; }
    }

    public class DbFieldViewModel {
        [Display(Name = "自增")]
        public bool             AutoIncrement { get; set; }
        [Display(Name = "主键")]
        public bool             PrimaryKey    { get; set; }
        [Display(Name = "唯一键")]
        public bool             UniqueKey     { get; set; }
        [Display(Name = "可空")]
        public bool             Nullable      { get; set; } = true;
        
        [EnumDataType(typeof(DbPrimaryKeyType))]
        [Display(Name = "指定类型")]
        public Type             BindType      { get; set; }

        [Required, Display(Name = "字段名")]
        public string           Name          { get; set; }
        [Required, Display(Name = "字段长度")]
        public int              Size          { get; set; }
        [Display(Name = "可过滤")]
        public bool             IsFilter      { get; set; }        
        [Display(Name = "可排序")]
        public bool             IsSort        { get; set; }
    }

    public class DbFieldValidatorViewModel : DbFieldValidator {
        public JObject Rule { get; set; }
    }
}
