using Core.SeedWork;

namespace Core;

public class Consumo : GenericModel
{
    public Consumo()
    {
        
    }
    
    public Consumo(Guid idUnidade, DateTime dataReferencia, float consumoTotal, string tipoFonte, bool eRenovavel)
    {
        IdUnidade = idUnidade;
        DataReferencia = dataReferencia;
        ConsumoTotal = consumoTotal;
        TipoFonte = tipoFonte;
        ERenovavel = eRenovavel;
    }

    public DateTime DataReferencia { get; private set; }
    public float ConsumoTotal { get; private set; }
    public string TipoFonte { get; private set; }
    public bool ERenovavel { get; private set; }
    
    public Guid IdUnidade { get; private set; }
    public virtual Unidade Unidade { get; private set; }
}