using apifinal.Services;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;

namespace apifinal.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class StockLogsController : ControllerBase
    {
        private readonly StockLogService _service;

        public StockLogsController(StockLogService stockLogService)
        {
            _service = stockLogService;
        }

        /// <summary>
        /// Consulta logs de estoque por produto.
        /// </summary>
        /// <param name="barcode">Código de barras do produto</param>
        /// <returns>Retorna os logs de estoque do produto especificado</returns>
        /// <response code="200">Logs de estoque retornados com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpGet("{barcode}")]
        public ActionResult<IEnumerable<StockLogDTO>> GetStockLogsByProduct(string barcode)
        {
            try
            {
                var logs = _service.GetStockLogsByProduct(barcode);
                return Ok(logs);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
