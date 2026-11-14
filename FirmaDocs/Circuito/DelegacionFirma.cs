namespace FirmaDocs;

/// <summary>Autorización temporal para que un delegado firme por un firmante en etapas concretas.</summary>
public sealed class DelegacionFirma
{
    private readonly IReadOnlyList<int> _etapas;

    /// <summary>Crea una delegación acotada por etapas y vigencia.</summary>
    /// <param name="firmanteOriginal">Firmante requerido en el circuito.</param>
    /// <param name="delegado">Persona autorizada para firmar en su nombre.</param>
    /// <param name="etapas">Números de etapa, empezando en uno, donde aplica la autorización.</param>
    /// <param name="vigenteDesde">Inicio inclusivo de la vigencia.</param>
    /// <param name="vigenteHasta">Fin exclusivo de la vigencia.</param>
    /// <exception cref="CircuitoFirmaException">Los firmantes, etapas o fechas no son válidos.</exception>
    public DelegacionFirma(
        Firmante firmanteOriginal,
        Firmante delegado,
        IReadOnlyCollection<int> etapas,
        DateTimeOffset vigenteDesde,
        DateTimeOffset vigenteHasta)
    {
        ArgumentNullException.ThrowIfNull(firmanteOriginal);
        ArgumentNullException.ThrowIfNull(delegado);
        ArgumentNullException.ThrowIfNull(etapas);

        if (string.IsNullOrWhiteSpace(firmanteOriginal.Id) ||
            string.IsNullOrWhiteSpace(delegado.Id) ||
            string.Equals(firmanteOriginal.Id, delegado.Id, StringComparison.Ordinal))
            throw new CircuitoFirmaException("La delegación necesita firmantes identificables y distintos.");

        var copiaEtapas = etapas.ToArray();
        if (copiaEtapas.Length == 0 ||
            copiaEtapas.Any(etapa => etapa < 1) ||
            copiaEtapas.Distinct().Count() != copiaEtapas.Length)
            throw new CircuitoFirmaException("La delegación debe indicar etapas válidas y sin repetir.");

        if (vigenteDesde >= vigenteHasta)
            throw new CircuitoFirmaException("La vigencia de la delegación debe tener un inicio anterior al fin.");

        FirmanteOriginal = firmanteOriginal;
        Delegado = delegado;
        _etapas = Array.AsReadOnly(copiaEtapas);
        VigenteDesde = vigenteDesde;
        VigenteHasta = vigenteHasta;
    }

    /// <summary>Firmante requerido cuya aprobación puede ejecutar el delegado.</summary>
    public Firmante FirmanteOriginal { get; }

    /// <summary>Persona autorizada para ejecutar la aprobación.</summary>
    public Firmante Delegado { get; }

    /// <summary>Números de etapa donde aplica la delegación, empezando en uno.</summary>
    public IReadOnlyList<int> Etapas => _etapas;

    /// <summary>Inicio inclusivo de la vigencia.</summary>
    public DateTimeOffset VigenteDesde { get; }

    /// <summary>Fin exclusivo de la vigencia.</summary>
    public DateTimeOffset VigenteHasta { get; }

    internal bool EstaVigente(DateTimeOffset instante) =>
        instante >= VigenteDesde && instante < VigenteHasta;
}
