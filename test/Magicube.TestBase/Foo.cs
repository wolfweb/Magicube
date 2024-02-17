using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.TestBase {
    public interface IFoo {
        string Name { get; set; }
    }

    public abstract class AbstractFoo : IFoo {
        public abstract string Name { get; set; }
    }

    public class Foo : AbstractFoo {
        public Foo() { }
        public Foo(string name) { 
            Name = name;
        }

        public Foo(string name, int state) {
            Name = name;
            State = state;
        }

        public Foo(string name, int state, DateTime time) {
            Name = name;
            State = state;
        }

        public Foo(string name, int state, DateTime time, TimeSpan timeSpan) {
            Name = name;
            State = state;
        }

        public Foo(string name, int state, DateTime time, TimeSpan timeSpan, float v) {
            Name = name;
            State = state;
        }

        public override string Name { get; set; } = "wolfweb";
        
        public int State { get; set; }
    }

    public class FooEntityMapping : EntityTypeConfiguration<FooEntity> {
        public override void Configure(EntityTypeBuilder<FooEntity> builder) {
            builder.ToTable("Foo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Attribute).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<JObject>());
        }
    }

    [Table("Foo")]
    public class FooEntity : Entity<int> {
        public int      Age       { get; set; }
        public string?  Name      { get; set; }
        public DateTime Born      { get; set; }
        public string?  Address   { get; set; }
        public long     CreateAt  { get; set; }

        [ColumnExtend(Size = 4000)]
        public JObject? Attribute { get; set; }
        
        [Required]
        public string?  Password { get; set; }
    }

    public class FooViewModel : EntityViewModel<FooEntity, int> {
        public FooViewModel(FooEntity? entity = null) : base( entity ?? new FooEntity() ) { 

        }
        [Required]
        [DataType(DataType.DateTime)]
        public ValueObject? CreateAt { get; set; }

        [Required]
        [EqualTo("Password")]
        public ValueObject? VerifyPwd { get; set; }
    }

    public class FooDynamicViewModel : EntityViewModel<DynamicEntity, int> {
        public FooDynamicViewModel(DynamicEntity entity) : base(entity) {

        }
    }
}
