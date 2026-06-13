using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vendas_Dividas.Models;

public class Cliente
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string NomeCompleto { get; set; } = string.Empty;

	[Required]
	public string Cpf { get; set; } = string.Empty;

	[Required]
	public DateTime DataNascimento { get; set; }
	
	[Required]
	public int Idade { get; set; }

	public string? Email { get; set; }
	public string? Celular { get; set; }
	public string? Genero { get; set; }
	public string? Observacao { get; set; }

	public ICollection<Divida> Dividas { get; set; } = new List<Divida>();
}