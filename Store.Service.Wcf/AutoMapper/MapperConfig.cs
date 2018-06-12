using AutoMapper;
using Store.Domain.Model;
using Store.ServiceContracts.ModelDTOs;
using Store.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Store.Application.AutoMapper
{
    public class MapperConfig
    {
        /// <summary>
        /// AutoMapper框架的初始化
        /// </summary>
        public static void Initialize()
        {
            Mapper.CreateMap<AddressDto, Address>();
            Mapper.CreateMap<UserDto, User>()
                .ForMember(uMermber => uMermber.ContactAddress, mceUto => mceUto.ResolveUsing<AddressResolver>().FromMember(fm => fm.ContactAddress))
                .ForMember(uMember => uMember.DeliveryAddress, mceUto =>
                        mceUto.ResolveUsing<AddressResolver>().FromMember(fm => fm.DeliveryAddress));

            Mapper.CreateMap<User, UserDto>()
               .ForMember(udoMember => udoMember.ContactAddress, mceU =>
                   mceU.ResolveUsing<InversedAddressResolver>().FromMember(fm => fm.ContactAddress))
                   .ForMember(udoMember => udoMember.DeliveryAddress, mceU =>
                       mceU.ResolveUsing<InversedAddressResolver>().FromMember(fm => fm.DeliveryAddress));

            Mapper.CreateMap<Product, ProductDto>();
            Mapper.CreateMap<ProductDto, Product>();
            Mapper.CreateMap<Category, CategoryDto>();
            Mapper.CreateMap<CategoryDto, Category>();
            Mapper.CreateMap<ShoppingCart, ShoppingCartDto>();
            Mapper.CreateMap<ShoppingCartDto, ShoppingCart>();
            Mapper.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
            Mapper.CreateMap<ShoppingCartItemDto, ShoppingCartItem>();
            Mapper.CreateMap<OrderItem, OrderItemDto>();
            Mapper.CreateMap<OrderItemDto, OrderItem>();
            /*订单*/
            Mapper.CreateMap<Order, OrderDto>()
                .ForMember(dto => dto.UserContact, m => m.ResolveUsing(o => o.User.Contact))
                .ForMember(dto => dto.UserPhone, m => m.MapFrom(o => o.User.PhoneNumber))
                .ForMember(dto=>dto.UserEmail,m=>m.MapFrom(o=>o.User.Email))
                .ForMember(dto => dto.UserId,
                    m => m.MapFrom(o => o.User.Id))
                .ForMember(dto => dto.UserName,
                    m => m.MapFrom(o => o.User.UserName))
                .ForMember(dto => dto.UserAddressCountry,
                    m => m.MapFrom(o => o.User.DeliveryAddress.Country))
                .ForMember(dto => dto.UserAddressState,
                    m => m.MapFrom(o => o.User.DeliveryAddress.State))
                .ForMember(dto => dto.UserAddressCity,
                    m => m.MapFrom(o => o.User.DeliveryAddress.City))
                .ForMember(dto => dto.UserAddressStreet,
                    m => m.MapFrom(o => o.User.DeliveryAddress.Street))
                .ForMember(dto => dto.UserAddressZip,
                    m => m.MapFrom(o => o.User.DeliveryAddress.Zip))
                .ForMember(dto => dto.Status,
                    m => m.ResolveUsing(o =>
                    {
                        switch (o.Status)
                        {
                            case OrderStatus.Created:
                                return OrderStatusDto.Created;
                            case OrderStatus.Delevered:
                                return OrderStatusDto.Delivered;
                            case OrderStatus.Dispatched:
                                return OrderStatusDto.Dispatched;
                            case OrderStatus.Paid:
                                return OrderStatusDto.Paid;
                            case OrderStatus.Picked:
                                return OrderStatusDto.Picked;
                            default:
                                throw new InvalidOperationException();
                        }
                    }));
            Mapper.CreateMap<OrderDto, Order>();
            Mapper.CreateMap<ProductCategorization, ProductCategorizationDto>();
            Mapper.CreateMap<ProductCategorizationDto, ProductCategorization>();
            Mapper.CreateMap<Role, RoleDto>();
            Mapper.CreateMap<RoleDto, Role>();
        }

    }
}