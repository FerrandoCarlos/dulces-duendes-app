const archivo = document.getElementById('avatar-archivo');
const nombre = document.getElementById('avatar-nombre');
const enviar = document.getElementById('avatar-enviar');

archivo.addEventListener('change', () => {
  const elegido = archivo.files[0];
  nombre.textContent = elegido ? elegido.name : 'Elegir imagen';
  enviar.disabled = !elegido;
});
