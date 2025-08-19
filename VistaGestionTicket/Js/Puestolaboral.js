window.onload = function() {
    obtenerPuesto();
    obtenerPuestoCategoria();
};

const getToken = () => localStorage.getItem("token");

console.log("Token:", getToken());

const authHeaders = () => ({
    "Content-Type": "application/json",
    "Authorization": `Bearer ${getToken()}`
}); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch

function obtenerPuesto(){
    fetch('http://localhost:5287/api/PuestoLaboral', {
        method: 'GET',
        headers: authHeaders()
    })
    
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error en el servidor");
        }
        return response.json();
    })
    .then(data => 
        mostrarPuestoLaboral(data),
        comboCategorias())
    .catch(error => console.error('Error al obtener puesto laboral:', error));
}

function mostrarPuestoLaboral(data) {
    const tbody = document.querySelector('tbody'); // Selecciona el tbody de tu tabla
    tbody.innerHTML = ''; // Limpia el contenido previo
    
    data.forEach(element => {

        const tr =tbody.insertRow();

        const descripcionPuesto = document.createTextNode(element.descripcion)
        const tddescripcionPuesto= tr.insertCell(0)
        tddescripcionPuesto.appendChild(descripcionPuesto);

        const eliminar = document.createElement('button');
        eliminar.textContent = "Eliminar";
        eliminar.setAttribute('onclick',`eliminarCategoria(${element.descripcionPuesto})`);
        eliminar.setAttribute('class', "btn btn-outline-warning dropdown-toggle");
        
        
        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarPuestoLaboral(${element.puestoLaboralID})`);
        editar.setAttribute('class', `btn btn-outline-success`);

        const tdEditar = tr.insertCell(1);
        tdEditar.appendChild(editar)

        const btnEstado = document.createElement('button');
        btnEstado.textContent = element.eliminado ? "Desactivar" : "Activar" ;
        btnEstado.className = element.eliminado ? "btn btn-outline-danger" : "btn btn-outline-warning";
        btnEstado.setAttribute('onclick', `cambiarEstadoPuestoLaboral(${element.puestoLaboralID}, ${!element.eliminado})`);

        const tdEstado = tr.insertCell(2);
        tdEstado.appendChild(btnEstado);

        const puetocategoria = document.createElement('button');
        puetocategoria.textContent ="asignar";
        puetocategoria.setAttribute('onclick', `asignarCategoria(${element.puestoLaboralID})`);
        puetocategoria.setAttribute('class', `btn btn-outline-warning`);

        const tdpuetocategoria = tr.insertCell(3);
        tdpuetocategoria.appendChild(puetocategoria)

        
    });
}

document.getElementById('formCrearPuestoLaboral').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarPuestoLaboral();
});

function guardarPuestoLaboral() {
    const descripcion = document.getElementById('decripcionPuestoLaboral').value.trim().toUpperCase();
    const id = document.getElementById('puestolaboralId').value;


    let url = 'http://localhost:5287/api/Puestolaboral';
    let method = 'POST';
    let bodyData = {
        descripcion: descripcion
    };

    if (id) {
        // Si hay ID, se actualiza
        url = `http://localhost:5287/api/Puestolaboral/${id}`;
        method = 'PUT';
        bodyData= {
            puestoLaboralID: parseInt(id),
            descripcion: descripcion
        };
    }

    fetch(url, {
        method: method,
        headers: authHeaders(),
        body: JSON.stringify(bodyData)
    })
    .then(response => {
        console.log("Respuesta:", response);
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`Error al guardar la puestolaboral`);
            });
        }
        return response.json();
    })
    .then(data => {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: 'PuestoLaboral guardada correctamente.',
            confirmButtonText: 'OK'
          });
        // Limpiar campos
        document.getElementById('decripcionPuestoLaboral').value = "";
        document.getElementById('puestolaboralId').value = "";
        // Recargar lista
        obtenerPuesto();
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




