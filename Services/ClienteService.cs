using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Vendas_Dividas.ContextDb;
using Vendas_Dividas.Models;

namespace Vendas_Dividas.Services
{
    public class ClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public List<Cliente> GetAllClientes()
        {
            return _context.Clientes
                .Include(c => c.Dividas)
                .ToList();
        }

        public Cliente? GetClienteById(int id)
        {
            return _context.Clientes
                .Include(c => c.Dividas)
                .FirstOrDefault(c => c.Id == id);
        }

        public Cliente? GetClienteByCpf(string cpf)
        {
            return _context.Clientes
                .FirstOrDefault(c => c.Cpf == cpf);
        }

        public int GetDividasEmAberto(int clienteId)
        {
            return _context.Dividas
                .Count(d => d.ClienteId == clienteId && !d.Paga);
        }

        public Cliente CreateCliente(Cliente cliente)
        {
            cliente.Idade = CalcularIdade(cliente.DataNascimento);

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            
            return cliente;
        }

        public Cliente UpdateCliente(int id, Cliente cliente)
        {
            var clienteExistente = _context.Clientes.Find(id);
            
            if (clienteExistente == null)
                throw new Exception("Cliente não encontrado.");

            clienteExistente.NomeCompleto = cliente.NomeCompleto;
            clienteExistente.Cpf = cliente.Cpf;
            clienteExistente.DataNascimento = cliente.DataNascimento;
            clienteExistente.Idade = CalcularIdade(cliente.DataNascimento);
            clienteExistente.Email = cliente.Email;
            clienteExistente.Celular = cliente.Celular;
            clienteExistente.Genero = cliente.Genero;
            clienteExistente.Observacao = cliente.Observacao;

            _context.SaveChanges();
            return clienteExistente;
        }

        public bool DeleteCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            
            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            
            return true;
        }

        public object GetClientesComDividas(string? nome, int pagina)
        {
            if (pagina < 1) pagina = 1;
            int tamanhoPagina = 10;

            var query = _context.Clientes
                .Include(c => c.Dividas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(c => 
                    c.NomeCompleto.ToUpper().Contains(nome.ToUpper()) ||
                    c.Cpf.Contains(nome));
            }

            var listaClientes = query.ToList();

            var resultado = listaClientes
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

            return resultado;
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) 
                idade--;
            return idade;
        }
    }
}