using GestionTicket.Models;
using System.ComponentModel.DataAnnotations; 

namespace GestionTicket.Models;

    public class PuestoLaboral
    {
        [Key]
        public int PuestoLaboralID { get; set; }
        public string? Descripcion { get; set; }
        public string? UsuarioClienteID { get; set; }
        public bool? Eliminado {get; set; } = true;    

        public ICollection<PuestoCategoria>? PuestoCategorias { get; set; }
    }