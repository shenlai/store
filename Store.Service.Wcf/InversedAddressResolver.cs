using AutoMapper;
using Store.Domain.Model;
using Store.ServiceContracts.ModelDTOs;

namespace Store.Application
{
    public class InversedAddressResolver : ValueResolver<Address, AddressDto>
    {
        protected override AddressDto ResolveCore(Address source)
        {
            return new AddressDto
            {
                City = source.City,
                Country = source.Country,
                State = source.State,
                Street = source.Street,
                Zip = source.Zip
            };
        }
    }
}