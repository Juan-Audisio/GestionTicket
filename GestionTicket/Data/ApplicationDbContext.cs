
using GestionTicket.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

        : base(options)

    {

    }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ComentarioTicket> ComentariosTickets { get; set; }
        public DbSet<HistorialTicket> HistorialTicket { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PuestoLaboral> PuestoLaborales { get; set; }
        public DbSet<Desarrollador> Desarrolladores { get; set; }
        public DbSet<PuestoCategoria> PuestoCategorias { get; set; }

}