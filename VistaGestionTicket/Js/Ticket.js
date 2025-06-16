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
    .then(data => buscadorCategoria(data))
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
        editar.setAttribute('class', `btn btn-outline-success`);

        const tdEditar = tr.insertCell(6);
        tdEditar.appendChild(editar)


        const historialTicket = document.createElement('button');
        historialTicket.textContent ="Buscar";
        historialTicket.setAttribute('onclick', `buscarHistorial(${element.ticketID})`);
        historialTicket.setAttribute('class', `btn btn-outline-warning`);

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
        console.log("Token:", getToken());
        console.log("Historial recibido:", data);
        
        const tbody = document.querySelector("#tablaHistorial tbody");
        tbody.innerHTML = "";
        
        data.forEach(tic => {
            const row = document.createElement("tr");
    
            row.innerHTML = `
                <td class="text-center">${tic.fechaCambio}</td>
                <td class='text-bold'>${tic.campoModificado}</td>
                <td>${tic.valorAnterior}</td>
                <td>${tic.valorNuevo}</td>
            `;
            tbody.appendChild(row);
        });

        getTickets()
    
        $("#historialModal").modal("show");
    })
    .catch(error => {
        console.error("Error al obtener historial:", error);
    });
}


document.getElementById("CategoriaIDBuscar").onchange = function () {
    getTickets();
};
document.getElementById("prioridadBuscar").onchange = function () {
    getTickets();
};
document.getElementById("estadoBuscar").onchange = function () {
    getTickets();
};

document.getElementById("FechaDesdeBuscar").onchange = function () {
    getTickets();
};

document.getElementById("FechaHastaBuscar").onchange = function () {
    getTickets();
};


async function buscadorCategoria() {

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
    const comboSelectBuscar = document.getElementById('CategoriaIDBuscar');

    comboSelectBuscar.innerHTML = "";
    let opcionesBuscar = `<option value="0">[Todas las categorías]</option>`;

    categorias.forEach(cat => {
        opcionesBuscar += `<option value="${cat.categoriaID}">${cat.descripcion}</option>`;
    });

    comboSelectBuscar.innerHTML = opcionesBuscar;
    
}

async function getTickets() {
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });

    try {
        const categoriaIDBuscar = parseInt(document.getElementById("CategoriaIDBuscar").value);
        const prioridadBuscar = parseInt(document.getElementById("prioridadBuscar").value);
        const estadoBuscar = parseInt(document.getElementById("estadoBuscar").value);
        const fechaDesde = document.getElementById("FechaDesdeBuscar").value;
        const fechaHasta= document.getElementById("FechaHastaBuscar").value;
        const filtros = { 
            CategoriaID: categoriaIDBuscar,
            Prioridad: prioridadBuscar,
            Estado: estadoBuscar,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta
         };
        
        const response = await fetch("http://localhost:5287/api/Tickets/filtrar", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                ...authHeaders()
            },
            
            body: JSON.stringify(filtros)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Error al obtener tickets: ${response.status} - ${errorText}`);
        }

        const tickets = await response.json();
        const tbody = document.querySelector("tbody");

        if (!tbody) {
            alert("Error: No se encontró la tabla para mostrar los tickets");
            return;
        }

        tbody.innerHTML = "";
        tickets.forEach(tic => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td class="text-center">${tic.titulo}</td>
                <td class='text-bold'>${tic.descripcion}</td>
                <td>${tic.categoriaDescripcion}</td>
                <td class="text-center text-bold">${tic.prioridad}</td>
                <td class="text-center text-bold">${tic.fechaCreacion}</td>
                <td class="text-center text-bold">${tic.estado}</td>
                <td class="text-center"><button class="btn btn-outline-success" onclick="prepararEdicion(${tic.ticketID})">Editar</button></td>
                <td class="text-center"><button class="btn btn-outline-warning" onclick="buscarHistorial(${tic.ticketID})">Buscar</button></td>
            `;
            tbody.appendChild(row);
        });

    } catch (error) {
        console.error("Error en getTickets:", error);
    }
}

function cerrarSesion() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}

