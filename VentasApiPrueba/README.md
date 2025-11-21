# Ventas API Prueba Tecnica

Descripcion del proyecto

Este proyecto es una API REST para gestionar ventas, clientes, productos, categorias y usuarios, proporciona endpoints para autenticacion, registro de usuarios, CRUD de entidades principales y gestion de ventas con detalles.

La API usa JWT para autenticacion y se integra con Entity Framework Core para acceso a base de datos SQL Server.

Tecnologias utilizadas

- .NET 10 (C#)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper
- Swashbuckle (Swagger)
- Azure.Identity
- Herramientas: dotnet CLI, SSMS 

Archivo de configuracion

- `appsettings.json` contiene la cadena de conexion y la configuracion de JWT.


Como correr, ojo debe estar dentro de la carpeta \VentasApiPrueba

1. Restaurar paquetes y compilar:

   dotnet restore
   Luego

    dotnet build

2. Ejecutar:

   dotnet run

3. Abrir Swagger en:

   http://localhost:5072


