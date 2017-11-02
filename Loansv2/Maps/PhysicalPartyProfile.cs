using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class PhysicalPartyProfile : Profile
    {
        public PhysicalPartyProfile()
        {
            CreateMap<PhysicalPartyViewModel, Party>()
                .ForMember(p => p.Name, opt => opt.MapFrom(vm => vm.ShortName))
                .ForMember(p => p.VatId, opt => opt.MapFrom(vm => vm.VatId))
                .ForMember(p => p.PartyType, opt => opt.MapFrom(vm => PartyType.Physical));

            CreateMap<PhysicalPartyViewModel, PhysicalParty>();
            CreateMap<PhysicalParty, PhysicalPartyViewModel>()
                .ForMember(vm => vm.VatId, opt => opt.MapFrom(p => p.Party.VatId));

            CreateMap<PhysicalParty, Party>();
            CreateMap<Party, PhysicalParty>();
        }   
    }
}