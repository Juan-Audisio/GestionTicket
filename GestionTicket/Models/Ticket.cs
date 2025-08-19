using System;
using System.ComponentModel.DataAnnotations;
using GestionTicket.Models;

namespace GestionTicket.Models;

public class Ticket
{
    public int TicketID { get; set; }
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public Estado Estado { get; set; }
    public Prioridad Prioridad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaCierre { get; set; }
    public int CategoriaID { get; set; }

    public string? UsuarioClienteID { get; set; }
    public virtual ApplicationUser? UsuarioCliente { get; set; }

    public virtual Categoria? Categorias { get; set; }
    public ICollection<HistorialTicket>? HistorialTickets { get; set;} 
}

public enum Estado
{
    Abierto = 1,
    EnProceso,
    Cerrado,
    Canselado 
}

public enum Prioridad
{
    Baja = 1,
    Media,
    Alta
}



public class TicketVista
{
    public int TicketID { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Estado { get; set; }
    public string Prioridad { get; set; }
    public DateTime FechaCreacion { get; set; }    
    public string FechaCreacionString => FechaCreacion.ToString("yyyy-MM-dd");
    public DateTime? FechaCierre { get; set; }
    public string UsuarioClienteID { get; set; }

    public int CategoriaID { get; set; }
    public string CategoriaDescripcion { get; set; }
}

public class TicketFiltroDTO
{
    public int? CategoriaID { get; set; }
    public int Prioridad { get; set; }
    public int Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}
