using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class PartyProfile : Profile
    {
        public PartyProfile()
        {
            CreateMap<Party, Phone>()
                .ForMember(p => p.PartyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.Party, opt => opt.MapFrom(src => src));

            CreateMap<Party, Email>()
                .ForMember(e => e.PartyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(e => e.Party, opt => opt.MapFrom(src => src));
        }

    }
}