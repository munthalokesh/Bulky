using Bulky.Models;

namespace Bulky.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public double total { get; set; }
    }
}
