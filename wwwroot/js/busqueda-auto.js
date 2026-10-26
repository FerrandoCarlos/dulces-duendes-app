(() => {
  const CLAVE = 'dd-foco-busqueda';
  const ESPERA = 400;

  document.querySelectorAll('form[data-busqueda-auto]').forEach((form) => {
    const input = form.querySelector('input[type="search"]');
    if (!input) return;

    if (sessionStorage.getItem(CLAVE) === input.name) {
      sessionStorage.removeItem(CLAVE);
      input.focus();
      input.setSelectionRange(input.value.length, input.value.length);
    }

    form.addEventListener('submit', () => {
      sessionStorage.setItem(CLAVE, input.name);
    });

    let valorEnviado = input.value.trim();
    let temporizador;

    input.addEventListener('input', () => {
      clearTimeout(temporizador);
      temporizador = setTimeout(() => {
        if (input.value.trim() === valorEnviado) return;
        valorEnviado = input.value.trim();
        form.requestSubmit();
      }, ESPERA);
    });
  });
})();
