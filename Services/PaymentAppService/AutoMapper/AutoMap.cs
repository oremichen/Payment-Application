using AutoMapper;
using PaymentAppService.Dto;
using PaymentCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentAppService.AutoMapper
{
    public class AutoMap : Profile
    {
        public AutoMap()
        {
            CreateMap<PaymentDto, Payments>();
        }
    }
}
