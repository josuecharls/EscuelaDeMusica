# Examen Técnico - Escuela de Música (.NET 8 + SQL Server)

Este repositorio contiene el desarrollo del examen técnico para el puesto de .NET Developer
El objetivo es implementar una API REST para la gestión de Escuelas de Música, incluyendo alumnos, profesores y relaciones entre ellos.

## Tecnologías que estoy utilizando

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core 9
- SQL Server
- Swagger
- Git y GitHub

# Cumplimiento del Examen Técnico
Este proyecto cumple con los requerimientos planteados en el examen:

## 1. Gestión de Escuelas, Profesores y Alumnos
- CRUD completo de Escuelas, Profesores y Alumnos
- Validación de campos obligatorios (nombre, apellido, identificación única (DNI), fecha de nacimiento)
- Profesores y alumnos están relacionados con una escuela

## 2. Asignación de Profesores a Alumnos
- Relación N:M entre alumnos y profesores
- Endpoint para asignar múltiples alumnos a un profesor
- Validación: un profesor solo puede pertenecer a una escuela

## 3. Inscripción de Alumnos a Escuelas
- Cada alumno se inscribe a una escuela mediante el campo EscuelaId
- Un alumno puede tener múltiples profesores

## 4. Consultas
- GET /api/profesores/{id}/alumnos: devuelve alumnos asignados a un profesor y su escuela
- GET /api/profesores/{id}/detalle: devuelve la escuela del profesor + todos los alumnos asignados

## 5. Uso de Stored Procedures
- Se han creado SPs para CRUD de Escuelas, Alumnos y Profesores
- La API usa directamente SPs en ProfesoresController y AlumnosController para insertar, actualizar, eliminar, y listar

- Adicional: Se mantiene también la opción de usar EF puro en EscuelasController como referencia

