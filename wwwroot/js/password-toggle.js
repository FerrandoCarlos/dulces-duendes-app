document.querySelectorAll('input[type="password"]').forEach((input) => {
  const wrapper = document.createElement('div');
  wrapper.className = 'password-wrapper';
  input.parentNode.insertBefore(wrapper, input);
  wrapper.appendChild(input);

  const boton = document.createElement('button');
  boton.type = 'button';
  boton.className = 'password-toggle-btn d-none';
  boton.setAttribute('aria-label', 'Mostrar contraseña');
  boton.setAttribute('aria-pressed', 'false');
  boton.innerHTML = '<i class="bi bi-eye-slash"></i>';
  wrapper.appendChild(boton);

  input.addEventListener('input', () => {
    if (input.value.length > 0) {
      boton.classList.remove('d-none');
    } else {
      boton.classList.add('d-none');
      input.type = 'password';
      boton.setAttribute('aria-pressed', 'false');
      boton.firstElementChild.className = 'bi bi-eye';
    }
  });

  boton.addEventListener('click', () => {
    const visible = input.type === 'password';
    input.type = visible ? 'text' : 'password';

    boton.setAttribute('aria-pressed', String(visible));
    boton.setAttribute(
      'aria-label',
      visible ? 'Ocultar contraseña' : 'Mostrar contraseña'
    );

    boton.firstElementChild.className = visible
      ? 'bi bi-eye'
      : 'bi bi-eye-slash';
  });
});
