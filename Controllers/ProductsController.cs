
using apifinal.BaseDados.Models;
using apifinal.Services;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace apifinal.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductService service, ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }
        /// <summary>
        /// Insere um novo produto.
        /// </summary>
        /// <param name="productDto">Dados do produto a ser inserido</param>
        /// <returns>Retorna o produto inserido</returns>
        /// <response code="201">Produto inserido com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpPost]
        public ActionResult<TbProduct> InsertProduct(ProductDTO productDto)
        {
            try
            {
                var product = _service.InsertProduct(productDto);
                return CreatedAtAction(nameof(GetByBarcode), new { barcode = product.Barcode }, product);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lista o Produto pelo seu código de barras.
        /// </summary>
        /// <returns>Retorna o Produto cadastrado com o código de barras fornecido</returns>
        /// <response code="200">Retorna o Produto cadastrado</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpGet("{barcode}")]
        public ActionResult<TbProduct> GetByBarcode(string barcode)
        {
            try
            {
                var entity = _service.GetByBarcode(barcode);
                return Ok(entity);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto pelo código de barras.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lista produtos por descrição.
        /// </summary>
        /// <param name="description">Descrição parcial do produto</param>
        /// <returns>Retorna a lista de produtos que contém a descrição fornecida</returns>
        /// <response code="200">Retorna a lista de produtos</response>
        /// <response code="404">Nenhum produto encontrado</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpGet("search/{description}")]
        public ActionResult<IEnumerable<ProductBaseDTO>> GetByDescription(string description)
        {
            try
            {
                var products = _service.GetProductsByDescription(description);
                if (products == null || !products.Any())
                {
                    return NotFound("Nenhum produto encontrado com a descrição fornecida.");
                }
                return Ok(products);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos pela descrição.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="barcode">Código de barras do produto a ser atualizado</param>
        /// <param name="productUpdateDto">Dados do produto a serem atualizados</param>
        /// <returns>Retorna o produto atualizado</returns>
        /// <response code="200">Produto atualizado com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro no Servidor</response>
        [HttpPut("{barcode}")]
        public ActionResult<TbProduct> UpdateProduct(string barcode, ProductUpdateDTO productUpdateDto)
        {
            try
            {
                var updatedProduct = _service.UpdateProduct(barcode, productUpdateDto);
                return Ok(updatedProduct);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o Estoque de um produto existente.
        /// </summary>
        /// <param name="barcode">Código de barras do produto a ser atualizado</param>
        /// <param name="productStockDTO">Dados do estoque a serem atualizados</param>
        /// <returns>Retorna o produto atualizado</returns>
        /// <response code="200">Produto atualizado com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro no Servidor</response>
       [HttpPatch("stock/{barcode}")]
        public ActionResult<TbProduct> UpdateStock(string barcode, ProductStockUpdateDTO productStockDTO)
        {
            try
            {
                int stockValue = Convert.ToInt32(productStockDTO.Quantity);

                var updatedStock = _service.UpdateStock(barcode, stockValue);
                return Ok(updatedStock);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Produto.");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
