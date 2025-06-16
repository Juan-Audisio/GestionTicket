const apiBase = "http://localhost:5287/api/Home";

document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const data = {
        email: document.getElementById("loginEmail").value,
        password: document.getElementById("loginPassword").value
    };

    const response = await fetch(`${apiBase}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (response.ok) {
        const result = await response.json();
        document.getElementById("tokenOutput").textContent = result;
        localStorage.setItem("token", result.token);
        console.log("Token guardado:", result.token);
        window.location.href = "categoria.html";
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Fallo algo al momento del ingreso',
            confirmButtonText: 'Intentar de nuevo'
          });
    }
});