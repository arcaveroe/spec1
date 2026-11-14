namespace FirmaDocs.Tests.Circuito;

public class DelegacionHabilitacionTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Delegada = new("eva", "Eva");
    private static readonly DateTimeOffset Desde = new(2026, 11, 14, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Hasta = Desde.AddHours(2);

    [Fact]
    public void RegistrarDelegacion_OriginalFueraDeAlcance_RechazaSinCambios_RFDF01()
    {
        EtapaFirma[] etapas = [new(ModoEtapaFirma.Secuencial, [Ana]), new(ModoEtapaFirma.Secuencial, [Luis])];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        var delegacion = new DelegacionFirma(Ana, Delegada, [2], Desde, Hasta);

        Assert.Throws<CircuitoFirmaException>(() => circuito.RegistrarDelegacion(delegacion));

        Assert.Empty(circuito.Delegaciones);
    }

    [Fact]
    public void Delegado_EnInicioPeroNoEnFinDeVigencia_EsHabilitadoSoloEnInicio_RFDF02()
    {
        var circuito = CrearCircuito([new EtapaFirma(ModoEtapaFirma.Secuencial, [Ana])]);
        circuito.RegistrarDelegacion(new DelegacionFirma(Ana, Delegada, [1], Desde, Hasta));
        circuito.Enviar();

        Assert.Equal([Ana, Delegada], circuito.FirmantesHabilitadosEn(Desde));
        Assert.Equal([Ana], circuito.FirmantesHabilitadosEn(Hasta));
    }

    [Fact]
    public void Delegado_AlcanceDeOtraEtapa_NoEstaHabilitadoEnEtapaActual_RFDF02()
    {
        EtapaFirma[] etapas = [new(ModoEtapaFirma.Secuencial, [Ana]), new(ModoEtapaFirma.Secuencial, [Luis])];
        var circuito = CrearCircuito(etapas);
        circuito.RegistrarDelegacion(new DelegacionFirma(Luis, Delegada, [2], Desde, Hasta));
        circuito.Enviar();

        Assert.Equal([Ana], circuito.FirmantesHabilitadosEn(Desde));

        circuito.Aprobar(Ana, Desde);

        Assert.Equal([Luis, Delegada], circuito.FirmantesHabilitadosEn(Desde));
    }

    [Fact]
    public void RegistrarDelegacion_PeriodosSolapadosParaMismoAlcance_RechazaSinCambios_RFDF01()
    {
        var circuito = CrearCircuito([new EtapaFirma(ModoEtapaFirma.Secuencial, [Ana])]);
        circuito.RegistrarDelegacion(new DelegacionFirma(Ana, Delegada, [1], Desde, Hasta));
        var otraDelegada = new Firmante("ines", "Inés");

        Assert.Throws<CircuitoFirmaException>(() =>
            circuito.RegistrarDelegacion(new DelegacionFirma(Ana, otraDelegada, [1], Hasta.AddMinutes(-1), Hasta.AddHours(1))));

        Assert.Single(circuito.Delegaciones);
    }

    private static CircuitoFirma CrearCircuito(IReadOnlyList<EtapaFirma> etapas) =>
        new("DOC-001", "autor", etapas);
}
