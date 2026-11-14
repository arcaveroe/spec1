using System.Security.Cryptography;
using System.Text;

namespace FirmaDocs.SelloTiempo;

/// <summary>Hash SHA-256 del contenido exacto de una versión, codificado en hexadecimal minúsculo.</summary>
/// <param name="Valor">Digest SHA-256 en hexadecimal minúsculo.</param>
public sealed record HashContenido(string Valor)
{
    /// <summary>Calcula el hash SHA-256 de los bytes UTF-8 exactos del contenido.</summary>
    /// <param name="contenido">Contenido sin normalización Unicode.</param>
    /// <returns>Hash SHA-256 codificado en hexadecimal minúsculo.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="contenido"/> es nulo.</exception>
    public static HashContenido Calcular(string contenido)
    {
        ArgumentNullException.ThrowIfNull(contenido);

        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(contenido));
        return new HashContenido(Convert.ToHexString(digest).ToLowerInvariant());
    }
}
