// ISIRUN · JavaScript mínimo. La lógica de negocio va en C# (controladores/servicios), NO acá.

// Diálogos de confirmación: <button data-dialog-open="idDelDialog"> abre el <dialog id="idDelDialog">,
// y cualquier <button data-dialog-close> dentro del diálogo lo cierra.
document.addEventListener('click', (e) => {
    const opener = e.target.closest('[data-dialog-open]');
    if (opener) {
        document.getElementById(opener.dataset.dialogOpen)?.showModal();
        return;
    }
    const closer = e.target.closest('[data-dialog-close]');
    if (closer) closer.closest('dialog')?.close();
});

// Registro: muestra los campos del tipo de cuenta elegido (radio name="Tipo").
// Los campos ocultos se deshabilitan para que no se envíen ni se validen.
function syncTipoCuenta() {
    const checked = document.querySelector('input[name="Tipo"]:checked');
    if (!checked) return;
    document.querySelectorAll('[data-tipo]').forEach((el) => {
        const on = el.dataset.tipo === checked.value;
        el.hidden = !on;
        el.querySelectorAll('input, select, textarea').forEach((i) => { i.disabled = !on; });
    });
}
document.addEventListener('change', (e) => {
    if (e.target.matches('input[name="Tipo"]')) syncTipoCuenta();
});
syncTipoCuenta();
