# POSNEX API (.NET)

Backend independiente del frontend Angular. La solución está organizada por capas:

- `Posnex.Api`: endpoints REST, autenticación, validación HTTP, CORS, rate limiting y Swagger.
- `Posnex.BLL`: casos de uso y reglas del negocio.
- `Posnex.DAL`: Entity Framework Core, PostgreSQL y repositorios.
- `Posnex.Entities`: entidades y enumeraciones del dominio.

## Requisitos

- .NET SDK 10.
- PostgreSQL y una base de datos destino vacía o revisada para recibir las tablas nuevas.

## Configuración local

Copia el ejemplo a `Posnex.Api/appsettings.Development.json` y coloca ahí los datos de PostgreSQL para tu máquina. Ese archivo está excluido de Git para no publicar la contraseña:

```powershell
Copy-Item Posnex.Api/appsettings.Development.json.example Posnex.Api/appsettings.Development.json
```

Completa el valor `Password` con la contraseña de PostgreSQL de tu entorno. No compartas la contraseña ni la agregues a Git. En producción se debe inyectar la cadena desde el gestor de secretos del entorno, no usar este archivo.

La configuración CORS de desarrollo permite Angular en `http://localhost:4200` y `http://127.0.0.1:4200`. En producción configura `Cors__AllowedOrigins__0` y `AllowedHosts` con los dominios reales.

## Migración inicial

La migración `InitialCreate` agrega tablas de ASP.NET Identity, `businesses` y `business_memberships`. No borra tablas existentes. Revisa el nombre correcto de la base antes de aplicarla:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet tool run dotnet-ef database update --project Posnex.DAL --startup-project Posnex.Api
```

El nombre que aparece bajo `Database=` debe ser el nombre de una base real. En PostgreSQL el servidor registrado en pgAdmin, la base de datos y las tablas son objetos distintos. La migración agrega tablas de Identity, `businesses` y `business_memberships`; no reutiliza una tabla preexistente llamada `posnex`.

## Ejecutar

```powershell
dotnet run --project Posnex.Api
```

En desarrollo, Swagger queda en `http://localhost:5071/swagger`.

## Rutas

La autenticación de ASP.NET Core Identity está bajo `/api/v1/auth` (`/register`, `/login`, `/refresh`). Los endpoints de negocio requieren el token bearer de `/login`.

| Método | Ruta | Acción |
| --- | --- | --- |
| GET | `/api/v1/businesses` | Lista negocios del usuario autenticado |
| POST | `/api/v1/businesses` | Crea negocio de tipo `Ecommerce` o `Restaurant` |
| GET | `/api/v1/businesses/{id}` | Obtiene un negocio propio |
| PUT | `/api/v1/businesses/{id}` | Actualiza nombre, colores y logo |
| DELETE | `/api/v1/businesses/{id}` | Lo marca eliminado, sin borrar físicamente |

El tipo de negocio queda establecido al crearlo. Cada negocio nuevo se relaciona con el usuario autenticado como `Owner`. Las lecturas y escrituras se filtran por esa membresía, y un negocio eliminado se excluye de las consultas.

Los colores se validan como `#RRGGBB`. `LogoUrl` es opcional y por ahora acepta solo HTTPS. La carga directa de archivos de logo queda para una siguiente etapa.

## Seguridad inicial

- Contraseñas gestionadas y hasheadas por ASP.NET Core Identity. Temporalmente no hay requisitos de longitud ni complejidad; el campo sigue siendo obligatorio y se mantienen los límites de intentos.
- Autenticación bearer, rutas del negocio protegidas y filtradas por el propietario.
- Rate limit en endpoints de autenticación.
- CORS limitado a los orígenes del frontend configurados.
- Manejo uniforme de errores; los detalles internos no se devuelven al cliente.
- Secretos fuera de la configuración versionada.
- En desarrollo las claves de Data Protection se guardan en `Posnex.Api/.data-protection-keys/`, carpeta excluida de Git.

Antes de producción quedan por agregar verificación de email, almacenamiento durable de claves de Data Protection, roles/permisos completos, auditoría durable y políticas de despliegue/backup.
