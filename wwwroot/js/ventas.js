const { createApp, ref, computed, onMounted } = Vue;

createApp({
    setup() {
        const detalles = ref([]);
        const busquedaCodigo = ref('');
        const error = ref('');
        const inputCodigo = ref(null);
        const tipoPago = ref('Efectivo');
        const clienteSeleccionado = ref(null);
        const errorClienteModal = ref('');
        const guardando = ref(false);
        let modalCobroInstance = null;

        const totalEfectivo = computed(() => {
            return detalles.value.reduce((acc, item) => acc + (item.cantidad * (item.precioVenta || 0)), 0);
        });

        const totalCuentaCorriente = computed(() => {
            return detalles.value.reduce((acc, item) => acc + (item.cantidad * (item.precioCuentaCorriente || 0)), 0);
        });

        const agregarProducto = (prod) => {
            if (prod.stock <= 0) {
                error.value = `El producto "${prod.descripcion || prod.nombre}" no cuenta con stock disponible.`;
                return;
            }
            const index = detalles.value.findIndex(d => d.productoId === (prod.id));

            if (index !== -1) {
                if (detalles.value[index].cantidad + 1 > detalles.value[index].stock) {
                    error.value = `No puedes agregar más unidades. El stock máximo disponible para "${detalles.value[index].descripcion}" es ${detalles.value[index].stock}.`;
                    return;
                }
                detalles.value[index].cantidad++;
            } else {
                detalles.value.push({
                    productoId: prod.id,
                    codigoBarra: prod.codigoBarra,
                    descripcion: prod.descripcion,
                    stock: prod.stock ?? 0,
                    cantidad: 1,
                    precioVenta: Number(prod.precioVenta ?? 0),
                    precioCuentaCorriente: Number(prod.precioCuentaCorriente ?? 0)
                });
            }
        };

        const buscarPorCodigo = async () => {
            const codigo = busquedaCodigo.value.trim();
            if (!codigo) return;
            error.value = '';
            try {
                const response = await fetch(`/Productos/ObtenerPorCodigoJson?codigo=${encodeURIComponent(codigo)}`);
                if (response.ok) {
                    const prod = await response.json();
                    if (prod) {
                        agregarProducto(prod);
                        busquedaCodigo.value = '';
                    } else {
                        error.value = "Producto no encontrado";
                    }
                } else {
                    error.value = "El producto no existe";
                }
            } catch (e) {
                console.error("Error al buscar el producto ", e);
                error.value = "Error de conexión al consultar el producto.";
            } finally {
                if (inputCodigo.value) inputCodigo.value.focus();
            }
        };

        const validarCantidad = (index) => {
            const item = detalles.value[index];

            if (!item.cantidad || item.cantidad <= 0 || isNaN(item.cantidad)) {
                item.cantidad = 1;
                return;
            }

            if (item.cantidad > item.stock) {
                item.cantidad = item.stock;
                error.value = `Se ajustó la cantidad al máximo disponible (${item.stock}) para "${item.descripcion}".`;
            } else {
                error.value = '';
            }
        };

        const quitarProducto = (index) => {
            detalles.value.splice(index, 1);
        };

        const limpiarTicket = () => {
            detalles.value = [];
            error.value = '';
        };

        const abrirModalCobro = () => {
            if (detalles.value.length === 0) return;
            tipoPago.value = 'Efectivo';
            clienteSeleccionado.value = null;
            errorClienteModal.value = '';
            if (modalCobroInstance) modalCobroInstance.show();
        };

        const cambiarTipoPago = () => {
            if (tipoPago.value === 'CuentaCorriente') {
                setTimeout(() => {
                    inicializarSelect2Cliente();
                }, 100);
            } else {
                clienteSeleccionado.value = null;
            }
        };

        const inicializarSelect2Cliente = () => {
            const select = $('#selectCliente');

            if (select.hasClass('select2-hidden-accessible')) {
                select.val(null).trigger('change');
                return;
            }

            select.select2({
                theme: 'bootstrap-5',
                dropdownParent: $('#modalCobro'),
                placeholder: 'Buscar cliente por Nombre o Apellido...',
                allowClear: true,
                minimumInputLength: 2,
                ajax: {
                    url: '/Clientes/BuscarClientesJson',
                    dataType: 'json',
                    delay: 300,
                    data: function (params) {
                        return { busqueda: params.term };
                    },
                    processResults: function (data) {
                        return {
                            results: data.map(c => ({
                                id: c.id || c.Id,
                                text: `${c.nombre} ${c.apellido}`,
                                clienteOriginal: c
                            }))
                        };
                    }
                }
            });

            select.on('select2:select', function (e) {
                clienteSeleccionado.value = e.params.data.clienteOriginal;
                errorClienteModal.value = '';
            });

            select.on('select2:clear', function () {
                clienteSeleccionado.value = null;
            });
        };

        const guardarTicket = async () => {
            if (tipoPago.value === 'CuentaCorriente' && !clienteSeleccionado.value) {
                errorClienteModal.value = 'Debe seleccionar un cliente.';
                return;
            }

            guardando.value = true;

            const ticketDTO = {
                id: 0,
                tipoPago: tipoPago.value,
                clienteId: clienteSeleccionado.value ? (clienteSeleccionado.value.id || clienteSeleccionado.value.Id) : null,
                detalles: detalles.value.map(d => ({
                    productoId: d.productoId,
                    cantidad: d.cantidad,
                    precioUnitario: tipoPago.value === 'Efectivo' ? d.precioVenta : d.precioCuentaCorriente
                }))
            };

            try {
                const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenInput ? tokenInput.value : '';

                const response = await fetch('/Tickets/Guardar', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(ticketDTO)
                });

                const result = await response.json().catch(() => null);

                if (response.ok && result && result.success) {
                    if (modalCobroInstance) modalCobroInstance.hide();
                    limpiarTicket();
                } else {
                    const mensajeError = result?.message || `Error en el servidor (Código ${response.status})`;
                }
            } catch (e) {
                console.error('Error procesando la venta:', e);
            } finally {
                guardando.value = false;
            }
        };

        onMounted(() => {
            const modalEl = document.getElementById('modalCobro');
            if (modalEl) modalCobroInstance = new bootstrap.Modal(modalEl);
            if (inputCodigo.value) inputCodigo.value.focus();
        });

        return {
            detalles,
            busquedaCodigo,
            error,
            inputCodigo,
            tipoPago,
            clienteSeleccionado,
            errorClienteModal,
            guardando,
            totalEfectivo,
            totalCuentaCorriente,
            buscarPorCodigo,
            validarCantidad,
            quitarProducto,
            limpiarTicket,
            abrirModalCobro,
            cambiarTipoPago,
            guardarTicket
        };
    }
}).mount("#app-tickets");