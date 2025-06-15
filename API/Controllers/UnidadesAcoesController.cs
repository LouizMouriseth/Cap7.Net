using Application.SeedWork.Responses;
using Application.UnidadeAcao;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnidadesAcoesController : Controller
{
    private readonly IMediator _mediator;

    public UnidadesAcoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todas as unidadesAções
    /// </summary>
    /// <response code="200">Uma lista de unidadeAcaos</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ListAll.ViewModel>), StatusCodes.Status200OK)]
    public async Task<BaseResponse<ListAll.ViewModel>> List([FromQuery] ListAll.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Cria uma unidadeAção
    /// </summary>
    /// <param name="request">Dados da unidadeAção</param>
    /// <response code="201">Retorna a unidadeAção criada</response>
    /// <response code="422">Um ou mais parâmetros estão ausentes ou incorretos</response>
    [HttpPost]
    [ProducesResponseType(typeof(DataResponse<Create.ViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorListResponse<>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<BaseResponse<Create.ViewModel>> Create([FromBody] Create.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Atualiza uma unidadeAção
    /// </summary>
    /// <param name="id">Id da unidadeAção a atualizar</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <response code="200">UnidadeAção atualizada</response>
    /// <response code="404">UnidadeAção não encontrada</response>
    /// <response code="422">Um ou mais parâmetros estão ausentes ou incorretos</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DataResponse<Update.ViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NoDataResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorListResponse<>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<BaseResponse<Update.ViewModel>> Update([FromRoute] Guid id, [FromBody] Update.Request request)
    {
        request.SetId(id);
        return await _mediator.Send(request);
    }
    
    /// <summary>
    /// Exclui uma unidadeAção
    /// </summary>
    /// <param name="id">Id da unidadeAção a ser excluída</param>
    /// <response code="200">unidadeAção excluída</response>
    /// <response code="404">unidadeAção não encontrada</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoDataResponse<Delete.ViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NoDataResponse<>), StatusCodes.Status404NotFound)]
    public async Task<BaseResponse<Delete.ViewModel>> Delete([FromRoute] Guid id)
    {
        var request = new Delete.Request(id);
        return await _mediator.Send(request);
    }

}