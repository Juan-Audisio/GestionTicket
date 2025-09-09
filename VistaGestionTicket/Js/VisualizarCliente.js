window.onload = function() {
    obtenerCliente();
    comboClientes();
};

function obtenerCliente(){
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    }); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch
    //console.log(getToken())
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
    .then(data => {
        obtenerTickets();
    })
    .catch(error => console.error('Error al obtener Cliente:', error));
}

function obtenerTickets(){
    const value = document.getElementById("ClienteIDBuscar").value;
    const filtro = {
        clienteID: value ? parseInt(value) : 0
    };
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    }); // Pong en una constante el header de autorizacion para no repetirlo en cada fetch

    fetch('http://localhost:5287/api/obtenerTicketporCliente', {
        method: 'POST',
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

// esta ok
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

    });
}


document.getElementById("ClienteIDBuscar").onchange = function () {
    obtenerCliente();
};


async function comboClientes() {
    const getToken = () => localStorage.getItem("token");
    const authHeaders = () => ({
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    });
    const response = await fetch(`http://localhost:5287/api/Cliente`, {
        method: 'GET',
        headers: authHeaders()
    });

    if (!response.ok) {
        throw new Error('Error al obtener clientes');
    }

    const cliente = await response.json();
    const comboSelectBuscar = document.getElementById('ClienteIDBuscar');

    comboSelectBuscar.innerHTML = "";
    let opcionesBuscar = `<option value="0">[Todas las cliente]</option>`;

    cliente.forEach(cat => {
        opcionesBuscar += `<option value="${cat.clienteID}">${cat.nombre}</option>`;
    });
    comboSelectBuscar.innerHTML = opcionesBuscar;
    
}



function cerrarSesion() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}

