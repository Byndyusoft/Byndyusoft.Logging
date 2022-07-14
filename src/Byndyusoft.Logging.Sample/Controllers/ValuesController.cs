using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Byndyusoft.Logging.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            this.logger = logger;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var values = new[] { "value1", "value2" };

            logger.LogInformation("запрошены {@Values}", (object)values);
            return values;
        }

        // GET: api/<ValuesController>
        [HttpGet("error")]
        public IEnumerable<string> GetError()
        {
            try
            {
                ThrowErrorWithInnerError();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Исключение с сообщением");
                try
                {
                    ThrowErrorWithInnerError();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Должен совпасть хэш ошибки");
                    throw;
                }
            }

            return new[] { "Недостижимый код" };
        }

        public void ThrowError()
        {
            throw new NotImplementedException("Скоро сделаем");
        }

        public void ThrowErrorWithInnerError()
        {
            try
            {
                ThrowError();
            }
            catch (Exception e)
            {
                throw new Exception("Что-то пошло не так", e);
            }
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}