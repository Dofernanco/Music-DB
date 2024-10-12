# Music-DB
Fernando Colón Acosta

Proyecto 2 - Modelado y Programacion

## Descripción

**Music DB** es una aplicación que permite extraer automáticamente información de archivos MP3 (como el título de la canción, artista, álbum, etc.) utilizando etiquetas **ID3v2.4** y almacenarla en una base de datos **SQLite**. Además, ofrece una interfaz gráfica de usuario (GUI) construida con **GTK#** que permite gestionar la información de las canciones, realizar búsquedas, y editar las canciones y álbumes.

La aplicación está diseñada para:
- **Minar archivos MP3** en un directorio y sus subdirectorios.
- **Almacenar la información** de las canciones en una base de datos SQLite según el esquema definido.
- **Visualizar canciones** directamente desde la interfaz gráfica.
- **Buscar canciones** por título, artista o álbum.

## Características

- **Extracción de etiquetas ID3**: Extrae automáticamente las etiquetas de los archivos MP3 usando **TagLib#**.
- **Base de datos SQLite**: Utiliza SQLite para almacenar la información de las canciones, intérpretes y álbumes.
- **Interfaz gráfica (GTK#)**: Proporciona una interfaz gráfica para interactuar con la base de datos de música.
- **Funcionalidades de gestión**:
  - Minar directorios completos de archivos MP3.
  - Visualizar y la información de las canciones.
  - Búsqueda por nombre de canción, artista o álbum.
  - Barra de progreso durante la minería de archivos.

