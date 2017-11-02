using System;
using System.Collections.Generic;
using System.Linq;
using Loansv2.Models;

namespace Loansv2.DAL
{
    public class LoansInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<LoansContext>
    {
        protected override void Seed(LoansContext context)
        {
            var projects = new List<Project>
            {
                new Project { Name = "Суши" },
                new Project { Name = "Пицца" },
                new Project { Name = "Кафе" },
                new Project { Name = "Магазин продуктов" },
                new Project { Name = "Производство чипсов" },
                new Project { Name = "Грузоперевозки" }
            };

            projects.ForEach(p => context.Projects.Add(p));
            context.SaveChanges();

            var parties = new List<Party>
            {
                new Party
                {
                    PartyType = PartyType.Physical,
                    VatId = "123456789123",
                    Name = "Сидоров И. П."
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "1234567891",
                    Name = "ООО Закон не писан"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "224456780181",
                    Name = "ИП Иванов Иван Иванович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "444456229123",
                    Name = "ИП Петров Пётр Евгеньевич"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "213256670954",
                    Name = "ИП Сидоров Алексей Евгеньевич"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "983556218122",
                    Name = "ИП Соколов Михаил Иванович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "113256835123",
                    Name = "ИП Попов Александр Петрович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "151236389187",
                    Name = "ИП Кузнецов Анатолий Федорович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "271232389188",
                    Name = "ИП Новиков Сергей Сергеевич"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "158206389287",
                    Name = "ИП Морозов Евгений Михайлович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "558216289476",
                    Name = "ИП Соловьёв Геннадий Алексеевич"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "112224549878",
                    Name = "ИП Зайцев Александр Константинович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "571236018943",
                    Name = "ИП Воробьёв Александр Игоревич"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "154318426803",
                    Name = "ИП Фёдоров Константин Викторович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "173742185984",
                    Name = "ИП Белов Антон Петрович"
                },
                new Party
                {
                    PartyType = PartyType.Individual,
                    VatId = "873242257961",
                    Name = "ИП Журавлёв Виктор Антонович"
                },
                new Party
                {   // at 16
                    PartyType = PartyType.Individual,
                    VatId = "437324221237",
                    Name = "ИП Бобров Геннадий Александрович"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "2531245467",
                    Name = "ООО Живу красиво"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "3952251612",
                    Name = "ООО Черное дело"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "1267124649",
                    Name = "ООО Белое дело"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "2857673497",
                    Name = "ООО Серое дело"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "3512685438",
                    Name = "ООО Счастливое предприятие"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "6212684529",
                    Name = "ООО Качественый бизнес"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "3432225433",
                    Name = "ООО Некачественный бизнес"
                },
                new Party
                {
                    PartyType = PartyType.Juristic,
                    VatId = "7832921432",
                    Name = "ООО Несчастное предприятие"
                },
                new Party
                {   // at 25
                    PartyType = PartyType.Juristic,
                    VatId = "9134251721",
                    Name = "ООО Тайная компания"
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    Name = "Жуков А. И."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    Name = "Журавлёв А. В."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    Name = "Сурков П. С."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    VatId = "869414262981",
                    Name = "Васильев Н. В."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    Name = "Семёнов Ф. П."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    Name = "Егоров Н. В."
                },
                new Party
                {
                    PartyType = PartyType.Physical,
                    VatId = "725869426371",
                    Name = "Козлов Н. К."
                },
                new Party
                {   // at 33
                    PartyType = PartyType.Physical,
                    VatId = "921839420282",
                    Name = "Иванов И. Д."
                }
            };

            parties.ForEach(p => context.Parties.Add(p));
            context.SaveChanges();

            var physicalParties = new List<PhysicalParty>
            {
                new PhysicalParty
                {
                    Id = parties.ElementAt(0).Id,
                    FirstName = "Иван",
                    MiddleName = "Петрович",
                    LastName = "Сидоров"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(26).Id,
                    FirstName = "Александр",
                    MiddleName = "Иванович",
                    LastName = "Жуков"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(27).Id,
                    FirstName = "Алексей",
                    MiddleName = "Викторович",
                    LastName = "Журавлёв"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(28).Id,
                    FirstName = "Пётр",
                    MiddleName = "Сергеевич",
                    LastName = "Сурков"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(29).Id,
                    FirstName = "Николай",
                    MiddleName = "Васильевич",
                    LastName = "Васильев"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(30).Id,
                    FirstName = "Фёдор",
                    MiddleName = "Петрович",
                    LastName = "Семёнов"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(31).Id,
                    FirstName = "Никита",
                    MiddleName = "Витальевич",
                    LastName = "Егоров"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(32).Id,
                    FirstName = "Никита",
                    MiddleName = "Контантинович",
                    LastName = "Козлов"
                },
                new PhysicalParty
                {
                    Id = parties.ElementAt(33).Id,
                    FirstName = "Иван",
                    MiddleName = "Демьянович",
                    LastName = "Иванов"
                }
            };

            physicalParties.ForEach(p => context.PhysicalParties.Add(p));
            context.SaveChanges();

            var juristicParties = new List<JuristicParty>
            {
                new JuristicParty
                {
                    Id = parties.ElementAt(1).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(17).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(18).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(19).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(20).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(21).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(22).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(23).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(24).Id
                },
                new JuristicParty
                {
                    Id = parties.ElementAt(25).Id
                }
            };

            juristicParties.ForEach(p => context.JuristicParties.Add(p));
            context.SaveChanges();

            var individualParties = new List<IndividualParty>
            {
                new IndividualParty
                {
                    Id = parties.ElementAt(2).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(3).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(4).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(5).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(6).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(7).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(8).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(9).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(10).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(11).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(12).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(13).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(14).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(15).Id
                },
                new IndividualParty
                {
                    Id = parties.ElementAt(16).Id
                }
            };

            individualParties.ForEach(p => context.IndividualParties.Add(p));
            context.SaveChanges();

            var loans = new List<LoanAgreement>
            {
                new LoanAgreement
                {
                    Number = "150",
                    CreditorId = parties.ElementAt(0).Id,
                    CreditorProjectId = projects.ElementAt(0).Id,

                    DebtorId = parties.ElementAt(1).Id,
                    DebtorProjectId = projects.ElementAt(2).Id,

                    SignDate = DateTime.Parse("2015-09-01"), DeadlineDate = DateTime.Parse("2020-09-01"),
                    Sum = 5950000
                },
                new LoanAgreement
                {
                    Number = "100",
                    CreditorId = parties.ElementAt(26).Id,
                    CreditorProjectId = projects.ElementAt(0).Id,

                    DebtorId = parties.ElementAt(30).Id,
                    DebtorProjectId = projects.ElementAt(5).Id,

                    SignDate = DateTime.Parse("2017-09-01"), DeadlineDate = DateTime.Parse("2018-09-01"),
                    Sum = 10500000
                },
                new LoanAgreement
                {
                    Number = "200",
                    CreditorId = parties.ElementAt(1).Id,
                    CreditorProjectId = projects.ElementAt(2).Id,

                    DebtorId = parties.ElementAt(16).Id,
                    DebtorProjectId = projects.ElementAt(3).Id,

                    SignDate = DateTime.Parse("2016-03-15"), DeadlineDate = DateTime.Parse("2017-12-10"),
                    Sum = 5000000
                },
                new LoanAgreement
                {
                    Number = "51/1",
                    CreditorId = parties.ElementAt(4).Id,
                    CreditorProjectId = projects.ElementAt(4).Id,

                    DebtorId = parties.ElementAt(19).Id,
                    DebtorProjectId = projects.ElementAt(2).Id,

                    SignDate = DateTime.Parse("2016-07-01"), DeadlineDate = DateTime.Parse("2018-02-01"),
                    Sum = 14000000
                },
                new LoanAgreement
                {
                    Number = "186",
                    CreditorId = parties.ElementAt(33).Id,
                    CreditorProjectId = projects.ElementAt(1).Id,

                    DebtorId = parties.ElementAt(24).Id,
                    DebtorProjectId = projects.ElementAt(4).Id,

                    SignDate = DateTime.Parse("2016-04-01"), DeadlineDate = DateTime.Parse("2017-12-01"),
                    Sum = 14000000
                },
                new LoanAgreement
                {
                    Number = "187",
                    CreditorId = parties.ElementAt(33).Id,
                    CreditorProjectId = projects.ElementAt(1).Id,

                    DebtorId = parties.ElementAt(25).Id,
                    DebtorProjectId = projects.ElementAt(4).Id,

                    SignDate = DateTime.Parse("2016-05-01"), DeadlineDate = DateTime.Parse("2018-05-01"),
                    Sum = 8000000
                },
            };

            loans.ForEach(p => context.LoanAgreements.Add(p));
            context.SaveChanges();

            var phones = new List<Phone>
            {
                new Phone
                {
                    Id = 1, PartyId = parties.ElementAt(0).Id, Number = "+79031234567"
                },
                new Phone
                {
                    Id = 2, PartyId = parties.ElementAt(1).Id, Number = "+79035555555"
                },
                new Phone
                {
                    Id = 3, PartyId = parties.ElementAt(1).Id, Number = "+79036666666"
                },
                new Phone
                {
                    Id = 4, PartyId = parties.ElementAt(2).Id, Number = "+79039561235"
                },
                new Phone
                {
                    Id = 5, PartyId = parties.ElementAt(3).Id, Number = "+79034021035"
                },
                new Phone
                {
                    Id = 6, PartyId = parties.ElementAt(4).Id, Number = "+79034021036"
                }
            };

            phones.ForEach(phone => context.Phones.Add(phone));
            context.SaveChanges();

            var mails = new List<Email>
            {
                new Email
                {
                    Id = 1, PartyId = parties.ElementAt(0).Id, Address = "somemail@mail.com"
                },
                new Email
                {
                    Id = 2, PartyId = parties.ElementAt(0).Id, Address = "dude@gmail.com"
                },
                new Email
                {
                    Id = 3, PartyId = parties.ElementAt(1).Id, Address = "supercompany@yandex.ru"
                },
                new Email
                {
                    Id = 4, PartyId = parties.ElementAt(2).Id, Address = "smartguy@mail.ru"
                },
                new Email
                {
                    Id = 5, PartyId = parties.ElementAt(3).Id, Address = "working24_7@outlook.com"
                },
                new Email
                {
                    Id = 6, PartyId = parties.ElementAt(4).Id, Address = "resting1@outlook.com"
                }
            };

            mails.ForEach(e => context.Emails.Add(e));
            context.SaveChanges();

            var payments = new List<Payment>
            {
                /*
                    Number = "150"
                    SignDate = DateTime.Parse("2015-09-01"), DeadlineDate = DateTime.Parse("2020-09-01")
                    Sum = 5950000
                 */
                new Payment(1, new DateTime(2015, 9, 10), 1500000, PaymentType.Credit),
                new Payment(1, new DateTime(2015, 10, 1), 500000, PaymentType.Credit),
                new Payment(1, new DateTime(2016, 3, 1), 1000000, PaymentType.DebtLoan),
                new Payment(1, new DateTime(2017, 9, 1), 500000, PaymentType.Credit),
                new Payment(1, new DateTime(2017, 9, 1), 1000000, PaymentType.DebtLoan),
                new Payment(1, new DateTime(2017, 9, 1), 100000, PaymentType.DebtPercents),
                new Payment(1, new DateTime(2017, 12, 24), 100000, PaymentType.DebtPercents),

                /*
                    Number = "100"
                    SignDate = DateTime.Parse("2017-09-01"), DeadlineDate = DateTime.Parse("2018-09-01")
                    Sum = 10500000
                */
                new Payment(2, new DateTime(2017, 9, 1), 1000000, PaymentType.Credit),
                new Payment(2, new DateTime(2017, 9, 10), 500000, PaymentType.Credit),
                new Payment(2, new DateTime(2017, 9, 20), 1500000, PaymentType.Credit),
                new Payment(2, new DateTime(2017, 9, 20), 650000, PaymentType.DebtLoan),
                

                /*
                    Number = "200"
                    SignDate = DateTime.Parse("2016-03-15"), DeadlineDate = DateTime.Parse("2017-12-10")
                    Sum = 5000000
                */
                new Payment(3, new DateTime(2016, 4, 15), 1000000, PaymentType.Credit),
                new Payment(3, new DateTime(2016, 10, 1), 1000000, PaymentType.DebtLoan),
                new Payment(3, new DateTime(2017, 10, 15), 46000, PaymentType.DebtPercents),
                new Payment(3, new DateTime(2017, 10, 3), 2000000, PaymentType.Credit),
                new Payment(3, new DateTime(2017, 12, 1), 1500000, PaymentType.DebtLoan),


                /*
                    Number = "51/1"
                    SignDate = DateTime.Parse("2016-07-01"), DeadlineDate = DateTime.Parse("2018-02-01")
                    Sum = 14000000
                */
                new Payment(4, new DateTime(2016, 7, 1), 1000000, PaymentType.Credit),
                new Payment(4, new DateTime(2016, 11, 1), 500000, PaymentType.Credit),
                new Payment(4, new DateTime(2017, 1, 31), 1000000, PaymentType.DebtLoan),
                new Payment(4, new DateTime(2017, 9, 20), 500000, PaymentType.DebtLoan),
                new Payment(4, new DateTime(2017, 9, 20), 200000, PaymentType.DebtPercents),


                /*
                    Number = "186"
                    SignDate = DateTime.Parse("2016-04-01"), DeadlineDate = DateTime.Parse("2017-12-01")
                    Sum = 14000000
                */
                new Payment(5, new DateTime(2016, 4, 1), 5000000, PaymentType.Credit),
                new Payment(5, new DateTime(2016, 8, 1), 5000000, PaymentType.Credit),
                new Payment(5, new DateTime(2017, 10, 1), 7000000, PaymentType.DebtLoan),
                new Payment(5, new DateTime(2017, 10, 2), 1000000, PaymentType.DebtPercents),
                new Payment(5, new DateTime(2017, 10, 3), 4000000, PaymentType.Credit),


                /*
                    Number = "187"
                    SignDate = DateTime.Parse("2016-05-01"), DeadlineDate = DateTime.Parse("2018-05-01")
                    Sum = 8000000
                 */
                new Payment(6, new DateTime(2016, 5, 1), 1000000, PaymentType.Credit),
                new Payment(6, new DateTime(2016, 6, 1), 1000000, PaymentType.Credit),
                new Payment(6, new DateTime(2016, 7, 1), 1000000, PaymentType.Credit),
                new Payment(6, new DateTime(2017, 2, 15), 60000, PaymentType.DebtPercents),
                new Payment(6, new DateTime(2017, 2, 1), 1000000, PaymentType.Credit),
                new Payment(6, new DateTime(2017, 3, 1), 1000000, PaymentType.Credit),
                new Payment(6, new DateTime(2017, 3, 1), 5000000, PaymentType.DebtLoan),
            };      

            payments.ForEach(p => context.Payments.Add(p));
            context.SaveChanges();

            var rates = new List<AnnumRate>
            {
                new AnnumRate(new DateTime(2015, 9, 1), 10, 1),
                new AnnumRate(new DateTime(2017, 9, 1), 15, 2),
                new AnnumRate(new DateTime(2016, 4, 1), 10, 3),
                new AnnumRate(new DateTime(2016, 7, 1), 20, 4),
                new AnnumRate(new DateTime(2016, 4, 1), 5, 5),
                new AnnumRate(new DateTime(2016, 7, 1), 10, 5),
                new AnnumRate(new DateTime(2016, 5, 1), 3, 6),
            };

            rates.ForEach(r => context.AnnumRates.Add(r));
            context.SaveChanges();

            var creditPlan = new List<CreditPlan>()
            {
                new CreditPlan(new DateTime(2015, 10, 1), 2000000, 1),
                new CreditPlan(new DateTime(2017, 12, 1), 1000000, 1),
            };

            creditPlan.ForEach(p => context.CreditPlans.Add(p));
            context.SaveChanges();

            var debtPlan = new List<DebtPlan>()
            {
                new DebtPlan(new DateTime(2017, 9, 1), 1000000, 1),
                new DebtPlan(new DateTime(2017, 11, 1), 1000000, 1),
            };

            debtPlan.ForEach(p => context.DebtPlans.Add(p));
            context.SaveChanges();
        }
    }
}
 
 