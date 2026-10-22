const { createApp, ref, onMounted, watch } = Vue;

createApp({
  setup() {
    const items = ref([]);
    const total = ref(0);
    const pagina = ref(1);
    const totalPaginas = ref(0);
    const cargando = ref(true);
    const error = ref('');
    const texto = ref('');
    const estado = ref('Todos');
    let temporizador;
    let numeroPedido = 0;

    const formatoPrecio = (n) =>
      n.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });

    const cargar = async () => {
      const pedido = ++numeroPedido;
      cargando.value = true;
      error.value = '';
      try {
        const params = new URLSearchParams({ page: String(pagina.value) });

        if (estado.value !== 'Todos') params.set('estado', estado.value);
        if (texto.value.trim()) params.set('texto', texto.value.trim());

        const resp = await fetch(`/ajax/productos?${params}`);
        if (pedido !== numeroPedido) return;

        if (resp.status === 401) {
          window.location.href = '/Account/Login';
          return;
        }
        if (!resp.ok) throw new Error();

        const datos = await resp.json();
        if (pedido !== numeroPedido) return;

        items.value = datos.items;
        total.value = datos.totalCount;
        totalPaginas.value = datos.totalPages;
      } catch {
        if (pedido === numeroPedido)
          error.value = 'No se pudo cargar el listado.';
      } finally {
        if (pedido === numeroPedido) cargando.value = false;
      }
    };

    const irAPagina = (n) => {
      if (n < 1 || n > totalPaginas.value || n === pagina.value) return;
      pagina.value = n;
      cargar();
    };

    onMounted(cargar);

    watch(texto, () => {
      clearTimeout(temporizador);
      temporizador = setTimeout(() => {
        pagina.value = 1;
        cargar();
      }, 400);
    });

    watch(estado, () => {
      clearTimeout(temporizador);
      pagina.value = 1;
      cargar();
    });

    return {
      items,
      total,
      pagina,
      totalPaginas,
      cargando,
      error,
      texto,
      estado,
      formatoPrecio,
      irAPagina,
    };
  },
}).mount('#app-productos');
