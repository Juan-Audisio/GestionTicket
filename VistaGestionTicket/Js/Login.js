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
        alert("Login fallido");
    }
});


        // //FUNCION DE LEER TOKEN DEL DISPOSITIVO
        // const getToken = () => localStorage.getItem("token");

        // async function cerrarSesion() {
        //     const token = getToken();
        //     const email = localStorage.getItem("email");
        //     if (!token || !email) {
        //         localStorage.removeItem("token");
        //         localStorage.removeItem("email");
        //         window.location.href = "Categoria.html";
        //         return;
        //     }

        //     try {
        //         const res = await fetch("https://localhost:5287/api/home/logout", {
        //             method: "POST",
        //             headers: {
        //                 "Content-Type": "application/json",
        //                 "Authorization": `Bearer ${token}`
        //             },
        //             body: JSON.stringify({ email })
        //         });

        //         if (res.ok) {
        //             alert("Sesión cerrada correctamente");
        //         } else {
        //             alert("Error al cerrar sesión: " + await res.text());
        //         }
        //     } catch (error) {
        //         console.error("Error en logout:", error);
        //     }

        //     // Limpiar token y redirigir
        //     localStorage.removeItem("token");
        //     localStorage.removeItem("email");
        //     window.location.href = "Categoria.html";
        // }