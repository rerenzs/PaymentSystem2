using AutoMapper;
using NSubstitute;
using PaymentSystem.Domain.DTO;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Domain.Enums;
using PaymentSystem.Domain.IRepositories;
using PaymentSystem.Domain.IServices;
using PaymentSystem.Services.Mapping;
using PaymentSystem.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace PaymentSystem.Tests.ServiceTests
{
    public class AccountServiceTests
    {

        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IMapper mapper;
        private readonly IAccountService accountService;
        private readonly IPaymentService paymentService;
        public AccountServiceTests()
        {

            var config = new MapperConfiguration(cfg => cfg.AddProfile<PaymentSystemProfile>());
            this.mapper = config.CreateMapper();
            paymentService = new PaymentService(_unitOfWork,mapper);
            accountService = new AccountService(_unitOfWork, mapper, paymentService);

            var accounts = new List<Account>()
            {
                new Account {
                    ID = 1,
                    Name = "Peter Parker",
                    AccountNumber = 2123123,
                    Balance = 1200,

                }
            };
            var payments = new List<Payment>() {
                            new Payment
                            {
                                ID = 1,
                                AccountID = 1,
                                Amount = 1000,
                                Status = Status.Closed.ToString(),
                                Date = DateTime.Now.AddDays(-3),
                                Reason = "Resolved",

                            },
                            new Payment
                            {
                                ID = 2,
                                AccountID = 1,
                                Amount = 5000,
                                Status = Status.Pending.ToString(),
                                Date = DateTime.Now,
                                Reason = "test",

                            },
                            new Payment
                            {
                                ID = 3,
                                AccountID = 1,
                                Amount = 1000,
                                Status = Status.Closed.ToString(),
                                Date = DateTime.Now.AddDays(-2),
                                Reason = "Duplicate",

                            },
                        };

            _unitOfWork.Account.GetAll().Returns(accounts.AsQueryable());
            _unitOfWork.Payment.GetAll().Returns(payments.AsQueryable());

        }
        [Fact]
        public void TestGetAccountByStatus()
        {
           
            var account = accountService.Get(1);
            account.Payments = paymentService.GetAllByAccountId(account.ID);

            var paymentWithClosedStatus = account.Payments.Where(x=> x.ID == 3).FirstOrDefault();
            var paymentWithPendingStatus = account.Payments.Where(x => x.ID == 2).FirstOrDefault();

            Assert.Equal("Closed - Duplicate", paymentWithClosedStatus.Status);
            Assert.Equal("Pending", paymentWithPendingStatus.Status);
        }
        [Fact]
        public void TestGetAccountPaymentLatestDate()
        {

            var account = accountService.Get(1);
            account.Payments = paymentService.GetAllByAccountId(account.ID);

            var paymentWithLatestDate = account.Payments.FirstOrDefault();

            Assert.Equal(2, paymentWithLatestDate.ID);
        }

    }
}
