using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetProduct")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetProduct()
        {
            var res = await _repo.GetProduct();
            return Ok(res);
        }
    }
}