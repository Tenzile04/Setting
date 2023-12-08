using Pustokk.Models;

namespace Pustokk.ViewModels
{
	public class ProductDetailViewModel
	{
		public Book Book { get; set; }
		public List<Book> RelatedBooks { get; set; }
	}
}
