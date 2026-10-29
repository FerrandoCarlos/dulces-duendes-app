let contadorSelectores = 0;
const TODOS = { id: null, tipo: 'Todos los tipos' };

const SelectorTipo = {
  props: {
    modelValue: { type: Object, default: null },
    permitirTodos: { type: Boolean, default: false },
    placeholder: { type: String, default: 'Buscar tipo...' },
  },
  emits: ['update:modelValue'],
  template: `
    <div class="position-relative">
      <input type="text" class="form-control" v-model="texto" autocomplete="off"
             role="combobox" aria-autocomplete="list"
             :aria-expanded="abierto" :aria-controls="idLista"
             :aria-activedescendant="indice >= 0 ? idOpcion(indice) : null"
             :placeholder="placeholder"
             @focus="alEnfocar" @input="alEscribir" @blur="cerrar" @keydown="alTeclear" />
      <div v-if="abierto" :id="idLista" role="listbox"
           class="list-group position-absolute w-100 shadow dd-selector-lista">
        <button v-for="(op, i) in opciones" :key="op.id ?? 'todos'" :id="idOpcion(i)"
                type="button" tabindex="-1" role="option" :aria-selected="esSeleccionada(op)"
                class="list-group-item list-group-item-action"
                :class="{ active: i === indice, 'fw-bold': esSeleccionada(op) }"
                @mousedown.prevent="elegir(op)" @mousemove="indice = i">{{ op.tipo }}</button>
        <div v-if="!cargando && opciones.length === 0" class="list-group-item text-muted">
          Sin resultados
        </div>
      </div>
    </div>`,
  setup(props, { emit }) {
    const { ref, computed, watch, nextTick } = Vue;

    const uid = `selector-tipo-${++contadorSelectores}`;
    const idLista = `${uid}-lista`;
    const idOpcion = (i) => `${uid}-op-${i}`;

    const texto = ref(props.modelValue?.tipo ?? '');
    const resultados = ref([]);
    const filtro = ref('');
    const abierto = ref(false);
    const cargando = ref(false);
    const indice = ref(-1);
    let temporizador;
    let numeroPedido = 0;

    // "Todos los tipos" solo se ofrece cuando no se está filtrando por texto.
    const opciones = computed(() =>
      props.permitirTodos && filtro.value.trim() === ''
        ? [TODOS, ...resultados.value]
        : resultados.value
    );

    const esSeleccionada = (op) => op.id === (props.modelValue?.id ?? null);

    watch(
      () => props.modelValue,
      (v) => {
        texto.value = v?.tipo ?? '';
      }
    );

    // Cuando cambian las opciones, el cursor vuelve a la primera.
    watch(opciones, () => {
      indice.value = opciones.value.length ? 0 : -1;
    });

    const buscar = async (termino) => {
      const pedido = ++numeroPedido;
      cargando.value = true;
      try {
        const params = new URLSearchParams();
        if (termino.trim()) params.set('texto', termino.trim());
        const resp = await apiFetch(`/ajax/tipos-producto?${params}`);
        if (!resp.ok) throw new Error();
        const datos = await resp.json();
        if (pedido === numeroPedido) {
          resultados.value = datos;
          filtro.value = termino;
        }
      } catch {
        if (pedido === numeroPedido) {
          resultados.value = [];
          filtro.value = termino;
        }
      } finally {
        if (pedido === numeroPedido) cargando.value = false;
      }
    };

    const alEnfocar = (e) => {
      abierto.value = true;
      e.target.select();
      buscar('');
    };

    const alEscribir = () => {
      abierto.value = true;
      clearTimeout(temporizador);
      temporizador = setTimeout(() => buscar(texto.value), 400);
    };

    const cerrar = () => {
      abierto.value = false;
      clearTimeout(temporizador);
      texto.value = props.modelValue?.tipo ?? '';
    };

    const elegir = (op) => {
      emit('update:modelValue', op.id === null ? null : op);
      texto.value = op.id === null ? '' : op.tipo;
      abierto.value = false;
    };

    const moverCursor = async (delta) => {
      const total = opciones.value.length;
      if (!total) return;
      indice.value =
        indice.value < 0
          ? delta > 0
            ? 0
            : total - 1
          : (indice.value + delta + total) % total; // circular
      await nextTick();
      document
        .getElementById(idOpcion(indice.value))
        ?.scrollIntoView({ block: 'nearest' });
    };

    const alTeclear = (e) => {
      switch (e.key) {
        case 'ArrowDown':
          e.preventDefault();
          if (abierto.value) moverCursor(1);
          else {
            abierto.value = true;
            buscar('');
          }
          break;
        case 'ArrowUp':
          e.preventDefault();
          if (abierto.value) moverCursor(-1);
          break;
        case 'Enter':
          // Con la lista abierta, Enter elige y no envía el formulario del modal.
          if (abierto.value) {
            e.preventDefault();
            const op = opciones.value[indice.value];
            if (op) elegir(op);
          }
          break;
        case 'Escape':
          // Primero cierra la lista; recién el siguiente Escape cierra el modal.
          if (abierto.value) {
            e.preventDefault();
            e.stopPropagation();
            cerrar();
          }
          break;
      }
    };

    return {
      texto,
      opciones,
      abierto,
      cargando,
      indice,
      idLista,
      idOpcion,
      esSeleccionada,
      alEnfocar,
      alEscribir,
      cerrar,
      elegir,
      alTeclear,
    };
  },
};
