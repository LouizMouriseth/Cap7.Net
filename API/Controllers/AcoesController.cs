using Application.SeedWork.Responses;
using Application.Acao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcoesController : Controller
{
    private readonly IMediator _mediator;

    public AcoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todas as ações
    /// </summary>
    /// <response code="200">Uma lista de ações</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedResponse<ListAll.ViewModel>), StatusCodes.Status200OK)]
    public async Task<BaseResponse<ListAll.ViewModel>> List([FromQuery] ListAll.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Cria uma ação
    /// </summary>
    /// <param name="request">Dados da ação</param>
    /// <response code="201">Retorna a ação criada</response>
    /// <response code="422">Um ou mais parâmetros estão ausentes ou incorretos</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(DataResponse<Create.ViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorListResponse<>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<BaseResponse<Create.ViewModel>> Create([FromBody] Create.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Atualiza uma ação
    /// </summary>
    /// <param name="id">Id da ação a atualizar</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <response code="200">Ação atualizada</response>
    /// <response code="404">Ação não encontrada</response>
    /// <response code="422">Um ou mais parâmetros estão ausentes ou incorretos</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(DataResponse<Update.ViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NoDataResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorListResponse<>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<BaseResponse<Update.ViewModel>> Update([FromRoute] Guid id, [FromBody] Update.Request request)
    {
        request.SetId(id);
        return await _mediator.Send(request);
    }
    
    /// <summary>
    /// Exclui uma ação
    /// </summary>
    /// <param name="id">Id da ação a ser excluída</param>
    /// <response code="200">Ação excluída</response>
    /// <response code="404">Ação não encontrada</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(NoDataResponse<Delete.ViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NoDataResponse<>), StatusCodes.Status404NotFound)]
    public async Task<BaseResponse<Delete.ViewModel>> Delete([FromRoute] Guid id)
    {
        var request = new Delete.Request(id);
        return await _mediator.Send(request);
    }

}