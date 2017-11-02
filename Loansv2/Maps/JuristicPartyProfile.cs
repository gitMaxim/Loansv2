using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class JuristicPartyProfile : Profile
    {
        public JuristicPartyProfile()
        {
            CreateMap<JuristicPartyViewModel, Party>()
                .ForMember(p => p.PartyType, opt => opt.MapFrom(src => PartyType.Juristic));
            CreateMap<JuristicPartyViewModel, JuristicParty>();

            CreateMap<JuristicParty, JuristicPartyViewModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(src => src.Party.Name))
                .ForMember(vm => vm.VatId, opt => opt.MapFrom(src => src.Party.VatId));
            CreateMap<JuristicPartyViewModel, JuristicParty>();

            CreateMap<JuristicParty, Party>();
            CreateMap<Party, JuristicParty>();
        }
    }
}