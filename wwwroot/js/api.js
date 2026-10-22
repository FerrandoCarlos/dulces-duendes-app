async function apiFetch(url, options) {
  const resp = await fetch(url, options);
  if (resp.status === 401) {
    window.location.href = '/Account/Login';
    throw new Error('Sesión vencida');
  }
  return resp;
}
