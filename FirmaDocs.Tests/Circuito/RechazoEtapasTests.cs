namespace FirmaDocs.Tests.Circuito;

public class RechazoEtapasTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Rechazar_FirmanteHabilitadoEnEtapaParalela_DocumentoDevuelto_RF07()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Paralela, [Ana, Luis]),
            new(ModoEtapaFirma.Secuencial, [Marta])
        ];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();
        circuito.Aprobar(Ana, Ahora);

        circuito.Rechazar(Luis, "Revisar el documento.", Ahora.AddMinutes(1));

        Assert.Equal(EstadoDocumento.Devuelto, circuito.Estado);
        Assert.Equal(new Rechazo(Luis, "Revisar el documento.", 1, Ahora.AddMinutes(1)), circuito.Rechazos[0]);
    }

    [Fact]
    public void Rechazar_EtapaSecuencialVigente_SinFirmanteEnTurno_RF07()
    {
        EtapaFirma[] etapas = [new(ModoEtapaFirma.Secuencial, [Ana, Luis])];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        circuito.Rechazar(Ana, "Revisar el documento.", Ahora);

        Assert.Equal(EstadoDocumento.Devuelto, circuito.Estado);
        Assert.Null(circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Rechazar_DesdeEtapaPosterior_ReenvioReiniciaPrimeraEtapa_RF07()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Secuencial, [Ana]),
            new(ModoEtapaFirma.Paralela, [Luis, Marta])
        ];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();
        circuito.Aprobar(Ana, Ahora);
        circuito.Aprobar(Luis, Ahora.AddMinutes(1));

        circuito.Rechazar(Marta, "Revisar la etapa actual.", Ahora.AddMinutes(2));
        circuito.Reenviar("autor");

        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(2, circuito.Version);
        Assert.Equal(etapas, circuito.Etapas);
        Assert.Equal([Ana], circuito.FirmantesHabilitados);
    }

    [Fact]
    public void Rechazar_TerceraVuelta_CierraCircuitoYNoAdmiteReenvio_RF07()
    {
        EtapaFirma[] etapas = [new(ModoEtapaFirma.Paralela, [Ana, Luis])];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        circuito.Rechazar(Ana, "Primera corrección.", Ahora);
        circuito.Reenviar("autor");
        circuito.Rechazar(Luis, "Segunda corrección.", Ahora.AddDays(1));
        circuito.Reenviar("autor");
        circuito.Rechazar(Ana, "Última corrección.", Ahora.AddDays(2));

        Assert.Equal(3, circuito.Version);
        Assert.Equal(EstadoDocumento.Rechazado, circuito.Estado);
        Assert.Throws<CircuitoFirmaException>(() => circuito.Reenviar("autor"));
    }
}
