using System.ComponentModel.DataAnnotations;

namespace Magicube.Core.Models {
    public enum EntityStatus {
        [Display(Name = "待审核")]
        Pending,

        [Display(Name = "已启用")]
        Actived,

        [Display(Name = "已删除")]
        Deleted,

        [Display(Name = "已禁用")]
        Disabled
    }
}
