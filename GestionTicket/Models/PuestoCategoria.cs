using GestionTicket.Models;
using System.ComponentModel.DataAnnotations; 


namespace GestionTicket.Models;

    public class PuestoCategoria
    {
        [Key]
        public int PuestoCategoriaID { get; set; }  
        public int CategoriaID { get; set; }
        public int PuestoLaboralID { get; set; }

        public virtual Categoria? Categorias { get; set; }
        public virtual  PuestoLaboral? PuestoLaborales { get; set; }

    }