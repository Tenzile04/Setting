using Pustokk.Core.Models;
using Pustokk.Models;

namespace Pustokk.ViewModels
{
    public class HeaderViewModel
    {
        public List<Genre> Genres { get; set; }
        public List<Setting> Settings { get; set; }
		public AppUser User { get; set; }
	}
}
