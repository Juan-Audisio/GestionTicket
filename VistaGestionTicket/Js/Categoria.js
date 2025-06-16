window.onload = function() {
    obtenerCategoria();
};

const getToken = () => localStorage.getItem("token");

console.log("Token:", getToken());

const authHeaders = () => ({
    "Content-Type": "application/json",
    "Authorization": `Bearer ${getToken()}`
}); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch


function obtenerCategoria(){
    fetch('http://localhost:5287/api/Categoria', {
        method: 'GET',
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error en el servidor");
        }
        return response.json();
    })
    .then(data => mostrarCategorias(data))
    .catch(error => console.error('Error al obtener categorías:', error));
}


function mostrarCategorias(data) {
    const tbody = document.querySelector('tbody'); // Selecciona el tbody de tu tabla
    tbody.innerHTML = ''; // Limpia el contenido previo
    
    data.forEach(element => {

        const tr =tbody.insertRow();

        const descripcionCategoria = document.createTextNode(element.descripcion)
        const tddescripcionCategoria = tr.insertCell(0)
        tddescripcionCategoria.appendChild(descripcionCategoria);

        const eliminar = document.createElement('button');
        eliminar.textContent = "Eliminar";
        eliminar.setAttribute('onclick',`eliminarCategoria(${element.categoriaID})`);
        eliminar.setAttribute('class', "btn btn-outline-warning dropdown-toggle");
        
        
        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarCategoria(${element.categoriaID})`);
        editar.setAttribute('class', `btn btn-outline-success`);

        const tdEditar = tr.insertCell(1);
        tdEditar.appendChild(editar)

        const btnEstado = document.createElement('button');
        btnEstado.textContent = element.eliminado ? "Activar" : "Desactivar";
        btnEstado.className = element.eliminado ? "btn btn-outline-warning" : "btn btn-outline-danger";
        btnEstado.setAttribute('onclick', `cambiarEstadoCategoria(${element.categoriaID}, ${!element.eliminado})`);

        const tdEstado = tr.insertCell(2);
        tdEstado.appendChild(btnEstado);

        
    });
}

document.getElementById('formCrearCategoria').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarCategoria();
});

function guardarCategoria() {
    const descripcion = document.getElementById('descripcionCategoria').value.trim().toUpperCase();
    const id = document.getElementById('categoriaId').value;


    let url = 'http://localhost:5287/api/Categoria';
    let method = 'POST';
    let bodyData = {
        descripcion: descripcion
    };

    if (id) {
        // Si hay ID, se actualiza
        url = `http://localhost:5287/api/Categoria/${id}`;
        method = 'PUT';
        bodyData.id = parseInt(id);
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
                throw new Error(`Error al guardar la categoria`);
            });
        }
        return response.json();
    })
    .then(data => {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: 'Categoría guardada correctamente.',
            confirmButtonText: 'OK'
          });
        // Limpiar campos
        document.getElementById('descripcionCategoria').value = "";
        document.getElementById('categoriaId').value = "";
        // Recargar lista
        obtenerCategoria();
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

const editarCategoria = (categoriaID) => {
    fetch(`http://localhost:5287/api/Categoria/${categoriaID}`, {
        method: "GET",
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error al obtener la categoría.");
        }
        return response.json();
    })
        .then(data => {
            document.getElementById('categoriaId').value = data.categoriaID;
            document.getElementById('descripcionCategoria').value = data.descripcion;
            const eliminado = data.eliminado;
            const descripcionInput = document.getElementById('descripcionCategoria');

            if (eliminado) {
                descripcionInput.disabled =  false;
                document.getElementById('avisoEliminado').style.display = 'none';

            } else {
                descripcionInput.disabled =  true;
                document.getElementById('avisoEliminado').style.display = 'block';
            }

            const modalElement = document.getElementById('categoriaModalLabel').closest('.modal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        })
        .catch(error => console.error("Error al obtener la categoría", error));
};


const cambiarEstadoCategoria = (categoriaID, nuevoEstado) => {
    if (confirm("¿Estás seguro de cambiar el estado de la categoría?")) {
        fetch(`http://localhost:5287/api/Categoria/Estado/${categoriaID}`, {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify({ eliminado: nuevoEstado })
        })
        .then(response => {
            if (!response.ok) throw new Error("Error al cambiar el estado");
            return response.json();
        })
        .then(() => {
            obtenerCategoria();
        })
        .catch(error => console.error("No se pudo cambiar el estado de la categoría.", error));
    }
};

const eliminarCategoria = (categoriaID) => {
    fetch(`http://localhost:5287/api/Categoria/${categoriaID}`, {
        method: 'DELETE',
        headers: authHeaders()
    })
    .then(() => obtenerCategoria())
    .catch(error => console.error("No se puede eliminar la categoría.", error));
};

function cerrarSesion() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}




