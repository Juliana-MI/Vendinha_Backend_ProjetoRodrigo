using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas_Dividas.ContextDb;
using Vendas_Dividas.Models;

namespace Vendas_Dividas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DividasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DividasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Divida>> PostDivida(Divida divida)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == divida.ClienteId);
            if (!clienteExiste)
            {
                return BadRequest("O Cliente informado não existe.");
            }

            var possuiDividaEmAberto = await _context.Dividas
                .AnyAsync(d => d.ClienteId == divida.ClienteId && d.Paga == false);

            if (possuiDividaEmAberto)
            {
                return BadRequest("Este cliente já possui uma dívida em aberto. Quite a dívida anterior antes de pendurar uma nova.");
            }

            divida.DataCriacao = DateTime.Now;
            _context.Dividas.Add(divida);
            await _context.SaveChangesAsync();

            return Ok(divida);
        }

        [HttpPut("{id}/pagar")]
        public async Task<IActionResult> PagarDivida(int id)
        {
            var divida = await _context.Dividas.FindAsync(id);
            if (divida == null)
            {
                return NotFound("Dívida não encontrada.");
            }

            divida.Paga = true;
            divida.DataPagamento = DateTime.Now; 

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}