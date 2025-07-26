using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripayController : ControllerBase
    {
        private readonly ITripayRepository _repo;

        public TripayController(ITripayRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetPaymentChannels")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPaymentChannels()
        {
            var res = await _repo.GetPaymentChannelsAsync();
            return Ok(res);
        }

    }
}