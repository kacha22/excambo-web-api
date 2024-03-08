using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Excambo.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(2, ErrorMessage = "Mínimo de 2 caracteres")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(255, ErrorMessage = "Máximo 64 caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo de 2 caracteres")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(255, ErrorMessage = "Máximo 255 caracteres")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres")]
        public string UserEmail { get; set; }

        public Point GeographicData { get; set; }

    }
}