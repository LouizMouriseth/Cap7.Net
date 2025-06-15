using Application.SeedWork.Responses;
using Application.Consumo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsumosController : Controller
{
    private readonly IMediator _mediator;

    public ConsumosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todas os consumos
    /// </summary>
    /// <response code="200">Uma lista de consumos</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ListAll.ViewModel>), StatusCodes.Status200OK)]
    public async Task<BaseResponse<ListAll.ViewModel>> List([FromQuery] ListAll.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Cria um consumo
    /// </summary>
    /// <param name="request">Dados do consumo</param>
    /// <response code="201">Retorna o consumo criada</response>
    /// <response code="422">Um ou mais parâmetros estão ausentes ou incorretos</response>
    [HttpPost]
    [ProducesResponseType(typeof(DataResponse<Create.ViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorListResponse<>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<BaseResponse<Create.ViewModel>> Create([FromBody] Create.Request request)
        => await _mediator.Send(request);

    /// <summary>
    /// Atualiza um consumo
    /// </summary>
    /// <param name="id">Id do consumo a atualizar</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <response code="200">Consumo atualizada</response>
    /// <response code="404">Consumo não encontrada</response>
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
    /// Exclui uma consumo
    /// </summary>
    /// <param name="id">Id do consumo a ser excluída</param>
    /// <response code="200">Consumo excluída</response>
    /// <response code="404">Consumo não encontrada</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoDataResponse<Delete.ViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NoDataResponse<>), StatusCodes.Status404NotFound)]
    public async Task<BaseResponse<Delete.ViewModel>> Delete([FromRoute] Guid id)
    {
        var request = new Delete.Request(id);
        return await _mediator.Send(request);
    }

}