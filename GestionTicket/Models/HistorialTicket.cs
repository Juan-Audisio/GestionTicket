using System;
using System.ComponentModel.DataAnnotations;
using GestionTicket.Models;

namespace GestionTicket.Models;

public class HistorialTicket
{
    
    public int HistorialTicketID { get; set; }
    public int TicketID { get; set; }
    public string? CampoModificado { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public DateTime FechaCambio { get; set; }

     public virtual Ticket? Tickets { get; set; }
}