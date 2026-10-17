const reloj = document.getElementById('reloj');

if(reloj){
    const actualizar = () =>{
        reloj.textContent = new Date().toLocaleTimeString('es-Ar',{
            hour: '2-digit',
            minute: '2-digit',
            hourCycle:'h23'
        });
    };
    actualizar();
    setInterval(actualizar, 1000);
}
