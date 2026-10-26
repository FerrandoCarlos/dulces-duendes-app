const { createApp, ref, computed, onMounted } = Vue;

createApp({
    setup() {
        const clientes = ref([]);
        const busqueda = ref('');
        const cargando = ref(true);
        const pagina = ref(1);
        const totalPaginas = ref(0);
        const error = ref('');
        const guardando = ref(false);
        const eliminando = ref(false);
        const editando = ref(false);
        const modalInstance = ref(null);
        const modalBajaInstance = ref(null);
        const clienteAEliminar = ref(null);

        const form = ref({
            id: 0,
            dni: '',
            nombre: '',
            apellido: '',
            telefono: '',
            direccion: ''
        });


        const getAntiForgeryToken = () => {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            return tokenInput ? tokenInput.value : '';
        };

        const cargarDatosIniciales = () => {
            const dataContainer = document.getElementById('clientes-data-json');
            if (dataContainer) {
                try {
                    const data = JSON.parse(dataContainer.textContent);
                    if (data && (data.items || data.Items)) {
                        clientes.value = data.items || data.Items || [];
                        pagina.value = data.page || data.Page || 1;
                        totalPaginas.value = data.totalPages || data.TotalPages || 1;
                    }
                } catch (e) {
                    console.error('Error al parsear los clientes iniciales:', e);
                }
            }
            cargando.value = false;
        };

        const buscarClientes = async (nroPagina = 1) => {
            cargando.value = true;
            error.value = '';
            try {
                const response = await fetch(`/Clientes/ObtenerPaginadoJson?busqueda=${encodeURIComponent(busqueda.value)}&paginaNro=${nroPagina}`);

                if (response.ok) {
                    const data = await response.json();
                    clientes.value = data.items || data.Items || [];
                    pagina.value = data.page || data.Page || 1;
                    totalPaginas.value = data.totalPages || data.TotalPages || 1;
                }
            } catch (e) {
                console.error('Error al consultar datos del servidor' , e);
                error.value = 'Error de conexión con el servidor';
            } finally {
                cargando.value = false;
            }
        }

        const irAPagina = (n) => {
            if (n < 1 || n > totalPaginas.value || n === pagina.value) return;
            buscarClientes(n);
        }

        const abrirModalCrear = () => {
            editando.value = false;
            error.value = '';
            form.value = { id: 0, dni: '', nombre: '', apellido: '', telefono: '', direccion: '' };
            if (modalInstance.value) modalInstance.value.show();
        };

        const abrirModalEditar = (cliente) => {
            editando.value = true;
            error.value = '';
            form.value = { ...cliente };
            if (modalInstance.value) modalInstance.value.show();
        };

        const guardarCliente = async () => {
            guardando.value = true;
            error.value = '';

            try {
                const response = await fetch('/Clientes/Guardar', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': getAntiForgeryToken()
                    },
                    body: JSON.stringify(form.value)
                });

                const resultado = await response.json();

                if (response.ok && resultado.success) {
                    if (modalInstance.value) modalInstance.value.hide();
                    await buscarClientes(pagina.value);
                } else {
                    error.value = resultado.message || 'No se pudo guardar el cliente';
                }
            } catch (e) {
                console.log('Error de conexión con el servidor', e);
            } finally {
                guardando.value = false;
            }
        };

        const pedirConfirmacionBaja = (cliente) => {
            error.value = '';
            clienteAEliminar.value = cliente;
            if (modalBajaInstance.value) modalBajaInstance.value.show();
        }

        const confirmarBaja = async () => {
            if (!clienteAEliminar.value) return;

            eliminando.value = true;
            error.value = '';

            try {
                const response = await fetch(`/Clientes/Eliminar/${clienteAEliminar.value.id}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': getAntiForgeryToken()
                    }
                });

                const resultado = await response.json();

                if (response.ok && resultado.success) {
                    if (modalBajaInstance.value) modalBajaInstance.value.hide();
                    await buscarClientes(pagina.value);
                } else {
                    error.value = resultado.message || 'No se pudo eliminar el cliente.';
                }
            } catch (e) {
                console.error('Error al intentar eliminar', e);
                error.value = 'Error de conexión al intentar eliminar.';
            } finally {
                eliminando.value = false;
                clienteAEliminar.value = null;
            }
        };

        onMounted(() => {
            cargarDatosIniciales();
            const elForm = document.getElementById('modalCliente');
            if (elForm) modalInstance.value = new bootstrap.Modal(elForm);
            const elBaja = document.getElementById('modalConfirmarBaja');
            if (elBaja) modalBajaInstance.value = new bootstrap.Modal(elBaja);
        });

        return {
            clientes,
            busqueda,
            cargando,
            pagina,
            totalPaginas,
            error,
            guardando,
            eliminando,
            editando,
            form,
            clienteAEliminar,
            buscarClientes,
            irAPagina,
            abrirModalCrear,
            abrirModalEditar,
            guardarCliente,
            pedirConfirmacionBaja,
            confirmarBaja,
        };
    }
}).mount('#app-clientes');