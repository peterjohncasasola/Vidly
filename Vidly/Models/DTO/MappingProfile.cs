using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace Vidly.Models.DTO
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      Mapper.CreateMap<Customer, CustomerDto>();
      Mapper.CreateMap<CustomerDto, Customer>();
    }
  }
}