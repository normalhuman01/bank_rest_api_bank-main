## BankRestAPI

Esta es una API REST básica para realizar transferencias bancarias. Se ha desarrollado utilizando ASP.NET Core y Entity Framework Core. 
La API tiene la capacidad de recibir solicitudes HTTP con datos de entidades y guardarlas en una base de datos.

## Requerimientos

- Visual Studio 2019 o superior.
- NET Core 6.0 o superior.
- PostgreSQL

## Instrucciones de uso

- Descargar o clonar el repositorio.
```bash
    git clone https://github.com/JuanMendozaOS/BankRestAPI.git
```
- Abrir el proyecto en Visual Studio.
- En el archivo appsettings.json, modificar la cadena de conexión  con los datos de su servidor de base de datos.

```json
    "ConnectionStrings": {
        "BankAPIConnectionString": "Host=<hostname>; Database=<name>; Username=<username>; Password=<password>"
    }
```
- Abrir el Package Manager Console (**Tools** > **NuGet Package Manager** > **Package Manager Console**) y ejecutar el siguiente comando: 
```bash
    Update-Database
```
- Ejecutar el proyecto
```bash
    dotnet run
```

- Visitar https://localhost:[puerto]/swagger/index.html para conocer los endpoints y sus requerimientos para realizar las peticiones.

## Estructura del proyecto

```markdown
├───BankRestAPI
│   ├───Controllers
│   ├───Data
│   ├───Migrations
│   ├───DTOs
│   ├───Models
│   ├───Services

```
## Screenshots
<img src="https://github.com/JuanMendozaOS/BankRestAPI/blob/main/Screenshots/CreateAccount.png" width="512">

<img src="https://github.com/JuanMendozaOS/BankRestAPI/blob/main/Screenshots/CreateTransfer.png" width="512">

<img src="https://github.com/JuanMendozaOS/BankRestAPI/blob/main/Screenshots/GetAccountByNumber.png" width="512">


## Autores

- [@JuanMendozaOS](https://www.github.com/JuanMendozaOS)
