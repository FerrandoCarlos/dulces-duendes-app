(() => {
  const modalEl = document.getElementById('modalConfirmar');
  if (!modalEl) return;

  const modal = new bootstrap.Modal(modalEl);
  const cabecera = modalEl.querySelector('.modal-header');
  const cerrar = modalEl.querySelector('.btn-close');
  const titulo = document.getElementById('modalConfirmarTitulo');
  const cuerpo = document.getElementById('modalConfirmarMensaje');
  const aceptar = document.getElementById('modalConfirmarAceptar');
  let formPendiente = null;

  document.addEventListener('submit', (e) => {
    const form = e.target;
    const mensaje = form.dataset?.confirmar;
    if (!mensaje) return;

    e.preventDefault();
    formPendiente = form;

    const peligro = form.dataset.confirmarTipo !== 'primario';
    titulo.textContent = form.dataset.confirmarTitulo ?? 'Confirmar';
    cuerpo.textContent = mensaje;
    aceptar.textContent = form.dataset.confirmarBoton ?? 'Aceptar';
    aceptar.className = peligro ? 'btn btn-danger-dd' : 'btn btn-primary';
    cabecera.classList.toggle('is-danger', peligro);
    cerrar.classList.toggle('btn-close-white', peligro);

    modal.show();
  });

  aceptar.addEventListener('click', () => {
    const form = formPendiente;
    formPendiente = null;
    modal.hide();
    form?.submit();
  });

  modalEl.addEventListener('hidden.bs.modal', () => {
    formPendiente = null;
  });
})();
