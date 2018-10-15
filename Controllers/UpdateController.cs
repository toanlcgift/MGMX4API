using Microsoft.AspNetCore.Mvc;
using MMX4.WebAPI.Model;
using System.Collections.Generic;

namespace MMX4.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        [HttpPost]
        public IEnumerable<DownloadURL> GetDownloadLink([FromBody]CheckVersionRequest request)
        {
            using (SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(Resources.DB))
            {
                var currentVersion = connection.Table<Version>().OrderByDescending(x => x.Value).FirstOrDefault().Value;
                if (request.Version < currentVersion)
                    return connection.Table<DownloadURL>().ToList();
                else
                    return null;
            }
        }
    }
}