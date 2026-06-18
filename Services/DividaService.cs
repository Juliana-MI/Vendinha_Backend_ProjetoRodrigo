using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Vendas_Dividas.ContextDb;
using Vendas_Dividas.Models;

namespace Vendas_Dividas.Services
{
    public class DividaService
    {
        private readonly AppDbContext _context;

        public DividaService(AppDbContext context)
        {
            _context = context;
        }

        public Divida? GetDividaById(int id)
        {
            return _context.Dividas.Find(id);
        }

        public int GetDividasEmAberto(int clienteId)
        {
            return _context.Dividas
                .Count(d => d.ClienteId == clienteId && !d.Paga);
        }

        public Divida CreateDivida(Divida divida)
        {
            divida.DataCriacao = DateTime.Now;
            divida.Paga = false;

            _context.Dividas.Add(divida);
            _context.SaveChanges();

            return divida;
        }

        public bool PagarDivida(int id)
        {
            var divida = _context.Dividas.Find(id);
            
            if (divida == null)
                throw new Exception("Dívida não encontrada.");

            divida.Paga = true;
            divida.DataPagamento = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public List<Divida> GetDividasByCliente(int clienteId)
        {
            return _context.Dividas
                .Where(d => d.ClienteId == clienteId)
                .OrderByDescending(d => d.DataCriacao)
                .ToList();
        }

        public List<Divida> GetAllDividas()
        {
            return _context.Dividas
                .Include(d => d.Cliente)
                .OrderByDescending(d => d.DataCriacao)
                .ToList();
        }

        public List<Divida> GetDividasPendentes()
        {
            return _context.Dividas
                .Include(d => d.Cliente)
                .Where(d => !d.Paga)
                .OrderByDescending(d => d.DataCriacao)
                .ToList();
        }

        public List<Divida> GetDividasPagas()
        {
            return _context.Dividas
                .Include(d => d.Cliente)
                .Where(d => d.Paga)
                .OrderByDescending(d => d.DataPagamento)
                .ToList();
        }
    }
}