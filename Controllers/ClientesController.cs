using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas_Dividas.ContextDb;
using Vendas_Dividas.Models;

namespace Vendas_Dividas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetCliente([FromQuery] string? nome = null, [FromQuery] int pagina = 1)
        {
            if (pagina < 1) pagina = 1;

            int tamanhoPagina = 10;

            var query = _context.Clientes.Include(c => c.Dividas).AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(c => c.NomeCompleto.ToUpper().Contains(nome.ToUpper()));
            }

            var listaClientes = await query.ToListAsync();

            var resultadoOrdenadoEPaginado = listaClientes
                .Select(c => new {
                    c.Id,
                    c.NomeCompleto,
                    c.Cpf,
                    c.Email,
                    c.Celular,
                    c.DataNascimento,
                    c.Idade, 
                    TotalDividas = c.Dividas.Where(d => !d.Paga).Sum(d => d.Valor) 
                })
                .OrderByDescending(c => c.TotalDividas) 
                .Skip((pagina - 1) * tamanhoPagina)    
                .Take(tamanhoPagina)                   
                .ToList();

            return Ok(resultadoOrdenadoEPaginado);
        }

        [HttpPost]
		public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
		{
  	       //Validação de CPF existente
  	       var cpfExiste = await _context.Clientes.AnyAsync(c => c.Cpf == cliente.Cpf);
  	       if (cpfExiste)
  	       {
  	           return BadRequest("Já existe um cliente cadastrado com este CPF.");
  	       }
	    
		  // Idade calculada
  	       var hoje = DateTime.Today;
  	       var idadeCalculada = hoje.Year - cliente.DataNascimento.Year;
  	       if (cliente.DataNascimento.Date > hoje.AddYears(-idadeCalculada)) 
  	       {
  	           idadeCalculada--;
  	       }
	    
  	       cliente.Idade = idadeCalculada;
	    
  	       _context.Clientes.Add(cliente);
  	       await _context.SaveChangesAsync();
	    
   	       return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
}
    }
}