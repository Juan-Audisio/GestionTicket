using System.ComponentModel.DataAnnotations;

namespace GestionTicket.Models;

public class ComentarioTicket
{
    [Key]
    public int ComentarioID { get; set; }
    public int TicketID { get; set; }
    public int UsuarioID { get; set; }
    public string Mensaje { get; set; }
    public DateTime FechaCreacion { get; set; }
}