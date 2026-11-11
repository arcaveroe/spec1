namespace FirmaDocs;

/// <summary>Modo de aprobación de una etapa del circuito de firma.</summary>
public enum ModoEtapaFirma
{
    /// <summary>Los firmantes aprueban en el orden configurado.</summary>
    Secuencial,

    /// <summary>Todos los firmantes pueden aprobar en cualquier orden.</summary>
    Paralela
}
