using Magicube.Core;
using Magicube.Data.Abstractions.Validation;
using Magicube.Forms.Services;
using Magicube.Forms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Forms.Controllers {
    [Authorize]
    public class FormDesignController : Controller {
        private readonly DbTableService _dbTableService;
        public FormDesignController(DbTableService dbTableService) {
            _dbTableService = dbTableService;
        }

        public async Task<IActionResult> Index(int id) {
            if (id < 0) throw new InvalidOperationException("无效的表单Id");

            var validations = DynamicEntityValidationFactory.Validations;



            return View();
        }

        [HttpPost]
        public IActionResult Index([FromBody] DbTableSchemaViewModel viewModel) {

            

            return null;
        }
    }
}
