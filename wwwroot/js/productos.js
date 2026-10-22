const { createApp, ref, onMounted } = Vue;

createApp({
  setup() {
    const items = ref([]);
    const total = ref(0);
    const cargando = ref(true);
    const error = ref('');

    const formatoPrecio = (n) =>
      n.toLocaleString('es-Ar', { style: 'currency', currency: 'ARS' });

    const cargar = async () => {
      cargando.value = true;
      error.value = '';
      try {
        const resp = await fetch('/ajax/productos?page=1');
        if (resp.status === 401) {
          window.location.href = '/Account/Login';
          return;
        }
        if (!resp.ok) throw new Error();

        const datos = await resp.json();
        items.value = datos.items;
        total.value = datos.totalCount;
      } catch {
        error.value = 'No se pudo cargar el listado.';
      } finally {
        cargando.value = false;
      }
    };

    onMounted(cargar);
    return { items, total, cargando, error, formatoPrecio };
  },
}).mount('#app-productos');
