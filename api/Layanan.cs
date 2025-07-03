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

        [HttpGet("GetLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll()
        {
            var layananList = await _layananRepository.GetAllAsync();
            return Ok(layananList);
        }

        [HttpGet("GetLayananById")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetLayananById(string Id)
        {
            var layananList = await _layananRepository.GetLayananById(Id);
            return Ok(layananList);
        }

        [HttpGet("InfoLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> InfoLayanan(string Id)
        {
            var layananList = await _layananRepository.InfoLayananAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("KeluhanMasalah")]
        public async Task<ActionResult<IEnumerable<dynamic>>> KeluhanMasalah(string Id)
        {
            var layananList = await _layananRepository.KeluhanMasalahAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("JasaLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> JasaLayanan(string Id)
        {
            var layananList = await _layananRepository.JasaLayananAsync(Id);
            return Ok(layananList);
        }
        [HttpGet("JenisProperti")]
        public async Task<ActionResult<IEnumerable<dynamic>>> JenisProperti(string Id)
        {
            var layananList = await _layananRepository.JenisPropertiAsync(Id);
            return Ok(layananList);
        }
    }
}