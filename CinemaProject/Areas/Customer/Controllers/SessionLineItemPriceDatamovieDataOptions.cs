using Stripe.Checkout;

namespace CinemaProject.Areas.Customer.Controllers
{
    internal class SessionLineItemPriceDatamovieDataOptions
    {
        public object Name { get; set; }
        public object Description { get; set; }

        public static implicit operator SessionLineItemPriceDataProductDataOptions(SessionLineItemPriceDatamovieDataOptions v)
        {
            throw new NotImplementedException();
        }
    }
}