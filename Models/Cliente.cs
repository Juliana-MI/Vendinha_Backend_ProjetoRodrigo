using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vendas_Dividas.Models
{
    public class Cliente
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]     
        [StringLength(11)]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }
        
        [JsonIgnore]
        public int Idade { get; set; }

        public string? Email { get; set; }

        public string? Celular { get; set; }
        
        public string? Genero { get; set; }
        
        public string? Observacao { get; set; }

        [JsonIgnore]
        public ICollection<Divida> Dividas { get; set; } = new List<Divida>();
    }
}