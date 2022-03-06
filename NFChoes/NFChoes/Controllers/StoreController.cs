using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NFChoes.Dto;

namespace NFChoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private readonly IMemoryCache _memoryCache;

        public StoreController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("{storeId}")]
        public ActionResult<List<NFCHistory>> Get(string storeId, [FromQuery] bool isInStore)
        {
            List<NFCHistory> result = new();

            if(string.IsNullOrWhiteSpace(storeId) || !_memoryCache.TryGetValue(storeId + "-store", out result))
            {
                return Ok(result);
            }

            result = isInStore ? result.Where(h => h.OutTimestamp == null).ToList() : result;

            return Ok(result);
        }
    }
}
