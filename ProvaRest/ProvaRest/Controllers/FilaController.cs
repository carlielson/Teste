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
                return NotFound("Não existe objeto a ser retornado");

            var id = _itemFilas.Max(x => x.IdControle);
            var itens = _itemFilas.Where(x=> x.IdControle ==id).ToList();
            var itensRemove = _itemFilas.Where(x => x.IdControle == id).ToList();

            foreach (var item in itensRemove)
            {
                _itemFilas.Remove(item);
            }
            
            return Ok(itens);
        }

        // POST api/<FilaController>
        [HttpPost]
        public void Post([FromBody] List<ItemFila> itens)
        {
            if (_itemFilas.Count > 0)
            {
                var id = _itemFilas.Max(x => x.IdControle);
                id++;

                foreach (var item in itens)
                {
                    item.IdControle = id;
                }
                _itemFilas.AddRange(itens);

            }
            else 
            {
                foreach (var item in itens)
                {
                    item.IdControle = 1;
                }
                _itemFilas.AddRange(itens);
            }
        }
    }
}
