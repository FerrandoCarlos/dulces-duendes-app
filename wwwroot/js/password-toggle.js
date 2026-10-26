document.querySelectorAll('input[type="password"]').forEach((input) => {
  const wrapper = document.createElement('div');
  wrapper.className = 'password-wrapper';
  input.parentNode.insertBefore(wrapper, input);
  wrapper.appendChild(input);

  const boton = document.createElement('button');
  boton.type = 'button';
  boton.className = 'password-toggle-btn d-none';
  boton.innerHTML = '<i class="bi"></i>';
  wrapper.appendChild(boton);

  const icono = boton.firstElementChild;

  // Único lugar donde se decide cómo se ve el botón: depende solo de input.type.
  const actualizar = () => {
    const visible = input.type === 'text';
    icono.className = visible ? 'bi bi-eye' : 'bi bi-eye-slash';
    boton.setAttribute('aria-pressed', String(visible));
    boton.setAttribute(
      'aria-label',
      visible ? 'Ocultar contraseña' : 'Mostrar contraseña'
    );
  };

  // Campo vacío: sin botón y siempre oculto.
  const sincronizar = () => {
    if (input.value.length === 0) {
      input.type = 'password';
      boton.classList.add('d-none');
    } else {
      boton.classList.remove('d-none');
    }
    actualizar();
  };

  input.addEventListener('input', sincronizar);

  boton.addEventListener('click', () => {
    input.type = input.type === 'password' ? 'text' : 'password';
    actualizar();
    input.focus();
  });

  sincronizar();
});
