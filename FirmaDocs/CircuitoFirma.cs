namespace FirmaDocs;

/// <summary>
/// Circuito de firma secuencial: los firmantes aprueban uno tras otro,
/// en el orden en que se definieron.
/// </summary>
public sealed class CircuitoFirma
{
    private readonly IReadOnlyList<Firmante> _firmantes;
    private readonly IReadOnlyList<EtapaFirma> _etapas;
    private readonly List<Firma> _firmas = [];
    private readonly List<Rechazo> _rechazos = [];
    private const int MaxVueltas = 3;

    /// <summary>Crea el circuito de un documento en estado <see cref="EstadoDocumento.Borrador"/>.</summary>
    /// <param name="documentoId">Identificador del documento.</param>
    /// <param name="autorId">Identificador del autor del documento.</param>
    /// <param name="firmantes">Firmantes en orden de firma; al menos uno y sin repetidos.</param>
    /// <exception cref="CircuitoFirmaException">La cantidad de firmantes no está admitida o hay repetidos.</exception>
    public CircuitoFirma(string documentoId, string autorId, IReadOnlyList<Firmante> firmantes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentoId);
        ArgumentException.ThrowIfNullOrWhiteSpace(autorId);
        ArgumentNullException.ThrowIfNull(firmantes);

        if (firmantes.Count == 0)
            throw new CircuitoFirmaException("El circuito necesita al menos un firmante.");
        if (firmantes.Any(firmante => firmante is null))
            throw new CircuitoFirmaException("El circuito no puede contener firmantes nulos.");
        if (firmantes.Select(f => f.Id).Distinct().Count() != firmantes.Count)
            throw new CircuitoFirmaException("Un firmante no puede repetirse en el circuito.");

