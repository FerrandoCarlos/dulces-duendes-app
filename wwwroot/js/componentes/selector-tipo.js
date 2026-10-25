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
             :placeholder="placeholder"
             @focus="alEnfocar" @input="alEscribir" @blur="alSalir" />
      <div v-if="abierto" class="list-group position-absolute w-100 shadow dd-selector-lista">
        <button v-if="permitirTodos" type="button"
                class="list-group-item list-group-item-action"
                @mousedown.prevent="elegir(null)">Todos los tipos</button>
        <button v-for="t in resultados" :key="t.id" type="button"
                class="list-group-item list-group-item-action"
                :class="{ active: modelValue && modelValue.id === t.id }"
                @mousedown.prevent="elegir(t)">{{ t.tipo }}</button>
        <div v-if="!cargando && resultados.length === 0" class="list-group-item text-muted">
          Sin resultados
        </div>
      </div>
    </div>`,
  setup(props, { emit }) {
    const { ref, watch } = Vue;
    const texto = ref(props.modelValue?.tipo ?? '');
    const resultados = ref([]);
    const abierto = ref(false);
    const cargando = ref(false);
    let temporizador;
    let numeroPedido = 0;

    watch(
      () => props.modelValue,
      (v) => {
        texto.value = v?.tipo ?? '';
      }
    );

    const buscar = async (termino) => {
      const pedido = ++numeroPedido;
      cargando.value = true;
      try {
        const params = new URLSearchParams();
        if (termino.trim()) params.set('texto', termino.trim());
        const resp = await apiFetch(`/ajax/tipos-producto?${params}`);
        if (!resp.ok) throw new Error();
        const datos = await resp.json();
        if (pedido === numeroPedido) resultados.value = datos;
      } catch {
        if (pedido === numeroPedido) resultados.value = [];
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

    const alSalir = () => {
      abierto.value = false;
      clearTimeout(temporizador);
      texto.value = props.modelValue?.tipo ?? '';
    };

    const elegir = (tipo) => {
      emit('update:modelValue', tipo);
      texto.value = tipo?.tipo ?? '';
      abierto.value = false;
    };

    return {
      texto,
      resultados,
      abierto,
      cargando,
      alEnfocar,
      alEscribir,
      alSalir,
      elegir,
    };
  },
};
