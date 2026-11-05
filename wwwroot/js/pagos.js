const { createApp, ref, onMounted } = Vue;

createApp({
    setup() {
        const cliente = ref(null);
        const pagos = ref([]);
        const pagina = ref(1);
        const totalPaginas = ref (0);
        const fechaDesde = ref('');
        const fechaHasta = ref('');
        const procesando = ref(false);
        const pagoAConfirmar = ref(null);
        let modalConfirmarInstance = null;

        const urlParams = new URLSearchParams(window.location.search);
        const pathParts = window.location.pathname.split('/').filter(p => p);
        const idFromPath = pathParts.length > 0 ? pathParts[pathParts.length - 1] : null;
        const clienteId = urlParams.get('clienteId') || (idFromPath && !isNaN(idFromPath) ? idFromPath : null);

        const cargarDatos = async (nroPagina = 1) => {
            if (!clienteId) return;
            try {
                let url = `/Pagos/ObtenerDatosCuentaCorrienteJson?clienteId=${clienteId}&paginaNro=${nroPagina}`;

                if (fechaDesde.value) url += `&fechaDesde=${encodeURIComponent(fechaDesde.value)}`;
                if (fechaHasta.value) url += `&fechaHasta=${encodeURIComponent(fechaHasta.value)}`;

                const response = await fetch(url);
                if (response.ok)
                {
                    const data = await response.json();
                    cliente.value = data.cliente;
                    pagos.value = data.pagos.items || [];
                    pagina.value = data.pagos.page;
                    totalPaginas.value = Math.ceil(data.pagos.totalCount / data.pagos.pageSize) || 1;
                }
            } catch (e) {
                console.error("Error al cargar datos de cuenta corriente:", e);
            }
        };

        const filtrarPagos = () => {
            cargarDatos(1);
        }

        const limpiarFiltros = () => {
            fechaDesde.value = '';
            fechaHasta.value = '';
            cargarDatos(1);
        }

        const irAPagina = (n) => {
            if (n < 1 || n > totalPaginas.value || n === pagina.value) return;
            cargarDatos(n);
        }

        const abrirModalCobro = (pago) => {
            pagoAConfirmar.value = pago;
            if (modalConfirmarInstance) {
                modalConfirmarInstance.show();
            }
        };

        const confirmarYProcesarCobro = async () => {
            if (!pagoAConfirmar.value) return;

            procesando.value = true;
            try {
                const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenInput ? tokenInput.value : '';

                const params = new URLSearchParams();
                params.append("pagoId", pagoAConfirmar.value.id);

                const response = await fetch('/Pagos/RegistrarPago', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token
                    },
                    body: params
                });

                const res = await response.json().catch(() => null);

                if (response.ok && res && res.success) {
                    if (modalConfirmarInstance) modalConfirmarInstance.hide();
                    pagoAConfirmar.value = null;
                    await cargarDatos(pagina.value);
                } else {
                    alert(res?.message || "Error al registrar el cobro.");
                }
            } catch (e) {
                console.error("Error al procesar el cobro:", e);
            } finally {
                procesando.value = false;
            }
        };

        const obtenerClaseEstado = (estado) => {
            switch (estado) {
                case 'Pendiente': return 'badge bg-warning text-dark';
                case 'Pagado': return 'badge bg-success';
                case 'Cancelado': return 'badge bg-secondary';
                default: return 'badge bg-light text-dark';
            }
        };

        onMounted(() => {
            const modalEl = document.getElementById('modalConfirmarCobro');
            if (modalEl) {
                modalConfirmarInstance = new bootstrap.Modal(modalEl);
            }
            cargarDatos(1);
        });

        return {
            cliente,
            pagos,
            pagina,
            totalPaginas,
            fechaDesde,
            fechaHasta,
            procesando,
            pagoAConfirmar,
            filtrarPagos,
            limpiarFiltros,
            irAPagina,
            abrirModalCobro,
            confirmarYProcesarCobro,
            obtenerClaseEstado
        };
    }
}).mount('#app-pagos');