        DocumentoId = documentoId;
        AutorId = autorId;
        var copiaFirmantes = firmantes.ToArray();
        _firmantes = Array.AsReadOnly(copiaFirmantes);
        _etapas = Array.AsReadOnly(new[] { new EtapaFirma(ModoEtapaFirma.Secuencial, copiaFirmantes) });
    }

    /// <summary>Crea el circuito a partir de etapas secuenciales o paralelas.</summary>
    /// <param name="documentoId">Identificador del documento.</param>
    /// <param name="autorId">Identificador del autor del documento.</param>
    /// <param name="etapas">Etapas del circuito en orden de aprobación.</param>
    /// <exception cref="CircuitoFirmaException">La cantidad de etapas no está entre uno y cinco o hay firmantes repetidos.</exception>
    public CircuitoFirma(string documentoId, string autorId, IReadOnlyList<EtapaFirma> etapas)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentoId);
        ArgumentException.ThrowIfNullOrWhiteSpace(autorId);
        ArgumentNullException.ThrowIfNull(etapas);

        if (etapas.Count is < 1 or > 5)
            throw new CircuitoFirmaException("El circuito debe tener entre una y cinco etapas.");
        if (etapas.Any(etapa => etapa is null))
            throw new CircuitoFirmaException("El circuito no puede contener etapas nulas.");

        var copiaEtapas = etapas.ToArray();
        var copiaFirmantes = copiaEtapas.SelectMany(etapa => etapa.Firmantes).ToArray();
        if (copiaFirmantes.Select(firmante => firmante.Id).Distinct().Count() != copiaFirmantes.Length)
            throw new CircuitoFirmaException("Un firmante no puede repetirse en el circuito.");

        DocumentoId = documentoId;
        AutorId = autorId;
        _etapas = Array.AsReadOnly(copiaEtapas);
        _firmantes = Array.AsReadOnly(copiaFirmantes);
    }

    /// <summary>Identificador del documento.</summary>
    public string DocumentoId { get; }

    /// <summary>Identificador del autor.</summary>
    public string AutorId { get; }

    /// <summary>Versión vigente del documento. Empieza en 1.</summary>
    public int Version { get; private set; } = 1;

    /// <summary>Estado actual del documento.</summary>
    public EstadoDocumento Estado { get; private set; } = EstadoDocumento.Borrador;

    /// <summary>Firmantes en orden de firma.</summary>
    public IReadOnlyList<Firmante> Firmantes => _firmantes;

    /// <summary>Etapas de aprobación en el orden configurado.</summary>
    public IReadOnlyList<EtapaFirma> Etapas => _etapas;

    /// <summary>Historial de aprobaciones, en orden cronológico.</summary>
    public IReadOnlyList<Firma> Firmas => _firmas;

    /// <summary>Historial de rechazos, en orden cronológico.</summary>
    public IReadOnlyList<Rechazo> Rechazos => _rechazos;

    /// <summary>Firmantes que pueden aprobar en la etapa vigente.</summary>
    public IReadOnlyList<Firmante> FirmantesHabilitados
    {
        get
        {
            if (Estado != EstadoDocumento.EnCircuito || EtapaActual is not { } etapa)
                return Array.Empty<Firmante>();

            var aprobados = FirmantesAprobadosEnVersionVigente();
            var pendientes = etapa.Firmantes
                .Where(firmante => !aprobados.Contains(firmante.Id))
                .ToArray();

            return etapa.Modo == ModoEtapaFirma.Secuencial
                ? Array.AsReadOnly(new[] { pendientes[0] })
                : Array.AsReadOnly(pendientes);
        }
    }

    /// <summary>Firmante que debe aprobar ahora; <c>null</c> si no existe un turno secuencial único.</summary>
    public Firmante? FirmanteEnTurno =>
        Estado == EstadoDocumento.EnCircuito && EtapaActual is { Modo: ModoEtapaFirma.Secuencial }
            ? FirmantesHabilitados[0]
            : null;

    /// <summary>Envía el documento a firma.</summary>
    /// <exception cref="CircuitoFirmaException">El documento no está en borrador.</exception>
    public void Enviar()
    {
        if (Estado != EstadoDocumento.Borrador)
            throw new CircuitoFirmaException("Solo se puede enviar a firma un documento en borrador.");

        Estado = EstadoDocumento.EnCircuito;
    }

    /// <summary>Registra la aprobación de un firmante habilitado.</summary>
    /// <param name="firmante">Firmante que aprueba.</param>
    /// <param name="instante">Momento de la aprobación.</param>
    /// <exception cref="CircuitoFirmaException">El documento no está en circuito o el firmante no está habilitado.</exception>
    public void Aprobar(Firmante firmante, DateTimeOffset instante)
    {
        ArgumentNullException.ThrowIfNull(firmante);

        if (Estado != EstadoDocumento.EnCircuito)
            throw new CircuitoFirmaException("El documento no está en circuito de firma.");
        if (!FirmantesHabilitados.Any(habilitado => habilitado.Id == firmante.Id))
            throw new CircuitoFirmaException($"No es el turno de {firmante.Nombre}.");

        _firmas.Add(new Firma(firmante, Version, instante));

        if (EtapaActual is null)
            Estado = EstadoDocumento.Aprobado;
    }

    /// <summary>Registra el rechazo del firmante en turno.</summary>
    /// <param name="firmante">Firmante que rechaza.</param>
    /// <param name="motivo">Motivo del rechazo.</param>
    /// <param name="instante">Momento del rechazo.</param>
    /// <exception cref="CircuitoFirmaException">El documento no está en circuito, el firmante no está en turno o el motivo está vacío.</exception>
    public void Rechazar(Firmante firmante, string motivo, DateTimeOffset instante)
    {
        ArgumentNullException.ThrowIfNull(firmante);

        if (Estado != EstadoDocumento.EnCircuito)
            throw new CircuitoFirmaException("Solo se puede rechazar un documento en circuito de firma.");
        if (FirmanteEnTurno!.Id != firmante.Id)
            throw new CircuitoFirmaException($"No es el turno de {firmante.Nombre}.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new CircuitoFirmaException("El motivo de rechazo es obligatorio.");

        _rechazos.Add(new Rechazo(firmante, motivo, Version, instante));
        Estado = Version == MaxVueltas ? EstadoDocumento.Rechazado : EstadoDocumento.Devuelto;
    }

    /// <summary>Reenvía al circuito un documento devuelto por su autor.</summary>
    /// <param name="solicitanteId">Identificador de quien solicita el reenvío.</param>
    /// <exception cref="CircuitoFirmaException">El solicitante no es el autor o el documento no está devuelto.</exception>
    public void Reenviar(string solicitanteId)
    {
        if (Estado != EstadoDocumento.Devuelto)
            throw new CircuitoFirmaException("Solo se puede reenviar un documento devuelto.");
        if (solicitanteId != AutorId)
            throw new CircuitoFirmaException("Solo el autor puede reenviar el documento.");

        Version++;
        Estado = EstadoDocumento.EnCircuito;
    }

    private EtapaFirma? EtapaActual
    {
        get
        {
            var aprobados = FirmantesAprobadosEnVersionVigente();
            return _etapas.FirstOrDefault(etapa =>
                etapa.Firmantes.Any(firmante => !aprobados.Contains(firmante.Id)));
        }
    }

    private HashSet<string> FirmantesAprobadosEnVersionVigente() =>
        _firmas
            .Where(firma => firma.Version == Version)
            .Select(firma => firma.Firmante.Id)
            .ToHashSet(StringComparer.Ordinal);
}
