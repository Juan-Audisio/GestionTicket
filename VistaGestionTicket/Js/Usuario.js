console.log("esta conectada")
const apiBase = "http://localhost:5287/api/home"; //MEDIO DE CONEXION A LA API

document.addEventListener("DOMContentLoaded", async () => {
    const getToken = () => localStorage.getItem("token");

    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });

    // Obtener datos actuales del usuario
    const response = await fetch("http://localhost:5287/api/home/usuario-logueado", {
        method: "GET",
        headers: authHeaders()
    });

    const usuario = await response.json();

    // Mostrar los datos en los campos
    document.getElementById("editarNombre").value = usuario.nombreCompleto;
    document.getElementById("editarEmail").value = usuario.email;
});

document.getElementById("usuariorForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const passwordActual = document.getElementById("passwordActual").value;
    const passwordNueva = document.getElementById("passwordNueva").value;
    const repetirPassword = document.getElementById("repetirPassword").value;

    // Validación en el frontend
    if (passwordActual && passwordNueva && repetirPassword) {
        if (passwordNueva !== repetirPassword) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'La nueva contraseña y su confirmación no coinciden',
                confirmButtonText: 'OK'
            });
            return;
        }

        if (passwordNueva.length < 6) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'La nueva contraseña debe tener al menos 6 caracteres',
                confirmButtonText: 'OK'
            });
            return;
        }
    }

    // Si se llenan algunos campos de contraseña pero no todos
    if ((passwordActual || passwordNueva || repetirPassword) && 
        (!passwordActual || !passwordNueva || !repetirPassword)) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Para cambiar la contraseña debe completar todos los campos de contraseña',
            confirmButtonText: 'OK'
        });
        return;
    }

    const data = {
        nombreCompleto: document.getElementById("editarNombre").value,
        email: document.getElementById("editarEmail").value,
        passwordActual: passwordActual,
        passwordNueva: passwordNueva,
        repetirPassword: repetirPassword
    };

    const response = await fetch("http://localhost:5287/api/home/editar-usuario", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(data)
    });

    const result = await response.text();

    Swal.fire({
        icon: response.ok ? 'success' : 'error',
        title: 'Resultado',
        text: result,
        confirmButtonText: 'OK'
    }).then(() => {
        if (response.ok) {
            // Limpiar los campos de contraseña después de un cambio exitoso
            document.getElementById("passwordActual").value = "";
            document.getElementById("passwordNueva").value = "";
            document.getElementById("repetirPassword").value = "";
        }
    });
});