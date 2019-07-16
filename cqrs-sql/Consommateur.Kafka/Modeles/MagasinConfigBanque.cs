using System;
namespace Consommateur.Kafka.Modeles
{
    public class MagasinConfigBanque : IMagasinConfigBanque
    {
        public MagasinConfigBanque()
        {
        }

        public string NomCollection { get; set; }
        public string ChaineConnexion { get; set; }
        public string NomBanque { get; set; }
    }
}
