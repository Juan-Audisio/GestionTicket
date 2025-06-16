using System;
using System.ComponentModel.DataAnnotations;
using GestionTicket.Models;

namespace GestionTicket.Models;

public class Cliente
{
    public int ClienteID { get; set; }
    public string? Nombre { get; set; }
    public string? DNI { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Observaciones { get; set; }
    public string? UsuarioClienteID { get; set; }
    public bool? Eliminado { get; set; } = false;
} 