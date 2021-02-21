using Microsoft.AspNetCore.Mvc;
using ProvaRest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        public static List<ItemFila> _itemFilas = new List<ItemFila>();
        
        // GET api/<FilaController>/5
        [HttpGet]
        public IActionResult GetLastItem()
        {
            if (_itemFilas.Count == 0) 
                return NotFound();

            return Ok(_itemFilas);
        }

        // POST api/<FilaController>
        [HttpPost]
        public void Post([FromBody] List<ItemFila> itens)
        {
            _itemFilas.Clear();
            _itemFilas.AddRange(itens);
        }
    }
}
