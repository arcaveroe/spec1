namespace FirmaDocs;

/// <summary>Configuración inmutable de una etapa del circuito de firma.</summary>
public sealed class EtapaFirma
{
    private readonly IReadOnlyList<Firmante> _firmantes;

    /// <summary>Crea una etapa con un modo y sus firmantes.</summary>
    /// <param name="modo">Modo de aprobación de la etapa.</param>
    /// <param name="firmantes">Firmantes de la etapa, en orden de configuración.</param>
    /// <exception cref="CircuitoFirmaException">La etapa tiene un modo o cantidad de firmantes no admitidos.</exception>
    public EtapaFirma(ModoEtapaFirma modo, IReadOnlyList<Firmante> firmantes)
    {
        ArgumentNullException.ThrowIfNull(firmantes);

        if (modo is not (ModoEtapaFirma.Secuencial or ModoEtapaFirma.Paralela))
            throw new CircuitoFirmaException("El modo de la etapa no está admitido.");
        if (firmantes.Count is < 1 or > 5)
            throw new CircuitoFirmaException("Cada etapa debe tener entre uno y cinco firmantes.");
        if (firmantes.Any(firmante => firmante is null))
            throw new CircuitoFirmaException("La etapa no puede contener firmantes nulos.");

        Modo = modo;
        _firmantes = Array.AsReadOnly(firmantes.ToArray());
    }

    /// <summary>Modo de aprobación de la etapa.</summary>
    public ModoEtapaFirma Modo { get; }

    /// <summary>Firmantes de la etapa en orden de configuración.</summary>
    public IReadOnlyList<Firmante> Firmantes => _firmantes;
}
