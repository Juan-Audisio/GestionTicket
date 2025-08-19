console.log("conectado")

window.onload = function() {
    obtenerDesarrollador();
    ObtenerPuestoLaboralDropDown();
};

async function ObtenerPuestoLaboralDropDown() {
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });
    const response = await fetch(`http://localhost:5287/api/PuestoLaboral`, {
        method: 'GET',
        headers: authHeaders()
    });

    if (!response.ok) {
        throw new Error('Error al obtener categorías');
    }
    const categorias = await response.json();
    const comboSelect = document.getElementById('puestolaboralSelect');
    comboSelect.innerHTML = "";
    let opciones = `<option value="0">[Seleccione un puesto laboral]</option>`;
    
    categorias.forEach(cat => {
        opciones += `<option value="${cat.puestoLaboralID}">${cat.descripcion}</option>`;
    });
    comboSelect.innerHTML = opciones;
}

const getToken = () => localStorage.getItem("token");

console.log("Token:", getToken());

const authHeaders = () => ({
    "Content-Type": "application/json",
    "Authorization": `Bearer ${getToken()}`
}); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch

function obtenerDesarrollador(){
    fetch('http://localhost:5287/api/Desarrollador', {
        method: 'GET',
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error en el servidor");
        }
        return response.json();
    })
    .then(data => mostrardesarrollador(data))
    .catch(error => console.error('Error al obtener desarrollador:', error));
}

function mostrardesarrollador(data) {
    const tbody = document.querySelector('tbody'); // Selecciona el tbody de tu tabla
    tbody.innerHTML = ''; // Limpia el contenido previo
    
    data.forEach(element => {

        const tr =tbody.insertRow();

        const nombredesarrollador = document.createTextNode(element.nombre)
        const tdnombredesarrollador = tr.insertCell(0)
        tdnombredesarrollador.appendChild(nombredesarrollador);



        const emaildesarrollador = document.createTextNode(element.email)
        const tdemaildesarrollador = tr.insertCell(1)
        tdemaildesarrollador.appendChild(emaildesarrollador);

        const dnidesarrollador = document.createTextNode(element.dni)
        const tddnidesarrollador = tr.insertCell(2)
        tddnidesarrollador.appendChild(dnidesarrollador);

        const telefonodesarrollador = document.createTextNode(element.telefono)
        const tdtelefonodesarrollador = tr.insertCell(3)
        tdtelefonodesarrollador.appendChild(telefonodesarrollador);

        const puestolaboraldarrollador = document.createTextNode(element.puestoLaborales?.descripcion ?? "—")
        const tdpuestolaboraldarrollador = tr.insertCell(4)
        tdpuestolaboraldarrollador.appendChild(puestolaboraldarrollador);



        const observaciondesarrollador = document.createTextNode(element.observaciones)
        const tdobservaciondesarrollador = tr.insertCell(5)
        tdobservaciondesarrollador.appendChild(observaciondesarrollador);

        
        
        
        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarDesarrollador(${element.desarrolladorID})`);
        editar.setAttribute('class', `btn btn-outline-success`);

        const tdEditar = tr.insertCell(6);
        tdEditar.appendChild(editar)

        const btnEstado = document.createElement('button');
        btnEstado.textContent = element.eliminado ? "Desactivar" : "Activar";
        btnEstado.className = element.eliminado ? "btn btn-outline-danger" : "btn btn-outline-warning";
        btnEstado.setAttribute('onclick', `cambiarEstadoDesarrollador(${element.desarrolladorID}, ${!element.eliminado})`);

        const tdEstado = tr.insertCell(7);
        tdEstado.appendChild(btnEstado);
        
    });
}

document.getElementById('formCreardesarrollador').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarDesarrollador();
});


function guardarDesarrollador() {
    const nombre = document.getElementById('nombredesarrollador').value.trim().toUpperCase();
    const dni = document.getElementById('dnidesarrollador').value.trim();
    const id = document.getElementById('desarrolladorId').value;
    const email = document.getElementById('emaildesarrollador').value.trim();
    const telefono = document.getElementById('telefonodesarrollador').value.trim().toUpperCase();
    const puestolaboralID = parseInt(document.getElementById('puestolaboralSelect').value);
    const observaciones = document.getElementById('observaciondesarrollador').value.trim().toUpperCase();



    let url = 'http://localhost:5287/api/Desarrollador';
    let method = 'POST';
    let bodyData = {
        Nombre: nombre,
        DNI: dni,
        Email: email,
        Telefono: telefono,
        PuestoLaboralID: parseInt(puestolaboralID),
        Observaciones: observaciones
    };
    console.log("Datos a enviar:", bodyData);

    if (id) {
        // Si hay ID, se actualiza
        url = `http://localhost:5287/api/Desarrollador/${id}`;
        method = 'PUT';
        bodyData.desarrolladorID = parseInt(id);
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
                throw new Error(`Error al guardar el desarrollador`);
            });
        }
        return response.json();
    })
    .then(data => {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: 'Desarrollador guardado correctamente.',
            confirmButtonText: 'OK'
          });
        // Limpiar campos
        document.getElementById('nombredesarrollador').value = "";
        document.getElementById('dnidesarrollador').value = "";
        document.getElementById('desarrolladorId').value = "";
        document.getElementById('emaildesarrollador').value = "";
        document.getElementById('telefonodesarrollador').value = "";
        document.getElementById('puestolaboralSelect').value = "0";
        document.getElementById('observaciondesarrollador').value = "";
        // Recargar lista
        obtenerDesarrollador();
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


const editarDesarrollador = (desarrolladorID) => {

    fetch(`http://localhost:5287/api/Desarrollador/${desarrolladorID}`, {
        method: "GET",
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error al obtener la desarrollador.");
        }
        return response.json();
    })
        .then(data => {
            document.getElementById('desarrolladorID').value = data.desarrolladorID;
            document.getElementById('nombredesarrollador').value = data.nombre;
            document.getElementById('dnidesarrollador').value = data.dni;
            document.getElementById('emaildesarrollador').value = data.email;
            document.getElementById('telefonodesarrollador').value = data.telefono;
            document.getElementById('observaciondesarrollador').value = data.observaciones;
            
            const eliminado = data.eliminado;
            const nombreInput = document.getElementById('nombredesarrollador');
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
            
            const modalElement = document.getElementById('desarrolladorModalLabel').closest('.modal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        })
        .catch(error => console.error("Error al obtener la desarrollador", error));
};

const cambiarEstadoDesarrollador = (desarrolladorID, nuevoEstado) => {
    Swal.fire({
        title: '¿Estás seguro?',
        text: '¿Querés cambiar el estado del desarrollador?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, cambiar',
        cancelButtonText: 'Cancelar'
      }).then((result) => {
        if (result.isConfirmed) {
          fetch(`http://localhost:5287/api/Desarrollador/Estado/${desarrolladorID}`, {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify({ eliminado: nuevoEstado })
          })
          .then(response => {
            if (!response.ok) throw new Error("Error al cambiar el estado");
            return response.json();
          })
          .then(data => {
            Swal.fire('¡Listo!', 'El estado del desarrollador se cambió correctamente.', 'success');
            obtenerDesarrollador();
          })
          .catch(error => {
            Swal.fire('Error', error.message, 'error');
          });
        }
      });
};

function cerrarSesion() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}


ObtenerPuestoLaboralDropDown();