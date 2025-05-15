using GestionTicket.Models;
using System.ComponentModel.DataAnnotations; 

namespace GestionTicket.Models;

    public class Categoria
    {
        [Key]
        public int CategoriaID { get; set; }
        public string? Descripcion { get; set; }
        public bool? Eliminado {get; set; } = true;    

        public ICollection<Ticket>? Tickets { get; set; }
    }