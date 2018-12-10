using Microsoft.AspNetCore.Mvc;
using MMX4.WebAPI.DBContext;
using MMX4.WebAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMX4.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IAccountManager _accountManager;

        public UpdateController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [HttpGet]
        public async Task<DownloadURL> GetDownloadLink()
        {
            return await _accountManager.GetURL();
        }

        [HttpPost]
        public async Task<bool> UpdateLink([FromBody]string request)
        {
            return await _accountManager.UpdateURL(request);
        }
    }
}