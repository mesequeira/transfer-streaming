# Transfer Streaming
Transfer streaming es un sitio web que permite a los usuarios de distintos servicios de música transferir sus playlists entre plataformas.

## 1. Configuración

### Ingress

Deberás registrarte en [ngrok](https://ngrok.com/), el cual es un ingress gratuito que nos permitirá compartir subdominios entre el frontend y el backend, logrando así compartir cookies de autenticación.

Si es la primera vez que usamos `ngrok`, este utiliza un archivo de configuración llamado `ngrok.yml` que debe estar ubicado en el directorio predeterminado según tu sistema operativo:

- **Windows**:  
  El archivo debe estar en:

  ```bash
  C:\Users\<TU_USUARIO>\AppData\Local\ngrok
  ```

    - **Mac/Linux**:  
      El archivo debe estar en:

   ```bash
   ~/.ngrok2/ngrok.yml
   ```
Si el archivo no existe, existe la posibilidad de generarlo automáticamente ejecutando el siguiente comando, el cual lo creará en las direcciones antes mencionadas.

```bash
ngrok config add-authtoken YOUR_AUTH_TOKEN
```

Una vez abierto el archivo `ngrok.yml` deberemos configurar los address del frontend y backend.

```yml
version: "3"
agent:
    authtoken: YOUR_AUTH_TOKEN
tunnels:
  backend:
    addr: https://localhost:3000
    proto: http
  frontend:
    addr: 5500
    proto: http
```

Para poder levantar el ingress, una vez configurado el manifiesto, se deberá ejecutar el comando:

```bash
ngrok start --all
```