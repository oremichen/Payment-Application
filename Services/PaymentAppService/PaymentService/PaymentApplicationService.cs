using AutoMapper;
using EnumsNET;
using PaymentAppService.Dto;
using PaymentCore;
using PaymentRepository.Data;
using PaymentRepository.PaymentGateway.CheapGateway;
using PaymentRepository.PaymentGateway.ExpensiveGateway;
using PaymentRepository.PaymentRepo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PaymentCore.ENUM;

namespace PaymentAppService.PaymentService
{
    public class PaymentApplicationService : IPaymentApplicationService
    {
        private readonly IPaymentServiceRepository _paymentServiceRepository;
        private readonly IMapper _mapper;
        private readonly ICheapPaymentGateway _cheapPaymentGateway;
        private readonly IExpensivePaymentGateway _expensivePaymentGateway;

        public PaymentApplicationService(IPaymentServiceRepository paymentServiceRepository, IMapper mapper, ICheapPaymentGateway cheapPaymentGateway, IExpensivePaymentGateway expensivePaymentGateway)
        {
            _paymentServiceRepository = paymentServiceRepository;
            _cheapPaymentGateway = cheapPaymentGateway;
            _expensivePaymentGateway = expensivePaymentGateway;
            _mapper = mapper;
        }

        public async Task<bool> ExecuteProcessPayment(PaymentDto payment)
        {
            bool response = false;

            try
            {
                var paymentResult = _mapper.Map<Payments>(payment);
                paymentResult.Status = Status.Pending.GetEnumDescription();

                //insert into table and set status to pending
                var paymentId =  await _paymentServiceRepository.InsertPayment(paymentResult);

                //get payment record
                var paymentresult = await _paymentServiceRepository.GetPaymentsByPaymentId(paymentId);

                if (payment.Amount < 20)
                {

                    //use ICheapPaymentGateway
                    await UseCheapPaymentGateway(paymentresult);
                    response = true;
                }

                else if (payment.Amount > 20 && payment.Amount < 500)
                {

                    //use IExpensivepaymentgateway
                   var isAvailable =  await _expensivePaymentGateway.ExpensiveaymentService(paymentresult.Amount);

                    if (isAvailable == true)
                    {
                        //update payment status
                        response = await UpdateProcessedPaymentRecord(paymentresult);
                    }
                    else
                    {
                        //use ICheapPaymentGateway
                        await UseCheapPaymentGateway(paymentresult);
                        response = true;
                    }

                }
                else if (payment.Amount > 500)
                {
                    var retry = 0;
                    bool isSuccessful = false;
                    //use premiumpayment service
                     isSuccessful = await _expensivePaymentGateway.PremiumPaymentService(paymentresult.Amount);

                    if (isSuccessful == false)
                    {
                        retry = 1;
                        isSuccessful = await _expensivePaymentGateway.PremiumPaymentService(paymentresult.Amount);

                        if (isSuccessful == false && retry < 3)
                        {
                            retry = 2;
                            isSuccessful = await _expensivePaymentGateway.PremiumPaymentService(paymentresult.Amount);

                            if (isSuccessful == false && retry < 3)
                            {
                                retry = 3;
                                isSuccessful = await _expensivePaymentGateway.PremiumPaymentService(paymentresult.Amount);

                                if (isSuccessful == false && retry == 3)
                                {
                                    response = await UpdateFailedPaymentRecord(paymentresult);
                                }
                                else
                                {
                                    //update table
                                    response = await UpdateProcessedPaymentRecord(paymentresult);
                                }
                            }
                            else
                            {
                                //update table
                                response = await UpdateProcessedPaymentRecord(paymentresult);
                            }
                        }
                        else
                        { //update table
                            response = await UpdateProcessedPaymentRecord(paymentresult);
                        }
                    }
                    else
                    {
                        //update table
                        response = await UpdateProcessedPaymentRecord(paymentresult);
                    }
                }
            }
            catch (Exception)
            {
                response = false;
            }

            return response;
        }

        #region Helper
        public async Task UseCheapPaymentGateway(Payments payments)
        {
            await _cheapPaymentGateway.ExecuteCheapPaymentService(payments.Amount);

            //update payment status
            payments.Status = Status.Processed.GetEnumDescription();
            await _paymentServiceRepository.UpdatePayment(payments);
        }

        public async Task<bool> UpdateProcessedPaymentRecord(Payments payments)
        {
            payments.Status = Status.Processed.GetEnumDescription();
            await _paymentServiceRepository.UpdatePayment(payments);
            return true;
        }

        public async Task<bool> UpdateFailedPaymentRecord(Payments payments)
        {
            payments.Status = Status.Failed.GetEnumDescription();
            await _paymentServiceRepository.UpdatePayment(payments);
            return false;
        }

        #endregion
    }
}
