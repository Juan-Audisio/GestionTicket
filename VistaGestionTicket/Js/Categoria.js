window.onload = function() {
    obtenerCategoria();
};

function obtenerCategoria(){
    fetch('http://localhost:5287/api/Categoria')
    .then(response => response.json())
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
        eliminar.setAttribute('class', `btn btn-danger`);
        
        
        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarCategoria(${element.categoriaID})`);
        editar.setAttribute('class', `btn btn-success`);

        const tdEditar = tr.insertCell(1);
        tdEditar.appendChild(editar)

        const btnEstado = document.createElement('button');
        btnEstado.textContent = element.eliminado ? "Activar" : "Desactivar";
        btnEstado.className = element.eliminado ? "btn btn-warning" : "btn btn-danger";
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
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
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
        alert("Categoría guardada correctamente.");
        // Limpiar campos
        document.getElementById('descripcionCategoria').value = "";
        document.getElementById('categoriaId').value = "";
        // Recargar lista
        obtenerCategoria();
    })
    .catch(error => {
        alert("Ocurrió un error: " + error.message);
    });
}

const editarCategoria = (categoriaID) => {
    fetch(`http://localhost:5287/api/Categoria/${categoriaID}`)
        .then(response => response.json())
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
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
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
        method: 'DELETE'
    })
    .then(() => obtenerCategoria())
    .catch(error => console.error("No se puede eliminar la categoría.", error));
};




