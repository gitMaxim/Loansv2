using System.Linq;
using AutoMapper;
using Loansv2.Models;

namespace Loansv2.Maps
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDetailsViewModel>();

            CreateMap<LoanAgreement, ProjectCreditorsViewModel>()
                .ForMember(vm => vm.LoanedSum,
                    opt => opt.MapFrom(src => src.Payments
                        .Where(p => p.PaymentType == PaymentType.Credit)
                        .Sum(p => p.Value)))
                .ForMember(vm => vm.Creditor, opt => opt.MapFrom(src => src.Creditor.Name))
                .ForMember(vm => vm.Debtor, opt => opt.MapFrom(src => src.Debtor.Name))
                .ForMember(vm => vm.DebtorProject, opt => opt.MapFrom(src => src.DebtorProject.Name));

            CreateMap<LoanAgreement, ProjectDebtorsViewModel>()
                .ForMember(vm => vm.RepayedSum,
                    opt => opt.MapFrom(src => src.Payments
                        .Where(p => p.PaymentType == PaymentType.DebtLoan)
                        .Sum(p => p.Value)))
                .ForMember(vm => vm.Creditor, opt => opt.MapFrom(src => src.Creditor.Name))
                .ForMember(vm => vm.Debtor, opt => opt.MapFrom(src => src.Debtor.Name))
                .ForMember(vm => vm.CreditorProject, opt => opt.MapFrom(src => src.CreditorProject.Name));
        }
    }
}