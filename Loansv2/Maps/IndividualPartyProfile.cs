using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class IndividualPartyProfile : Profile
    {
        public IndividualPartyProfile()
        {
            CreateMap<IndividualPartyViewModel, Party>()
                .ForMember(p => p.PartyType, opt => opt.MapFrom(src => PartyType.Individual));
            CreateMap<Party, IndividualPartyViewModel>();

            CreateMap<IndividualParty, IndividualPartyViewModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(src => src.Party.Name))
                .ForMember(vm => vm.VatId, opt => opt.MapFrom(src => src.Party.VatId));

            CreateMap<IndividualPartyViewModel, IndividualParty>();

            CreateMap<IndividualParty, Party>();
            CreateMap<Party, IndividualParty>();
        }
    }
}