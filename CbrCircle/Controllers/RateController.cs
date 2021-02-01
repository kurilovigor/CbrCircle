using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Globalization;
using CbrCircle.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace CbrCircle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IRateServiceOptions _options;
        private readonly ICbrDailyInfo _cbrDailyInfo;
        private readonly IMemoryCache _cache;
        public RateController(
            ILogger<RateController> logger, 
            IRateServiceOptions options,
            ICbrDailyInfo cbrDailyInfo,
            IMemoryCache cache)
        {
            _logger = logger;
            _options = options;
            _cbrDailyInfo = cbrDailyInfo;
            _cache = cache;
        }
        [HttpGet("x/{x}/y/{y}")]
        public async Task<ActionResult<Model.RateResponse>> Get(double x, double y)
        {
            // валидация входных значений
            if (x == 0)
                return BadRequest("Invalid [x] value"); // новозможно определить вертикальную полуполосткость
            if (y == 0)
                return BadRequest("Invalid [y] value"); // новозможно определить горизантальную полуполосткость
            try
            {
                // проверка на вхождение в круг
                var vectorRadius = Convert.ToDecimal(Math.Sqrt(x * x + y * y));
                if (vectorRadius > _options.Radius)
                    return BadRequest($"Vector length should be less or equal {_options.Radius}");
                // расчет квадранта круга
                var quater = x > 0 ?
                                (y > 0 ? Quater.Today : Quater.Tomorrow) :
                                (y > 0 ? Quater.Yesterday : Quater.DayBeforeYesterday);
                DateTime dt = quater switch
                    {
                        Quater.Today => DateTime.Today,
                        Quater.Tomorrow => DateTime.Today.AddDays(1),
                        Quater.Yesterday => DateTime.Today.AddDays(-1),
                        Quater.DayBeforeYesterday => DateTime.Today.AddDays(-2)
                    };
                // Получение курса на заданную дату
                // Здесь мы используем простое кэширование
                decimal? rate = null;
                var cacheKey = $"{dt}::{_options.Currency}::rate";
                if (!_cache.TryGetValue(cacheKey, out rate))
                    rate = await _cbrDailyInfo.GetCurs(dt);
                if (!rate.HasValue)
                    return BadRequest("Currency rate not found");
                _cache.Set(cacheKey, rate);
                return new Model.RateResponse
                {
                    Date = dt,
                    Quater = quater.ToString(),
                    Radius = _options.Radius,
                    Rate = rate.Value,
                    Сurrency = _options.Currency
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Internal error", null);
                return StatusCode(500);
            }
        }
    }
}
