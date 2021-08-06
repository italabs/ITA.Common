using System.Collections.Generic;
using ITA.Common.ETW;
using ITA.Common.ETWTest.Components.BusinessLogics;
using ITA.Common.ETWTest.Model;

namespace ITA.Common.ETWTest.Components.Services
{       
    public partial class MainService : IEventSourceProvider
    {
        private readonly BusinessLogic _businessLogic = new BusinessLogic();

        [EtwTrace]
        public Customer CreateCustomer(string name)
        {
            return _businessLogic.CreateCustomer(name);
        }

        [EtwTrace]
        public Order CreateOrder(string number, string description)
        {
            return _businessLogic.CreateOrder(number, description);
        }

        [EtwTrace]
        public Customer AttachOrderToCustomer(Customer customer, Order order)
        {
            return _businessLogic.AttachOrderToCustomer(customer, order);
        }

        [EtwTrace]
        public List<Customer> GetCustomers(out int count)
        {
            return _businessLogic.GetCustomers(out count);
        }

        [EtwTrace]
        public void MainServiceCall()
        {
            _businessLogic.BusinessLogicCall();
        }

        [EtwTrace(AttributeExclude = true)]
        public IStaticEventSource GetEventSource()
        {
            return MainServiceEtwProvider.Log;
        }
    }
}
