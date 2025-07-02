using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using appacd.Services;
using appacd.Models;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LayananController : ControllerBase
    {
        /// <summary>
        /// Represents a repository interface for managing Layanan (service) entities.
        /// Provides methods for accessing and manipulating Layanan data storage.
        /// </summary>
        private readonly ILayananRepository _layananRepository;

        public LayananController(ILayananRepository layananRepository)
        {
            _layananRepository = layananRepository;
        }

        [HttpGet]
        [ActionName("GetLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll()
        {
            var layananList = await _layananRepository.GetAllAsync();
            return Ok(layananList);
        }
    }
}