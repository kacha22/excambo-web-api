using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Excambo.Models
{
    public class MyGame
    {
        [Key]
        public int IdGame { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(64, ErrorMessage = "Máximo de 64 caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo de 2 caracteres")]
        public string GameName { get; set; }

        public int GameYear { get; set; }

        public string Genre { get; set; }
        
        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        public User User { get; set; }
    }
}