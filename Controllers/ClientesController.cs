using Microsoft.AspNetCore.Mvc;
using Vendas_Dividas.Models;
using Vendas_Dividas.Services;

namespace Vendas_Dividas.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public ActionResult GetClientes([FromQuery] string? nome = null, [FromQuery] int pagina = 1)
        {
            var resultado = _clienteService.GetClientesComDividas(nome, pagina);
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public ActionResult<Cliente> GetClienteById(int id)
        {
            var cliente = _clienteService.GetClienteById(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            return Ok(cliente);
        }

        [HttpPost]
        public ActionResult<Cliente> PostCliente(Cliente cliente)
        {
            // Verifica se o cliente já existe
            var clienteExistente = _clienteService.GetClienteByCpf(cliente.Cpf);
            if (clienteExistente != null)
                return BadRequest("Já existe um cliente com este CPF.");

            // Validações simples - como ensinado nas aulas
            if (string.IsNullOrEmpty(cliente.NomeCompleto) || cliente.NomeCompleto.Length < 3)
                return BadRequest("Nome completo é obrigatório e deve ter no mínimo 3 caracteres.");

            if (string.IsNullOrEmpty(cliente.Cpf) || cliente.Cpf.Length != 11)
                return BadRequest("CPF é obrigatório e deve ter 11 dígitos.");

            if (cliente.DataNascimento == default)
                return BadRequest("Data de nascimento é obrigatória.");

            // Validação de email simples (opcional)
            if (!string.IsNullOrEmpty(cliente.Email) && !cliente.Email.Contains("@"))
                return BadRequest("E-mail inválido.");

            var novoCliente = _clienteService.CreateCliente(cliente);
            return CreatedAtAction(nameof(GetClienteById), new { id = novoCliente.Id }, novoCliente);
        }

        [HttpPut("{id}")]
        public ActionResult<Cliente> PutCliente(int id, Cliente cliente)
        {
            var clienteExistente = _clienteService.GetClienteById(id);
            if (clienteExistente == null)
                return NotFound("Cliente não encontrado.");

            // Verifica se o CPF está sendo alterado para um já existente
            if (clienteExistente.Cpf != cliente.Cpf)
            {
                var cpfExistente = _clienteService.GetClienteByCpf(cliente.Cpf);
                if (cpfExistente != null)
                    return BadRequest("CPF já cadastrado para outro cliente.");
            }

            // Validações simples
            if (string.IsNullOrEmpty(cliente.NomeCompleto) || cliente.NomeCompleto.Length < 3)
                return BadRequest("Nome completo é obrigatório e deve ter no mínimo 3 caracteres.");

            if (string.IsNullOrEmpty(cliente.Cpf) || cliente.Cpf.Length != 11)
                return BadRequest("CPF é obrigatório e deve ter 11 dígitos.");

            var clienteAtualizado = _clienteService.UpdateCliente(id, cliente);
            return Ok(clienteAtualizado);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            var cliente = _clienteService.GetClienteById(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            // Verifica se tem dívidas em aberto
            var dividasEmAberto = _clienteService.GetDividasEmAberto(id);
            if (dividasEmAberto > 0)
                return BadRequest("Não é possível excluir cliente com dívidas em aberto.");

            _clienteService.DeleteCliente(id);
            return NoContent();
        }
    }
}