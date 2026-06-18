using Microsoft.AspNetCore.Mvc;
using Vendas_Dividas.Models;
using Vendas_Dividas.Services;

namespace Vendas_Dividas.Controllers
{
    [Route("api/[controller]")]
    public class DividasController : ControllerBase
    {
        private readonly DividaService _dividaService;
        private readonly ClienteService _clienteService;

        public DividasController(DividaService dividaService, ClienteService clienteService)
        {
            _dividaService = dividaService;
            _clienteService = clienteService;
        }

        [HttpGet]
        public ActionResult GetDividas()
        {
            var dividas = _dividaService.GetAllDividas();
            return Ok(dividas);
        }

        [HttpGet("pendentes")]
        public ActionResult GetDividasPendentes()
        {
            var dividas = _dividaService.GetDividasPendentes();
            return Ok(dividas);
        }

        [HttpGet("pagas")]
        public ActionResult GetDividasPagas()
        {
            var dividas = _dividaService.GetDividasPagas();
            return Ok(dividas);
        }

        [HttpGet("cliente/{clienteId}")]
        public ActionResult GetDividasByCliente(int clienteId)
        {
            var cliente = _clienteService.GetClienteById(clienteId);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var dividas = _dividaService.GetDividasByCliente(clienteId);
            return Ok(dividas);
        }

        [HttpPost]
        public ActionResult<Divida> PostDivida(Divida divida)
        {
            // Verifica se cliente existe
            var cliente = _clienteService.GetClienteById(divida.ClienteId);
            if (cliente == null)
                return BadRequest("Cliente não encontrado.");

            // Verifica se o valor é válido
            if (divida.Valor <= 0)
                return BadRequest("O valor da dívida deve ser maior que zero.");

            // Verifica se já existe dívida em aberto
            var dividasEmAberto = _dividaService.GetDividasEmAberto(divida.ClienteId);
            if (dividasEmAberto > 0)
                return BadRequest("Este cliente já possui uma dívida em aberto.");

            var novaDivida = _dividaService.CreateDivida(divida);
            return CreatedAtAction(nameof(GetDividas), new { id = novaDivida.Id }, novaDivida);
        }

        [HttpPut("{id}/pagar")]
        public IActionResult PagarDivida(int id)
        {
            var divida = _dividaService.GetDividaById(id);
            if (divida == null)
                return NotFound("Dívida não encontrada.");

            // VALIDAÇÃO: verifica se já está paga - como o professor pediu
            if (divida.Paga)
                return BadRequest("Esta dívida já foi paga.");

            _dividaService.PagarDivida(id);
            return NoContent();
        }
    }
}