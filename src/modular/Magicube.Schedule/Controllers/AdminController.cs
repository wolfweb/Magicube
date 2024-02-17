using Magicube.Core.Signals;
using Magicube.Data.Migration;
using Magicube.Quartz.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicube.Schedule.Controllers {
    public class AdminController : Controller {
        private readonly IJobService _jobService;

        public AdminController(IJobService jobService) {
            _jobService = jobService; 
        }

        public async Task<IActionResult> Index() {
            var jobs = await _jobService.GetAllJobs();
            return View(jobs);
        }
    }
}
