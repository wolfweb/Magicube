﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Localization.Controllers {
    public class AdminController : Controller {
        private readonly IStringLocalizer L;
        public AdminController(IStringLocalizer localizer) {
            L = localizer;
        }
    }
}
