using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerangkatController : ControllerBase
    {
        private readonly IPerangkatRepository _repo;

        public PerangkatController(IPerangkatRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("CheckQrCodeUnit")]
        public async Task<ActionResult<IEnumerable<dynamic>>> CheckQrCodeUnit(string decodedText)
        {
            var res = await _repo.CheckQrCodeUnit(decodedText);
            return Ok(res);
        }

        [HttpGet("GetPhoto")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPhoto(string Id)
        {
            var res = await _repo.GetPhotoAsync(Id);
            return Ok(res);
        }

    }
}