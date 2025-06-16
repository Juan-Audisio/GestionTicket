console.log("esta conectada")
const apiBase = "http://localhost:5287/api/home"; //MEDIO DE CONEXION A LA API

document.getElementById("registerForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const data = {
        nombreCompleto: document.getElementById("regNombre").value,
        email: document.getElementById("regEmail").value,
        password: document.getElementById("regPassword").value
    };

    const response = await fetch(`${apiBase}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    const result = await response.text();
    Swal.fire({
    icon: 'info',
    title: 'Resultado',
    text: result,
    confirmButtonText: 'OK'
    });

    if (response.ok) {
        console.log("Token guardado:", result.token);
        // window.location.href = "login.html";
    }
});