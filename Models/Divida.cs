using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vendas_Dividas.Models
{
    public class Divida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [JsonIgnore]
        public Cliente? Cliente { get; set; }

        [Required]
        public decimal Valor { get; set; }

        public bool Paga { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataPagamento { get; set; }
    }
}