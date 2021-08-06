using System.Collections.Generic;
using ITA.Common.ETW;
using ITA.Common.ETWTest.Components.DatabaseManagers;
using ITA.Common.ETWTest.Model;

namespace ITA.Common.ETWTest.Components.BusinessLogics
{
    [EtwTrace]
    public class BusinessLogic : IEventSourceProvider
    {
        private readonly DatabaseManager _dbManager = new DatabaseManager();

        public Customer CreateCustomer(string name)
        {
            return _dbManager.CreateCustomer(name);
        }

        public Order CreateOrder(string number, string description)
        {
            return _dbManager.CreateOrder(number, description);
        }

        public Customer AttachOrderToCustomer(Customer customer, Order order)
        {
            return _dbManager.AttachOrderToCustomer(customer, order);
        }

        public List<Customer> GetCustomers(out int count)
        {
            return _dbManager.GetCustomers(out count);
        }

        public void BusinessLogicCall()
        {
            _dbManager.GenerateException();
        }

        [EtwTrace(AttributeExclude = true)]
        public IStaticEventSource GetEventSource()
        {
            return BusinessLogicEtwProvider.Log;
        }
    }
}
