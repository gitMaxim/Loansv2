using System;
using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class LoanAgreementProfile : Profile
    {
        public LoanAgreementProfile()
        {
            CreateMap<LoanAgreement, AnnumRate>()
                .ForMember(r => r.LoanAgreementId, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.Date, opt => opt.MapFrom(src => DateTime.Today));
            CreateMap<LoanAgreement, CreditPlan>()
                .ForMember(c => c.LoanAgreementId, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.Date, opt => opt.MapFrom(src => DateTime.Today));
            CreateMap<LoanAgreement, DebtPlan>()
                .ForMember(d => d.LoanAgreementId, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.Date, opt => opt.MapFrom(src => DateTime.Today));
        }
    }
}