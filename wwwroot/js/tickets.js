const { createApp, ref, computed, onMounted } = Vue;

createApp({
    setup() {
        const tickets = ref([]);
        const ticketSeleccionado = ref(null);
        const ticketAnular = ref(null);
        const procesandoAnulacion = ref(false);
        const cargando = ref(true);
        const error = ref('');
        const fechaDesde = ref('');
        const fechaHasta = ref('');
        const modalDetalleInstance = ref(null);
        const modalAnulacionInstance = ref(null);
        const pagina = ref(1);
        const totalPaginas = ref(0);

        const formatFecha = (fechaString) => {
            if (!fechaString) return '';
            return new Date(fechaString).toLocaleString('es-AR', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });
        };

        const obtenerTickets = async (nroPagina = 1) => {
            cargando.value = true;
            error.value = '';
            pagina.value = nroPagina;

            try {
                const params = new URLSearchParams({
                    paginaNro: pagina.value
                });

                if (fechaDesde.value) params.append('fechaDesde', fechaDesde.value);
                if (fechaHasta.value) params.append('fechaHasta', fechaHasta.value);

                const response = await fetch(`/Tickets/ObtenerTodosJson?${params.toString()}`);

                if (response.ok) {
                    const data = await response.json();

                    const itemsRecibidos = data.items || data.Items || [];

                    tickets.value = itemsRecibidos.map(t => ({
                        id: t.id || t.Id,
                        fecha: t.fecha || t.Fecha,
                        usuarioNombre: t.usuarioNombre || (t.usuario ? `${t.usuario.nombre} ${t.usuario.apellido}` : 'Sistema'),
                        clienteNombre: t.clienteNombre || (t.cliente ? `${t.cliente.nombre} ${t.cliente.apellido}` : 'Consumidor Final'),
                        tipoPago: t.tipoPago !== undefined ? t.tipoPago : t.TipoPago,
                        total: t.total !== undefined ? t.total : t.Total,
                        activo: t.activo !== undefined ? t.activo : t.Activo
                    }));

                    const totalCount = data.totalCount || data.TotalCount || 0;
                    const pageSize = data.pageSize || data.PageSize || 10;

                    pagina.value = data.page || data.Page || 1;
                    totalPaginas.value = Math.max(1, Math.ceil(totalCount / pageSize));
                } else {
                    error.value = "No se pudo obtener el historial de tickets.";
                    tickets.value = [];
                }
            } catch (e) {
                console.error("Error al cargar tickets: ", e);
                error.value = "Error de conexión al cargar los tickets.";
                tickets.value = [];
            } finally {
                cargando.value = false;
            }
        };

        const irAPagina = (n) => {
            if (n < 1 || n > totalPaginas.value || n === pagina.value) return;
            obtenerTickets(n);
        }

        const filtrarTickets = () => {
            obtenerTickets(1);
        };

        const limpiarFiltros = () => {
            fechaDesde.value = '';
            fechaHasta.value = '';
            obtenerTickets(1);
        };

        const obtenerModalInstance = (idHtml) => {
            const el = document.getElementById(idHtml);
            if (!el) {
                console.error(`No se encontró el elemento HTML con id #${idHtml}`);
                return null;
            }
            return bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
        };

        const abrirDetalle = async (id) => {
            try {
                const response = await fetch(`/Tickets/ObtenerDetallesJson?ticketId=${id}`);
                if (response.ok) {
                    ticketSeleccionado.value = await response.json();
                    const modal = obtenerModalInstance('modalDetalleTicket');
                    if (modal) modal.show();
                } else {
                    console.error("Error 404/500 al obtener el detalle del ticket");
                }
            } catch (e) {
                console.error("Error al cargar detalle: ", e);
            }
        };

        const abrirAnulacion = (ticket) => {
            ticketAnular.value = ticket;
            const modal = obtenerModalInstance('modalAnulacionTicket');
            if (modal) modal.show();
        };

        const confirmarAnulacion = async () => {
            if (!ticketAnular.value) return;

            procesandoAnulacion.value = true;
            const ticket = ticketAnular.value;
            const numTicket = ticket.id;

            try {
                const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenInput ? tokenInput.value : '';

                const response = await fetch(`/Tickets/Anular/${numTicket}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                const result = await response.json().catch(() => null);

                if (response.ok && result?.success) {
                    ticket.activo = false;
                    if (modalAnulacionInstance.value) modalAnulacionInstance.value.hide();
                    obtenerTickets(pagina.value);
                } else {
                    console.error("No se pudo anular el ticket.");
                }

                await obtenerTickets(pagina.value);
            } catch (e) {
                console.error("Ha ocurrido un error al anular el ticket.", e);
            } finally {
                procesandoAnulacion.value = false;
                ticketAnular.value = null;
            }
        };

        onMounted(() => {
            const elDetalle = document.getElementById('modalDetalleTicket');
            if (elDetalle) modalDetalleInstance.value = new bootstrap.Modal(elDetalle);
            const elAnulacion = document.getElementById('modalAnulacionTicket');
            if (elAnulacion) modalAnulacionInstance.value = new bootstrap.Modal(elAnulacion);
            obtenerTickets();
        })

        return {
            tickets,
            ticketSeleccionado,
            ticketAnular,
            procesandoAnulacion,
            cargando,
            error,
            fechaDesde,
            fechaHasta,
            pagina,
            totalPaginas,
            formatFecha,
            filtrarTickets,
            limpiarFiltros,
            abrirDetalle,
            abrirAnulacion,
            confirmarAnulacion,
            irAPagina
        };
    }
}).mount('#app-tickets');
