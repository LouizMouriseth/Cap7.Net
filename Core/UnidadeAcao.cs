using Core.SeedWork;

namespace Core;

public class UnidadeAcao : GenericModel
{
    public UnidadeAcao()
    {
        
    }
    
    public UnidadeAcao(Guid idUnidade, Guid idAcao, DateTime dataImplantacao)
    {
        IdUnidade = idUnidade;
        IdAcao = idAcao;
        DataImplantacao = dataImplantacao;
    }

    public DateTime DataImplantacao { get; private set; }
    
    public Guid IdUnidade { get; private set; }
    public virtual Unidade Unidade { get; private set; }
    
    public Guid IdAcao { get; private set; }
    public virtual Acao Acao { get; private set; }
}