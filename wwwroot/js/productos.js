const { createApp, ref, computed, onMounted, watch } = Vue;

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

    // ---- Formulario de alta y edición ----
    const modalEl = ref(null);
    let modal;
    const formVacio = () => ({
      id: null,
      codigoBarra: '',
      descripcion: '',
      tipo: null,
      stock: 0,
      precioCosto: 0,
      porcentajeGanancia: 0,
    });
    const form = ref(formVacio());
    const errorGeneral = ref('');
    const erroresCampo = ref({});
    const guardando = ref(false);
    const esEdicion = computed(() => form.value.id !== null);

    const limpiarErrores = () => {
      errorGeneral.value = '';
      erroresCampo.value = {};
    };

    const abrirNuevo = () => {
      form.value = formVacio();
      limpiarErrores();
      limpiarImagenes();
      modal.show();
    };

    const abrirEditar = (p) => {
      form.value = {
        id: p.id,
        codigoBarra: p.codigoBarra,
        descripcion: p.descripcion,
        tipo: { id: p.tipoProductoId, tipo: p.tipoProductoNombre },
        stock: p.stock,
        precioCosto: p.precioCosto,
        porcentajeGanancia: p.porcentajeGanancia,
      };
      limpiarErrores();
      limpiarImagenes();
      cargarImagenes(p.id);
      modal.show();
    };

    const normalizarErrores = (errors) =>
      Object.fromEntries(
        Object.entries(errors).map(([campo, mensajes]) => [
          campo.charAt(0).toLowerCase() + campo.slice(1),
          mensajes[0],
        ])
      );

    const guardar = async () => {
      if (guardando.value) return;
      guardando.value = true;
      limpiarErrores();
      try {
        const f = form.value;
        const cuerpo = {
          codigoBarra: f.codigoBarra.trim(),
          descripcion: f.descripcion.trim(),
          tipoProductoId: f.tipo?.id ?? 0,
          stock: Number(f.stock),
          precioCosto: Number(f.precioCosto),
          porcentajeGanancia: Number(f.porcentajeGanancia),
        };
        const resp = await apiFetch(
          esEdicion.value ? `/ajax/productos/${f.id}` : '/ajax/productos',
          {
            method: esEdicion.value ? 'PUT' : 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cuerpo),
          }
        );

        if (resp.ok) {
          imagenesModificadas = false;
          modal.hide();
          await cargar();
          return;
        }
        if (resp.status === 404) {
          errorGeneral.value = 'El producto ya no existe.';
          return;
        }

        const datos = await resp.json().catch(() => null);
        if (datos?.mensaje) errorGeneral.value = datos.mensaje;
        else if (datos?.errors)
          erroresCampo.value = normalizarErrores(datos.errors);
        else errorGeneral.value = 'No se pudo guardar.';
      } catch {
        errorGeneral.value = 'No se pudo guardar.';
      } finally {
        guardando.value = false;
      }
    };

    // ---- Baja ----
    const modalBajaEl = ref(null);
    let modalBaja;
    const aBorrar = ref(null);
    const errorBaja = ref('');
    const borrando = ref(false);

    const abrirBaja = (p) => {
      aBorrar.value = p;
      errorBaja.value = '';
      modalBaja.show();
    };

    const confirmarBaja = async () => {
      if (borrando.value || !aBorrar.value) return;
      borrando.value = true;
      errorBaja.value = '';
      try {
        const resp = await apiFetch(`/ajax/productos/${aBorrar.value.id}`, {
          method: 'DELETE',
        });

        // 404 significa que ya no existía: se refresca igual.
        if (!resp.ok && resp.status !== 404) {
          const datos = await resp.json().catch(() => null);
          errorBaja.value = datos?.mensaje ?? 'No se pudo eliminar.';
          return;
        }

        // Si era el único de la página, se vuelve a la anterior.
        if (items.value.length === 1 && pagina.value > 1) pagina.value--;
        modalBaja.hide();
        await cargar();
      } catch {
        errorBaja.value = 'No se pudo eliminar.';
      } finally {
        borrando.value = false;
      }
    };

    // ---- Imágenes ----
    const maxImagenes = 5;
    const imagenes = ref([]);
    const imagenesCargando = ref(false);
    const subiendo = ref(false);
    const errorImagenes = ref('');
    const imagenAConfirmar = ref(null);
    const inputImagenes = ref(null);

    let pedidoImagenes = 0;
    let imagenesModificadas = false;

    const urlImagenes = (productoId) =>
      `/ajax/productos/${productoId}/imagenes`;

    const limpiarImagenes = () => {
      pedidoImagenes++;
      imagenes.value = [];
      imagenesCargando.value = false;
      errorImagenes.value = '';
      imagenAConfirmar.value = null;
    };

    const cargarImagenes = async (productoId) => {
      const pedido = ++pedidoImagenes;
      imagenesCargando.value = true;
      try {
        const resp = await apiFetch(urlImagenes(productoId));
        if (!resp.ok) throw new Error();
        const datos = await resp.json();
        if (pedido === pedidoImagenes) imagenes.value = datos;
      } catch {
        if (pedido === pedidoImagenes)
          errorImagenes.value = 'No se pudieron cargar las imágenes.';
      } finally {
        if (pedido === pedidoImagenes) imagenesCargando.value = false;
      }
    };

    const subirImagenes = async (evento) => {
      const archivos = Array.from(evento.target.files ?? []);
      if (archivos.length === 0 || subiendo.value) return;

      const productoId = form.value.id;
      subiendo.value = true;
      errorImagenes.value = '';
      try {
        const cuerpo = new FormData();
        archivos.forEach((a) => cuerpo.append('archivos', a));

        const resp = await apiFetch(urlImagenes(productoId), {
          method: 'POST',
          body: cuerpo,
        });

        if (resp.ok) {
          const nuevas = await resp.json();
          if (form.value.id === productoId) imagenes.value.push(...nuevas);
          imagenesModificadas = true;
          return;
        }
        if (resp.status === 404) {
          errorImagenes.value = 'El producto ya no existe.';
          return;
        }
        if (resp.status === 413) {
          errorImagenes.value = 'Las imágenes superan el tamaño permitido.';
          return;
        }
        const datos = await resp.json().catch(() => null);
        errorImagenes.value =
          datos?.mensaje ?? 'No se pudieron subir las imágenes.';
      } catch {
        errorImagenes.value = 'No se pudieron subir las imágenes.';
      } finally {
        subiendo.value = false;
        if (inputImagenes.value) inputImagenes.value.value = '';
      }
    };

    const eliminarImagen = async (imagen) => {
      const productoId = form.value.id;
      errorImagenes.value = '';
      try {
        const resp = await apiFetch(`${urlImagenes(productoId)}/${imagen.id}`, {
          method: 'DELETE',
        });
        if (!resp.ok && resp.status !== 404) throw new Error();
        imagenes.value = imagenes.value.filter((i) => i.id !== imagen.id);
        imagenesModificadas = true;
      } catch {
        errorImagenes.value = 'No se pudo eliminar la imagen.';
      } finally {
        imagenAConfirmar.value = null;
      }
    };

    // ---- Visor de imágenes ----
    const modalImagenesEl = ref(null);
    let modalImagenes;
    const productoVisor = ref(null);

    const abrirImagenes = (p) => {
      productoVisor.value = p;
      modalImagenes.show();
    };

    // ---- Ciclo de vida ----
    onMounted(() => {
      cargar();
      modal = new bootstrap.Modal(modalEl.value);
      modalBaja = new bootstrap.Modal(modalBajaEl.value);
      modalImagenes = new bootstrap.Modal(modalImagenesEl.value);

      modalEl.value.addEventListener('hidden.bs.modal', () => {
        if (!imagenesModificadas) return;
        imagenesModificadas = false;
        cargar();
      });
    });

    // ---- Observadores Reactivos ----
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
      modalEl,
      form,
      esEdicion,
      errorGeneral,
      erroresCampo,
      guardando,
      abrirNuevo,
      abrirEditar,
      guardar,
      modalBajaEl,
      aBorrar,
      errorBaja,
      borrando,
      abrirBaja,
      confirmarBaja,
      maxImagenes,
      imagenes,
      imagenesCargando,
      subiendo,
      errorImagenes,
      imagenAConfirmar,
      inputImagenes,
      subirImagenes,
      eliminarImagen,
      modalImagenesEl,
      productoVisor,
      abrirImagenes,
    };
  },
})
  .component('selector-tipo', SelectorTipo)
  .mount('#app-productos');
