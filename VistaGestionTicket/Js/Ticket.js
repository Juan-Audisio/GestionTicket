window.onload = obtenerCategoria();

function obtenerCategoria(){
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    }); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch
    //console.log(getToken())
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
    .then(data => comboCategorias(data))
    .catch(error => console.error('Error al obtener categorías:', error));
}

function comboCategorias(data) {
    const tbody = document.getElementById('categoriaTicket'); // Selecciona el tbody de tu tabla
    tbody.innerHTML = ''; // Limpia el contenido previo
    
    let opciones = '';
    data.forEach(element => {

        opciones += `<option value="${element.categoriaID}">${element.descripcion}</option>`;

        
    });

    tbody.innerHTML = opciones; 
    obtenerTickets();
}

function obtenerTickets(){
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    }); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch

    fetch('http://localhost:5287/api/Tickets', {
        method: 'GET',
        headers: authHeaders()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("No autorizado o error en el servidor");
        }
        return response.json();
    })
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


        const historialTicket = document.createElement('button');
        historialTicket.textContent ="Buscar";
        historialTicket.setAttribute('onclick', `buscarHistorial(${element.ticketID})`);
        editar.setAttribute('class', `btn btn-success`);

        const tdhistorialTicket = tr.insertCell(7);
        tdhistorialTicket.appendChild(historialTicket)
    });
}

document.getElementById('formCrearTicket').addEventListener('submit', function (event) {
    event.preventDefault();
    guardarTicket();
});



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
    const modo = document.getElementById('modoFormulario').value;
    const ticketID = document.getElementById('ticketID').value;
    const titulo = document.getElementById('tituloTicket').value;
    const descripcion = document.getElementById('descripcionTicket').value;
    const categoriaID = parseInt(document.getElementById('categoriaTicket').value);
    const prioridad = parseInt(document.getElementById('prioridadTicket').value);

    if (!titulo || !descripcion || !categoriaID || !prioridad) {
        alert("Todos los campos son obligatorios");
        return;
    }

    const ticket = {
        titulo,
        descripcion,
        categoriaID,
        prioridad
    };

    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });

    let url = 'http://localhost:5287/api/tickets';
    let method = 'POST';

    if (modo === 'editar') {
        ticket.ticketID = parseInt(ticketID);
        url = `http://localhost:5287/api/tickets/${ticketID}`;
        method = 'PUT';
    }

    fetch(url, {
        method: method,
        headers: authHeaders(),
        body: JSON.stringify(ticket)
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`Error ${response.status}: ${text}`);
            });
        }
        return response.json().catch(() => null); // en PUT no hay body
    })
    .then(() => {
        obtenerTickets();
        const modal = bootstrap.Modal.getInstance(document.getElementById('crearTicketModal'));
        modal.hide();

        // Reiniciar formulario
        document.getElementById('formCrearTicket').reset();
        document.getElementById('modoFormulario').value = 'crear';
        document.getElementById('ticketID').value = '';
    })
    .catch(error => {
        console.error("Error al guardar ticket:", error);
        alert(`Error: ${error.message}`);
    });
}

function editarTickets(ticketID) {
    // Buscar el ticket por su ID
    fetch(`http://localhost:5287/api/Tickets/${ticketID}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    })
    .then(response => {
        if (!response.ok) throw new Error("No se pudo obtener el ticket");
        return response.json();
    })
    .then(ticket => {
        document.getElementById('ticketID').value = ticket.ticketID;
        document.getElementById('tituloTicket').value = ticket.titulo;
        document.getElementById('descripcionTicket').value = ticket.descripcion;
        document.getElementById('categoriaTicket').value = ticket.categoriaID;
        document.getElementById('prioridadTicket').value = ticket.prioridad;

        document.getElementById('modoFormulario').value = 'editar';

        // Mostrar el modal
        const modal = new bootstrap.Modal(document.getElementById('crearTicketModal'));
        modal.show();
    })
    .catch(error => {
        console.error("Error al obtener ticket:", error);
        alert("No se pudo cargar el ticket para editar");
    });
}


function buscarHistorial(ticketID) {
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });

    console.log("Token:", getToken());
    
    fetch(`http://localhost:5287/api/historial/${ticketID}`, {
        method: 'GET',
        headers: authHeaders()
    })
    
    .then(response => {
        if (!response.ok) {
            throw new Error("No se pudo obtener el historial");
        }
        return response.json();
    })
    .then(data => {
        console.log("Historial recibido:", data);
        const tbody = document.querySelector("#tablaHistorial tbody");
        tbody.innerHTML = ''; 

        data.forEach(tic => {
            const tr = tbody.insertRow();

            // Fecha de cambio
            const fechaCambio = document.createTextNode(tic.fechaCambio);
            const tdFechaCambio = tr.insertCell(0);
            tdFechaCambio.classList.add("text-center");
            tdFechaCambio.appendChild(fechaCambio);

            // Campo modificado
            const campoModificado = document.createTextNode(tic.campoModificado);
            const tdCampoModificado = tr.insertCell(1);
            tdCampoModificado.classList.add("text-bold");
            tdCampoModificado.appendChild(campoModificado);

            // Valor anterior
            const valorAnterior = document.createTextNode(tic.valorAnterior);
            const tdValorAnterior = tr.insertCell(2);
            tdValorAnterior.appendChild(valorAnterior);

            // Valor nuevo
            const valorNuevo = document.createTextNode(tic.valorNuevo);
            const tdValorNuevo = tr.insertCell(3);
            tdValorNuevo.appendChild(valorNuevo);
        });

         //$("#historialModal").modal
         var modal = new bootstrap.Modal(document.getElementById('historialModal'));
            modal.show();

        // Mostrar modal después de cargar los datos
        // $("#historialModal").modal("show");
    })
    .catch(error => {
        console.error("Error al obtener historial:", error);
    });
}
