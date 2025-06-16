console.log("conectado")

window.onload = function() {
    obtenerCliente();
};


const getToken = () => localStorage.getItem("token");

console.log("Token:", getToken());

const authHeaders = () => ({
    "Content-Type": "application/json",
    "Authorization": `Bearer ${getToken()}`
}); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch


function obtenerCliente(){
    fetch('http://localhost:5287/api/Cliente', {
        method: 'GET',
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error en el servidor");
        }
        return response.json();
    })
    .then(data => mostrarCliente(data))
    .catch(error => console.error('Error al obtener cliente:', error));
}

function mostrarCliente(data) {
    const tbody = document.querySelector('tbody'); // Selecciona el tbody de tu tabla
    tbody.innerHTML = ''; // Limpia el contenido previo
    
    data.forEach(element => {

        const tr =tbody.insertRow();

        const nombrecliente = document.createTextNode(element.nombre)
        const tdnombrecliente = tr.insertCell(0)
        tdnombrecliente.appendChild(nombrecliente);



        const emailcliente = document.createTextNode(element.email)
        const tdemailcliente = tr.insertCell(1)
        tdemailcliente.appendChild(emailcliente);

        const dnicliente = document.createTextNode(element.dni)
        const tddnicliente = tr.insertCell(2)
        tddnicliente.appendChild(dnicliente);

        const telefonocliente = document.createTextNode(element.telefono)
        const tdtelefonocliente = tr.insertCell(3)
        tdtelefonocliente.appendChild(telefonocliente);


        const observacioncliente = document.createTextNode(element.observaciones)
        const tdobservacioncliente = tr.insertCell(4)
        tdobservacioncliente.appendChild(observacioncliente);

        
        
        
        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarCliente(${element.clienteID})`);
        editar.setAttribute('class', `btn btn-outline-success`);

        const tdEditar = tr.insertCell(5);
        tdEditar.appendChild(editar)

        const btnEstado = document.createElement('button');
        btnEstado.textContent = element.eliminado ? "Activar" : "Desactivar";
        btnEstado.className = element.eliminado ? "btn btn-outline-warning" : "btn btn-outline-danger";
        btnEstado.setAttribute('onclick', `cambiarEstadoCliente(${element.clienteID}, ${!element.eliminado})`);

        const tdEstado = tr.insertCell(6);
        tdEstado.appendChild(btnEstado);
        
    });
}

document.getElementById('formCrearCliente').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarCliente();
});


function guardarCliente() {
    const nombre = document.getElementById('nombrecliente').value.trim().toUpperCase();
    const dni = document.getElementById('dnicliente').value.trim();
    const id = document.getElementById('clienteId').value;
    const email = document.getElementById('emailcliente').value.trim();
    const telefono = document.getElementById('telefonocliente').value.trim().toUpperCase();
    const observaciones = document.getElementById('observacioncliente').value.trim().toUpperCase();



    let url = 'http://localhost:5287/api/Cliente';
    let method = 'POST';
    let bodyData = {
        nombre: nombre,
        dni: dni,
        email: email,
        telefono: telefono,
        observaciones: observaciones
    };

    if (id) {
        // Si hay ID, se actualiza
        url = `http://localhost:5287/api/Cliente/${id}`;
        method = 'PUT';
        bodyData.clienteID = parseInt(id);
    }

    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            ...authHeaders()
        },
        body: JSON.stringify(bodyData) 
    })
    .then(response => {
        console.log("Respuesta:", response);
        if (!response.ok) {
            return response.text().then(text => {
                console.error("Detalle del error:", text);
                throw new Error(`Error al guardar el cliente`);
            });
        }
        return response.json();
    })
    .then(data => {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: 'Cliente guardado correctamente.',
            confirmButtonText: 'OK'
          });
        // Limpiar campos
        document.getElementById('nombrecliente').value = "";
        document.getElementById('dnicliente').value = "";
        document.getElementById('clienteId').value = "";
        document.getElementById('emailcliente').value = "";
        document.getElementById('telefonocliente').value = "";
        document.getElementById('observacioncliente').value = "";
        // Recargar lista
        obtenerCliente();
    })
    .catch(error => {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Ocurrió un error: ' + error.message,
            confirmButtonText: 'OK'
          });
    });
}


const editarCliente = (clienteID) => {

    fetch(`http://localhost:5287/api/Cliente/${clienteID}`, {
        method: "GET",
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error al obtener la cliente.");
        }
        return response.json();
    })
        .then(data => {
            document.getElementById('clienteId').value = data.clienteID;
            document.getElementById('nombrecliente').value = data.nombre;
            document.getElementById('dnicliente').value = data.dni;
            document.getElementById('emailcliente').value = data.email;
            document.getElementById('telefonocliente').value = data.telefono;
            document.getElementById('observacioncliente').value = data.observaciones;
            
            const eliminado = data.eliminado;
            const nombreInput = document.getElementById('nombrecliente');
            const avisoEliminado = document.getElementById('avisoEliminado');

            if (eliminado) {
                nombreInput.disabled = false;
                if (avisoEliminado) {
                    avisoEliminado.style.display = 'none';
                }
            } else {
                nombreInput.disabled = true;
                if (avisoEliminado) {
                    avisoEliminado.style.display = 'block';
                }
            }
            
            const modalElement = document.getElementById('clienteModalLabel').closest('.modal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        })
        .catch(error => console.error("Error al obtener la cliente", error));
};


const cambiarEstadoCliente = (clienteID, nuevoEstado) => {
    Swal.fire({
        title: '¿Estás seguro?',
        text: '¿Querés cambiar el estado del cliente?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, cambiar',
        cancelButtonText: 'Cancelar'
      }).then((result) => {
        if (result.isConfirmed) {
          fetch(`http://localhost:5287/api/Cliente/Estado/${clienteID}`, {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify({ eliminado: nuevoEstado })
          })
          .then(response => {
            if (!response.ok) throw new Error("Error al cambiar el estado");
            return response.json();
          })
          .then(data => {
            Swal.fire('¡Listo!', 'El estado del cliente se cambió correctamente.', 'success');
                        obtenerCliente();
          })
          .catch(error => {
            Swal.fire('Error', error.message, 'error');
          });
        }
      });
};

const eliminarcliente = (clienteID) => {
    if (!confirm("¿Estás seguro de que querés eliminar este cliente?")) return;
    fetch(`http://localhost:5287/api/Cliente/${clienteID}`, {
        method: 'DELETE',
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`Error al eliminar: ${text}`);
            });
        }
        return response.text();
    })
    .then(() => {
        alert("Cliente eliminado correctamente.");
        obtenerCliente();
    })
    .catch(error => console.error("No se puede eliminar el cliente:", error));
};

function cerrarSesion() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}


