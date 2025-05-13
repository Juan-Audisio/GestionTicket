window.onload = obtenerTickets();

function obtenerTickets(){
    fetch('http://localhost:5287/api/Tickets')
    .then(response => response.json())
    .then(data => mostrarTickets(data)) 
    .catch(error => console.error('Error al obtener Tickets:', error));
}

function mostrarTickets (data) { 
    const tbody = document.querySelector('tbody');
    tbody.innerHTML = ''; 

    data.forEach(element => {
        const tr = tbody.insertRow();

        const tituloTicket = document.createTextNode(element.titulo);
        const tdtituloTicket = tr.insertCell(0);
        tdtituloTicket.appendChild(tituloTicket);

        const descripcionTicket = document.createTextNode(element.descripcion);
        const tddescripcionTicket = tr.insertCell(1);
        tddescripcionTicket.appendChild(descripcionTicket);

        const categoriaTicket = document.createTextNode(element.categoriaDescripcion);
        const tdcategoriaTicket = tr.insertCell(2);
        tdcategoriaTicket.appendChild(categoriaTicket);

        const prioridadTicket = document.createTextNode(element.prioridad);
        const tdprioridadTicket = tr.insertCell(3);
        tdprioridadTicket.appendChild(prioridadTicket);

        const fechaCreacionTicket = document.createTextNode(element.fechaCreacion);
        const tdfechaCreacionTicket = tr.insertCell(4);
        tdfechaCreacionTicket.appendChild(fechaCreacionTicket);

        const estado = document.createTextNode(element.estado);
        const tdestado = tr.insertCell(5);
        tdestado.appendChild(estado);

        const editar = document.createElement('button');
        editar.textContent = "Editar";
        editar.setAttribute('onclick', `editarTickets(${element.ticketID})`);
        editar.setAttribute('class', `btn btn-success`);

        const tdEditar = tr.insertCell(6);
        tdEditar.appendChild(editar)
    });
}

document.getElementById('formCrearTicket').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarTicket();
});

const cargarSelects = async () => {
    try {
        const [resPrioridades, resEstados, resCategorias] = await Promise.all([
            fetch('http://localhost:5287/api/Tickets/prioridades'),
            fetch('http://localhost:5287/api/Tickets/estados'),
            fetch('http://localhost:5287/api/Tickets/listado')
        ]);

        const prioridades = await resPrioridades.json();
        const estados = await resEstados.json();
        const categorias = await resCategorias.json();

        llenarSelect('prioridadTicket', [
            { id: '', nombre: '- Seleccione Prioridad -' },
            ...prioridades
        ]);

        llenarSelect('estadoTicket', [
            { id: '', nombre: '- Seleccione Estado -' },
            ...estados
        ]);

        llenarSelect('categoriaTicket', [
            { id: '', nombre: '- Seleccione Categoría -' },
            ...categorias
        ]);

    } catch (error) {
        console.error("Error cargando selects:", error);
    }
};

const llenarSelect = (idSelect, opciones) => {
    const select = document.getElementById(idSelect);
    select.innerHTML = ''; // limpiar

    opciones.forEach(opcion => {
        const opt = document.createElement('option');
        opt.value = opcion.id;
        opt.textContent = opcion.nombre;
        select.appendChild(opt);
    });
};


function guardarTicket() {
    const id = document.getElementById('ticketID').value;
    const titulo = document.getElementById('tituloTicket').value;
    const descripcion = document.getElementById('descripcionTicket').value;
    const categoria = document.getElementById('categoriaTicket').value;
    const prioridad = document.getElementById('prioridadTicket').value;
    const estado = document.getElementById('estadoTicket').value;

    const ticket = {
        titulo: titulo,
        descripcion: descripcion,
        categoria: categoria,
        prioridad: parseInt(prioridad),  // Convertir a entero si es necesario
        estado: parseInt(estado), // Convertir a entero si es necesario
    };

    let url = 'http://localhost:5287/api/Tickets';
    let method = 'POST';

    if (id) {
        ticket.ticketID = parseInt(id);
        url = `http://localhost:5287/api/Tickets/${id}`;
        method = 'PUT';
    }

    fetch(url, {
        method: method,
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(ticket)
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`Error ${response.status}: ${text}`);
            });
        }
        return response.json();
    })
    .then(() => {
        // Limpiar formulario
        document.getElementById('formCrearTicket').reset();
        document.getElementById('ticketID').value = "";

        // Ocultar modal
        const modalElement = document.getElementById('ticketModalLabel');
        const modal = bootstrap.Modal.getInstance(modalElement);
        modal.hide();

        // Recargar listado de tickets
        obtenerTickets();
    })
    .catch(error => {
        console.error("Error al guardar el ticket:", error);
        alert("Error: " + error.message);
    });
};


