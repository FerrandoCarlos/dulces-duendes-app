const { createApp, ref, onMounted } = Vue;

createApp({
  setup() {
    const total = ref('cargando...');

    onMounted(async () => {
      const resp = await fetch('/ajax/productos?page=1');
      const datos = await resp.json();
      total.value = datos.totalCount;
    });

    return { total };
  },
}).mount('#app-productos');
