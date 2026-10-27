# ISIRUN Web: inventario de pantallas

DER actualizado y explicación de los cambios: [DER_actualizado.md](DER_actualizado.md)
Implementación: **ASP.NET Core MVC + Entity Framework Core**, según la rúbrica. Cada pantalla corresponde a una vista `.cshtml` con su acción en un controlador.

Base: diagrama de clases (Corredor, Equipo, Carrera, Categoria_edad, Resultados, Organizacion, Org_carrera, Sponsor)
más lo que ya hacía la app WPF (registro/login, actividades, amigos, likes).
**Sin IA**: no hay chatbot, generador de rutinas ni rutinas.

## Dos tipos de cuenta
Al registrarse se elige **Corredor** u **Organización**. El login es uno solo y lleva al panel que corresponde.

## Ciclo de vida de una carrera
1. **Abierta**: la organización la crea. Los corredores se inscriben, y cada inscripción crea un registro en Resultados con tiempo y ranking vacíos ("Inscrito · pendiente").
2. **Terminada**: la organización aprieta "Terminar carrera" y confirma. Se cierran las inscripciones y el sistema simula el tiempo de cada inscrito:
   ritmo aleatorio entre **4:00 y 8:00 min/km** × distancia. Después ordena por tiempo dentro de cada categoría y guarda el ranking.
   En una versión real los tiempos vendrían del chip de cronometraje.
- En una carrera terminada no se puede inscribir nadie, y en una abierta no se muestra el ranking.
- Hace falta un campo de estado en Carrera (`Abierta` / `Terminada`), que no está en el diagrama.

## Pantallas
### Públicas
1. **Inicio**: hero, próximas carreras, cómo funciona y llamado para organizaciones.
2. **Login**: correo + contraseña.
3. **Registro**: elegir el tipo de cuenta. Corredor: nombre, fecha de nacimiento y equipo. Organización: nombre y actividad. Ambos: correo y contraseña.

### Corredor
4. **Mi perfil**: stats, equipo, inscripciones pendientes, resultados y últimas actividades.
5. **Registrar actividad**.
6. **Feed**: actividades de amigos con likes.
7. **Carreras**: pestañas *Abiertas* y *Terminadas*, más filtro por distancia.
8. **Carrera abierta**: info, categorías y premios, organizadores, sponsors y botón **Inscribirme**.
9. **Carrera terminada**: tu resultado, podio y ranking por categoría con tiempo y ritmo. No permite inscribirse.
10. **Equipos** + crear equipo.
11. **Detalle de equipo**: miembros, kms y botón para unirse o salir.
12. **Organizaciones**: solo lectura.
13. **Amigos**: buscar, agregar y quitar.
14. **Perfil de amigo**.

### Organización
15. **Mis carreras**: abiertas (con número de inscritos y botón Gestionar) y terminadas.
16. **Gestionar carrera**: inscritos por categoría, editar, sponsors y **Terminar carrera** (con confirmación, simulación y ranking).
17. **Crear / editar carrera**: datos, tamaño del equipo organizador y categorías por edad.
18. **Sponsors y co-organizadores**: vincular otra organización registrada como co-organizadora (fecha de inicio y tamaño del equipo) o como sponsor (monto y tipo).
19. **Perfil de la organización**: nombre y actividad.
20. **Reporte 1: Inscripciones y recaudación**: filtros por fechas y estado. Muestra total de carreras, inscritos, recaudación y promedio por carrera, una tabla por carrera desglosada por categoría con fila de totales, y barras de inscritos por categoría.
21. **Reporte 2: Rendimiento por categoría y equipo**: filtro por carrera terminada y opción de agrupar por categoría o por equipo. Muestra finalistas, mejor tiempo, tiempo promedio y ritmo promedio, y una tabla con podios por grupo y fila de totales.

## Eliminar (CRUD)
- **Carrera**: desde Gestionar carrera, solo si está abierta, con confirmación. Borra en cascada categorías, inscripciones y vínculos.
- **Categoría**: quitar una fila en Crear / editar carrera.
- **Sponsor o co-organizador**: "Quitar" en la lista de vinculadas, con confirmación. La organización creadora no se puede quitar.
- **Actividad**: "Eliminar" en Mi perfil, con confirmación (resta sus km).
- **Inscripción**: "Cancelar inscripción" en una carrera abierta.
- **Amigo**: "Quitar" en Amigos.

## Elementos comunes
- Barra del corredor: Mi perfil · Feed · Carreras · Equipos · Organizaciones · Amigos · avatar · Salir.
- Barra de la organización: Mis carreras · Crear carrera · Reportes · Perfil de la organización · Salir.
- Estados vacíos, mensajes de éxito y error, y diseño responsive para celular.
