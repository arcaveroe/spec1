namespace FirmaDocs.Tests.Circuito;

public class VersionesEtapasTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Reenviar_FirmasAnterioresSeConservanYNoCompletanEtapaVigente_RF08()
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
        circuito.Rechazar(Marta, "Revisar el documento.", Ahora.AddMinutes(2));
        circuito.Reenviar("autor");

        circuito.Aprobar(Ana, Ahora.AddDays(1));

        Assert.Equal([Luis, Marta], circuito.FirmantesHabilitados);
        Assert.Equal(
            [
                new Firma(Ana, 1, Ahora),
                new Firma(Luis, 1, Ahora.AddMinutes(1)),
                new Firma(Ana, 2, Ahora.AddDays(1))
            ],
            circuito.Firmas);
    }
}
