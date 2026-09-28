const CACHE_NAME = 'vault-cache-v1';
const urlsToCache = [
    '/',
    '/css/site.css',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/images/icon-192.png',
    '/images/icon-512.png'
];

// Instalación del Service Worker y almacenamiento inicial en caché
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(urlsToCache);
        })
    );
});

// Activación del Service Worker
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cache => {
                    if (cache !== CACHE_NAME) {
                        return caches.delete(cache);
                    }
                })
            );
        })
    );
});

// Intercepción de peticiones de red
self.addEventListener('fetch', event => {
    event.respondWith(
        fetch(event.request)
            .then(response => {
                return response;
            })
            .catch(() => {
                return caches.match(event.request);
            })
    );
});