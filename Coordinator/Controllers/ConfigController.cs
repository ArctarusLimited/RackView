using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Models.Config;
using Coordinator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Coordinator.Controllers
{
    [Route("api/v0/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigController : Controller
    {
        private readonly ISrnRepository _service;
        private readonly ISrnSecurityService _security;
        public ConfigController(ISrnRepository service, ISrnSecurityService security)
        {
            _service = service;
            _security = security;
        }

        // GET api/v0/config/urn:srn:v0:global:discovery.v4.enabled
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key, SrnDto value)
        {
            if (!await _security.Match(key, User)) return Forbid();

            try
            {
                var srn = (Srn) key;

                if (string.IsNullOrWhiteSpace(srn.Namespace))
                    throw new SrnException("Bulk retrieving namespaces is not currently supported.");

                return Json(value.Providers != null
                    ? await _service.GetAsync(srn, value.Providers)
                    : await _service.GetAsync(srn, value.Provider));
            }
            catch (SrnException e)
            {
                return BadRequest(new { e.Message });
            }
        }

        // PUT api/v0/config/urn:srn:v0:discovery.v4.enabled
        [HttpPut("{key}")]
        public async Task<IActionResult> Put(string key, SrnDto value)
        {
            if (!await _security.Match(key, User)) return Forbid();

            try
            {
                if (value.Data == null)
                    throw new SrnException("SRN data must not be null. Use the Delete method to remove a value.");

                var srn = (Srn)key;
                if (!srn.HasNamespace() || !srn.HasKey())
                    throw new SrnException("Updating SRN data in bulk is not currently supported.");

                await _service.SetAsync(srn, value.Data, value.Provider);
                return Ok();
            }
            catch (SrnException e)
            {
                return BadRequest(new { e.Message });
            }
        }

        // DELETE api/v0/config/urn:srn:v0:discovery.v4.enabled
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key, SrnDto value)
        {
            if (!await _security.Match(key, User)) return Forbid();

            try
            {
                var srn = (Srn)key;
                if (string.IsNullOrWhiteSpace(srn.Namespace))
                    throw new SrnException("No namespace or key was provided to delete.");

                await _service.DeleteAsync(srn, value.Provider);
                return Ok();
            }
            catch (SrnException e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }
}
