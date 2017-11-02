using AutoMapper;

namespace Loansv2.Maps
{
    public static class AutoMapperModelConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new PartyProfile());
                cfg.AddProfile(new PhysicalPartyProfile());
                cfg.AddProfile(new JuristicPartyProfile());
                cfg.AddProfile(new IndividualPartyProfile());
                cfg.AddProfile(new ProjectProfile());
                cfg.AddProfile(new LoanAgreementProfile());
            });
        }
    }
}