const { createApp, ref, onMounted } = Vue;

createApp({
    setup() {
        const cliente = ref(null);
        const pagos = ref([]);
        const procesando = ref(false);
        const pagoAConfirmar = ref(null);
        let modalConfirmarInstance = null;

        const urlParams = new URLSearchParams(window.location.search);
        const pathParts = window.location.pathname.split('/').filter(p => p);
        const idFromPath = pathParts.length > 0 ? pathParts[pathParts.length - 1] : null;
        const clienteId = urlParams.get('clienteId') || (idFromPath && !isNaN(idFromPath) ? idFromPath : null);

        const cargarDatos = async () => {
            if (!clienteId) return;
            try {
                const response = await fetch(`/Pagos/ObtenerDatosCuentaCorrienteJson?clienteId=${clienteId}`);
                if (response.ok) {
                    const data = await response.json();
                    cliente.value = data.cliente;
                    pagos.value = data.pagos || [];
                }
            } catch (e) {
                console.error("Error al cargar datos de cuenta corriente:", e);
            }
        };

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
                    await cargarDatos();
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
            cargarDatos();
        });

        return {
            cliente,
            pagos,
            procesando,
            pagoAConfirmar,
            abrirModalCobro,
            confirmarYProcesarCobro,
            obtenerClaseEstado
        };
    }
}).mount('#app-pagos');