namespace FirmaDocs;

public sealed partial class CircuitoFirma
{
    private readonly List<DelegacionFirma> _delegaciones = [];

    /// <summary>Delegaciones registradas para este circuito.</summary>
    public IReadOnlyList<DelegacionFirma> Delegaciones => _delegaciones.AsReadOnly();

    /// <summary>Firmantes y delegados habilitados en el instante indicado para la etapa actual.</summary>
    /// <param name="instante">Instante que se usa para evaluar la vigencia de las delegaciones.</param>
    /// <returns>Ejecutores habilitados, sin repetir identificadores.</returns>
    public IReadOnlyList<Firmante> FirmantesHabilitadosEn(DateTimeOffset instante)
    {
        if (Estado != EstadoDocumento.EnCircuito || IndiceEtapaActual is not { } indiceEtapa)
            return Array.Empty<Firmante>();

        var habilitados = FirmantesHabilitados.ToList();
        foreach (var delegacion in _delegaciones)
        {
            if (delegacion.Etapas.Contains(indiceEtapa) &&
                delegacion.EstaVigente(instante) &&
                habilitados.Any(firmante => firmante.Id == delegacion.FirmanteOriginal.Id) &&
                !habilitados.Any(firmante => firmante.Id == delegacion.Delegado.Id))
            {
                habilitados.Add(delegacion.Delegado);
            }
        }

        return Array.AsReadOnly(habilitados.ToArray());
    }

    /// <summary>Registra una delegación para un firmante y etapas de este circuito.</summary>
    /// <param name="delegacion">Autorización temporal que se desea conservar.</param>
    /// <exception cref="CircuitoFirmaException">El firmante o las etapas no pertenecen al circuito, o existe una autorización solapada.</exception>
    public void RegistrarDelegacion(DelegacionFirma delegacion)
    {
        ArgumentNullException.ThrowIfNull(delegacion);

        if (!_firmantes.Any(firmante => firmante.Id == delegacion.FirmanteOriginal.Id))
            throw new CircuitoFirmaException("El firmante original de la delegación no pertenece al circuito.");

        foreach (var numeroEtapa in delegacion.Etapas)
        {
            if (numeroEtapa > _etapas.Count ||
                !_etapas[numeroEtapa - 1].Firmantes.Any(firmante => firmante.Id == delegacion.FirmanteOriginal.Id))
                throw new CircuitoFirmaException("El alcance de la delegación no contiene una etapa del firmante original.");
        }

        var tieneSolapamiento = _delegaciones.Any(registrada =>
            registrada.FirmanteOriginal.Id == delegacion.FirmanteOriginal.Id &&
            registrada.Etapas.Intersect(delegacion.Etapas).Any() &&
            registrada.VigenteDesde < delegacion.VigenteHasta &&
            delegacion.VigenteDesde < registrada.VigenteHasta);
        if (tieneSolapamiento)
            throw new CircuitoFirmaException("La delegación se solapa con otra autorización del firmante y etapa.");

        _delegaciones.Add(delegacion);
    }

    /// <summary>Registra la aprobación de un firmante mediante una delegación vigente.</summary>
    /// <param name="firmanteOriginal">Firmante habilitado que delegó su aprobación.</param>
    /// <param name="delegado">Persona que ejecuta la aprobación.</param>
    /// <param name="instante">Momento de la aprobación.</param>
    /// <exception cref="CircuitoFirmaException">El firmante no está habilitado o no existe una delegación vigente y aplicable.</exception>
    public void AprobarPorDelegacion(Firmante firmanteOriginal, Firmante delegado, DateTimeOffset instante)
    {
        ArgumentNullException.ThrowIfNull(firmanteOriginal);
        ArgumentNullException.ThrowIfNull(delegado);

        if (Estado != EstadoDocumento.EnCircuito)
            throw new CircuitoFirmaException("El documento no está en circuito de firma.");
        if (!FirmantesHabilitados.Any(habilitado => habilitado.Id == firmanteOriginal.Id))
            throw new CircuitoFirmaException($"No es el turno de {firmanteOriginal.Nombre}.");
        if (IndiceEtapaActual is not { } indiceEtapa)
            throw new CircuitoFirmaException("No hay una etapa habilitada para aprobar.");

        var autorizacion = _delegaciones.FirstOrDefault(delegacion =>
            delegacion.FirmanteOriginal.Id == firmanteOriginal.Id &&
            delegacion.Delegado.Id == delegado.Id &&
            delegacion.Etapas.Contains(indiceEtapa) &&
            delegacion.EstaVigente(instante));
        if (autorizacion is null)
            throw new CircuitoFirmaException("El delegado no tiene una autorización vigente para esta etapa.");

        RegistrarAprobacion(firmanteOriginal, instante, null, autorizacion.Delegado);
    }

    private int? IndiceEtapaActual
    {
        get
        {
            var etapaActual = EtapaActual;
            if (etapaActual is null)
                return null;

            for (var indice = 0; indice < _etapas.Count; indice++)
            {
                if (ReferenceEquals(_etapas[indice], etapaActual))
                    return indice + 1;
            }

            return null;
        }
    }
}
