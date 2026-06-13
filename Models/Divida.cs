using System;
using System.ComponentModel.DataAnnotations;

namespace Vendas_Dividas.Models;

public class Divida
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int ClienteId { get; set; }

	public Cliente? Cliente { get; set; }

	[Required]
	public decimal Valor { get; set; }

	[Required]
	public bool Paga { get; set; } = false;

	[Required]
	public DateTime DataCriacao { get; set; } = DateTime.Now;

	public DateTime? DataPagamento { get; set; }
}