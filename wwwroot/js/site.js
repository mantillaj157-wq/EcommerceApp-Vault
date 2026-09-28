// Función para limpiar el texto (elimina tildes, mayúsculas y signos)
function limpiarTextoVoz(texto) {
    return texto
        .toLowerCase()
        .trim()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .replace(/[.,\/#!$%\^&\*;:{}=\-_`~()]/g, "");
}

// Mapeo de comandos de voz con las rutas reales de tus controladores de C#
const diccionarioRutas = [
    // INICIO
    { palabras: ["inicio", "home", "ir a inicio", "pagina principal"], url: "/" },

    // PRODUCTOS Y CATEGORÍAS
    { palabras: ["productos", "producto", "ir a productos", "catalogo"], url: "/Products" },
    { palabras: ["arriba", "partes de arriba"], url: "/Products?category=Arriba" },
    { palabras: ["abajo", "partes de abajo"], url: "/Products?category=Abajo" },
    { palabras: ["accesorios", "accesorio"], url: "/Products?category=Accesorios" },

    // TALLES Y PREGUNTAS (Controlador Home)
    { palabras: ["talles", "talle", "guia de talles"], url: "/Home/Talles" },
    { palabras: ["preguntas", "pregunta", "preguntas frecuentes", "ayuda"], url: "/Home/Preguntas" },

    // SESIÓN Y REGISTRO (Controlador Account)
    { palabras: ["iniciar sesion", "ingresar", "login", "entrar"], url: "/Account/Login" },
    { palabras: ["crear cuenta", "registrarse", "registro", "cuenta nueva"], url: "/Account/Register" },

    // CARRITO
    { palabras: ["carrito", "ir al carrito", "ver carrito", "compras"], url: "/Cart" } // Ajusta si tu ruta del carrito se llama distinto
];

// Función principal que activa el micrófono
function activarReconocimientoVoz() {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

    if (!SpeechRecognition) {
        alert("Tu navegador no soporta el reconocimiento de voz por API.");
        return;
    }

    const recognition = new SpeechRecognition();
    recognition.lang = 'es-ES'; // Idioma Español
    recognition.interimResults = false;
    recognition.continuous = false;

    const btnMic = document.getElementById("btnMicrofonoFlotante");

    recognition.onstart = function () {
        console.log("Escuchando comando de voz...");
        if (btnMic) {
            btnMic.classList.add("listening-active"); // Usa la animación CSS que ya tienes definida
        }
    };

    recognition.onresult = function (event) {
        const textoEscuchado = event.results[0][0].transcript;
        const comandoLimpio = limpiarTextoVoz(textoEscuchado);

        console.log("Escuchado:", textoEscuchado, "-> Procesado:", comandoLimpio);

        let urlDestino = null;

        // Buscar coincidencia en el diccionario
        for (const item of diccionarioRutas) {
            if (item.palabras.some(palabra => comandoLimpio.includes(palabra))) {
                urlDestino = item.url;
                break;
            }
        }

        if (urlDestino) {
            window.location.href = urlDestino; // Redirige a la ruta seleccionada
        } else {
            alert(`No reconozco el comando: "${textoEscuchado}". Intenta decir "Inicio", "Talles", "Productos", "Iniciar sesión" o "Carrito".`);
        }
    };

    recognition.onerror = function (event) {
        console.error("Error en Web Speech API:", event.error);
        if (btnMic) {
            btnMic.classList.remove("listening-active");
        }
    };

    recognition.onend = function () {
        if (btnMic) {
            btnMic.classList.remove("listening-active");
        }
    };

    recognition.start();
}