using Microsoft.AspNetCore.Mvc;
using MeuProjetoApi.Models;

namespace MeuProjetoApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProdutosController : ControllerBase
	{
		private static List<Produto> produtos = new()
		{
			new Produto { Id = 1, Nome = "Camiseta", Preco = 49.90M },
			new Produto { Id = 2, Nome = "Calça", Preco = 89.90M }
		};

		[HttpGet]
		public ActionResult<IEnumerable<Produto>> Get() => Ok(produtos);

		[HttpGet("{id}")]
		public ActionResult<Produto> Get(int id)
		{
			var produto = produtos.FirstOrDefault(p => p.Id == id);
			return produto == null ? NotFound() : Ok(produto);
		}

		[HttpPost]
		public ActionResult<Produto> Post(Produto novoProduto)
		{
			novoProduto.Id = produtos.Max(p => p.Id) + 1;
			produtos.Add(novoProduto);
			return CreatedAtAction(nameof(Get), new { id = novoProduto.Id }, novoProduto);
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var produto = produtos.FirstOrDefault(p => p.Id == id);
			if (produto == null) return NotFound();

			produtos.Remove(produto);
			return NoContent();
		}
	}
}
