using System;
namespace Consommateur.Kafka.Modeles
{
    public interface IMagasinConfigBanque
    {
        string NomCollection { get; set; }
        string ChaineConnexion { get; set; }
        string NomBanque { get; set; }
    }
}
