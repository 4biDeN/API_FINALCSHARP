using apifinal.Services;
using apifinal.Services.DTOs;
using apifinal.BaseDados.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using apifinal.Services.Exceptions;
using System.Net.Mime;

namespace apifinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class PromotionController : ControllerBase
    {
        private readonly PromotionService _service;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(PromotionService service, ILogger<PromotionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Insere uma nova promoção.
        /// </summary>
        /// <param name="promotionDto">Dados da promoção a ser inserida</param>
        /// <returns>Retorna a promoção inserida</returns>
        /// <response code="201">Promoção inserida com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpPost]
        public ActionResult<TbPromotion> InsertPromotion(PromotionDTO promotionDto)
        {
            try
            {
                var entity = _service.InsertPromotion(promotionDto);
                return Ok(entity);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir promoção.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma promoção existente.
        /// </summary>
        /// <param name="id">ID da promoção a ser atualizada</param>
        /// <param name="promotionDto">Dados atualizados da promoção</param>
        /// <returns>Retorna a promoção atualizada</returns>
        /// <response code="200">Promoção atualizada com sucesso</response>
        /// <response code="404">Promoção não encontrada</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpPut("{id}")]
        public ActionResult<TbPromotion> UpdatePromotion(int id, PromotionDTO promotionDto)
        {
            try
            {
                var entity = _service.UpdatePromotion(id, promotionDto);
                return Ok(entity);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar promoção.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Busca promoções de um produto em um determinado período.
        /// </summary>
        /// <param name="productId">ID do produto</param>
        /// <param name="startDate">Data de início do período</param>
        /// <param name="endDate">Data de fim do período</param>
        /// <returns>Retorna a lista de promoções encontradas</returns>
        /// <response code="200">Promoções encontradas com sucesso</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpGet("by-product/{productId}")]
        public ActionResult<IEnumerable<TbPromotion>> GetPromotionsByProductAndPeriod(int productId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var promotions = _service.GetPromotionsByProductAndPeriod(productId, startDate, endDate);
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar promoções.");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
