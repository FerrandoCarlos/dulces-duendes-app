using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class AlmacenArchivos : IAlmacenArchivos
    {
        private const long MaxBytes = 2 * 1024 * 1024;
        private const string Prefijo = "/uploads/";

        private readonly string _raiz;

        public AlmacenArchivos(IWebHostEnvironment env)
        {
            _raiz = Path.GetFullPath(Path.Combine(env.WebRootPath, "uploads"));
        }

        public async Task<string> GuardarImagen(Stream contenido, string carpeta)
        {
            if (contenido.Length == 0 || contenido.Length > MaxBytes) throw new AppException("La imagen debe pesar hasta 2 MB");

            var extension = DetectarExtension(contenido) ?? throw new AppException("El archivo debe se una imagen .JPG, .PNG o .WebP.");

            var nombre = $"{Guid.NewGuid():N}{extension}";
            var directorio = Path.Combine(_raiz, carpeta);
            Directory.CreateDirectory(directorio);

            contenido.Position = 0;
            await using var destino = File.Create(Path.Combine(directorio, nombre));
            await contenido.CopyToAsync(destino);

            return $"{Prefijo}{carpeta}/{nombre}";
        }

        public void Eliminar(string? urlRelativa)
        {
            if (string.IsNullOrWhiteSpace(urlRelativa) || !urlRelativa.StartsWith(Prefijo)) return;

            var relativa = urlRelativa[Prefijo.Length..].Replace('/', Path.DirectorySeparatorChar);
            var ruta = Path.GetFullPath(Path.Combine(_raiz, relativa));

            if (!ruta.StartsWith(_raiz + Path.DirectorySeparatorChar)) return;

            if (File.Exists(ruta)) File.Delete(ruta);
        }

        private static string? DetectarExtension(Stream s)
        {
            Span<byte> c = stackalloc byte[12];
            s.Position = 0;

            if (s.ReadAtLeast(c, 12, throwOnEndOfStream: false) < 12) return null;

            if (c[0] == 0xFF && c[1] == 0xD8 && c[2] == 0xFF) return ".jpg";
            if (c[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return ".png";
            if (c[..4].SequenceEqual("RIFF"u8) && c[8..12].SequenceEqual("WEBP"u8)) return ".webp";
            return null;
        }
    }
}