const editarPuestoLaboral = (id) => {
    const puestoLaboral = {
        puestoLaboralID: document.getElementById('puestolaboralId').value,
        descripcion: document.getElementById('decripcionPuestoLaboral').value,
        eliminado: false
    };
    fetch(`http://localhost:5287/api/Puestolaboral/${id}`, {
        method: "GET",
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error al obtener la puesto laboral.");
        }
        return response.json();
    })
        .then(data => {
            document.getElementById('puestolaboralId').value = data.puestoLaboralID;
            document.getElementById('decripcionPuestoLaboral').value = data.descripcion;
            const eliminado = data.eliminado;
            const descripcionInput = document.getElementById('decripcionPuestoLaboral');
            descripcionInput.disabled = eliminado; // Deshabilita el campo si el puesto laboral está eliminado

            const modalElement = document.getElementById('puestolaboralModalLabel').closest('.modal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        })
        .catch(error => console.error("Error al obtener la puestolaboral", error));
};


const cambiarEstadoPuestoLaboral = (puestolaboralID, nuevoEstado) => {
    Swal.fire({
        title: '¿Estás seguro?',
        text: '¿Querés cambiar el estado del Puesto Laboral?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, cambiar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
        fetch(`http://localhost:5287/api/Puestolaboral/Estado/${puestolaboralID}`, {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify({ eliminado: nuevoEstado })
        })
        .then(response => {
            if (!response.ok) throw new Error("Error al cambiar el estado");
            return response.json();
        })
        .then(data => {
            Swal.fire('¡Listo!', 'El estado del puesto laboral se cambió correctamente.', 'success');
                        obtenerPuesto();
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


const asignarCategoria = (puestoLaboralID) => {
    
    fetch(`http://localhost:5287/api/Puestolaboral/${puestoLaboralID}`, {
        method: "GET",
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error al obtener la puesto laboral.");
        }
        return response.json();
    })
        .then(data => {
            document.getElementById('puestolaboralId').value = data.puestoLaboralID;
            document.getElementById('puestolaboealcategoria').value = data.descripcion;

            const modalElement = document.getElementById('puestocategoriaModalLabel').closest('.modal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        })
        .catch(error => console.error("Error al obtener la puestolaboral", error));
};

async function comboCategorias() {
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });
    const response = await fetch(`http://localhost:5287/api/Categoria`, {
        method: 'GET',
        headers: authHeaders()
    });

    if (!response.ok) {
        throw new Error('Error al obtener categorías');
    }
    const categorias = await response.json();
    const comboSelect = document.getElementById('puestolaboralSelect');
    comboSelect.innerHTML = "";
    let opciones = `<option value="0">[Seleccione una categoría]</option>`;
    
    categorias.forEach(cat => {
        opciones += `<option value="${cat.categoriaID}">${cat.descripcion}</option>`;
    });
    comboSelect.innerHTML = opciones;
}

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById('formAsignarCategoria');
    if (form) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            asignarCategoriaPuestoLaboral();
        });
    } else {
        console.error("No se encontró el formulario con id 'formAsignarCategoria'");
    }
});

async function asignarCategoriaPuestoLaboral() {
    const puestoLaboralID = document.getElementById('puestolaboralId').value;
    const categoriaID = document.getElementById('selectCategoria').value;

    const puestoCategoria = {
        puestoLaboralID: parseInt(puestoLaboralID),
        categoriaID: parseInt(categoriaID)
    };

    try {
        const response = await fetch("http://localhost:5287/api/PuestoCategoria", {
            method: "POST",
            headers: authHeaders(),
            body: JSON.stringify(puestoCategoria)
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Éxito',
                text: 'Categoría asignada correctamente al puesto laboral.',
                confirmButtonText: 'OK'
            }).then(() => {
                // Limpiar campos
                document.getElementById('puestolaboralId').value = "";
                document.getElementById('selectCategoria').value = "0";
                // Recargar lista
                obtenerPuesto();
            });
        } else {
            const error = await response.text();
            console.error("Error en el guardado:", error);
            Swal.fire("Error", "No se pudo asignar la categoría", "error");
        }
    } catch (err) {
        console.error("Excepción en el fetch:", err);
        Swal.fire("Error", "Error de conexión con el servidor", "error");
    }
}

async function obtenerPuestoCategoria ()
{
    try {
        const response = await fetch("http://localhost:5287/api/PuestoCategoria", {
            method: "GET",
            headers: authHeaders()
        });

        if (!response.ok) {
            throw new Error("Error al obtener las categorías asignadas al puesto laboral.");
        }

        const data = await response.json();
        mostrarPuestoCategoria(data);
    } catch (error) {
        console.error("Error al obtener las categorías asignadas:", error);
    }
}

async function mostrarPuestoCategoria(data) {
    const tbody = document.querySelector('#tablaPuestoCategoria tbody');
    tbody.innerHTML = ''; 
    
    data.forEach(element => {
        const tr = tbody.insertRow();

        // Puesto laboral
        const tddescripcionPuesto = tr.insertCell(0);
        tddescripcionPuesto.textContent = element.puestoLaborales?.descripcion ?? "Sin puesto";

        // Categoría
        const tddescripcionCategoria = tr.insertCell(1);
        tddescripcionCategoria.textContent = element.categorias?.descripcion ?? "Sin categoría";
    });
}