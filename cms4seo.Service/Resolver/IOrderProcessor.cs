
using cms4seo.Model.Entities;

namespace cms4seo.Service.Resolver
{
    public interface IOrderProcessor
    {
        void ProcessOrder(Cart cart, ShippingDetails shippingDetails);

        string ProcessContact(string name, string email, string phone, string message);
    }
}