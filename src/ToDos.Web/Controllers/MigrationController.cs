using System;
using System.Diagnostics.Contracts;
using System.Web.Http;

namespace ToDos.Web.Controllers
{
    public class MigrationController : ApiController
    {
        private readonly LegacyMigrationWorker Worker;

        public MigrationController(LegacyMigrationWorker worker)
        {
            Contract.Requires<System.ArgumentNullException>(worker != null, "worker");
            Worker = worker;
        }

        [Route("api/Migrate")]
        [HttpPost]
        public IHttpActionResult Migrate()
        {
            try
            {
                Worker.ExecuteMigration();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
