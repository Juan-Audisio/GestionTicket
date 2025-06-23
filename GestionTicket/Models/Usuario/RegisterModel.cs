 using GestionTicket.Models.Usuario;

 namespace GestionTicket.Models.Usuario
 {
     public class RegisterModel
     {
         public string Email { get; set; }
         public string Password { get; set; }
         public string NombreCompleto { get; set; }
     }



 public class EditarUsuarioModel
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string PasswordActual { get; set; }
        public string PasswordNueva { get; set; }
        public string RepetirPassword { get; set; }
    }
}