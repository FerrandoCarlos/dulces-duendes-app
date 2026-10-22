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
    const tipoFiltro = ref(null);
    let temporizador;
    let numeroPedido = 0;

    const estados = [
      { valor: 'Todos', texto: 'Todos' },
      { valor: 'ConStock', texto: 'Con stock' },
      { valor: 'StockBajo', texto: 'Stock bajo' },
      { valor: 'SinStock', texto: 'Sin stock' },
    ];

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
        if (tipoFiltro.value)
          params.set('tipoProductoId', String(tipoFiltro.value.id));

        const resp = await apiFetch(`/ajax/productos?${params}`);
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

    watch([estado, tipoFiltro], () => {
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
      tipoFiltro,
      estados,
      formatoPrecio,
      irAPagina,
    };
  },
})
  .component('selector-tipo', SelectorTipo)
  .mount('#app-productos');
