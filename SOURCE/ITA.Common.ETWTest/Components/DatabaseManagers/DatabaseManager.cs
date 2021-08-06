using System;
using System.Collections.Generic;
using ITA.Common.ETW;
using ITA.Common.ETWTest.Model;
using System.Threading;

namespace ITA.Common.ETWTest.Components.DatabaseManagers
{
    [EtwTrace]
    public partial class DatabaseManager : IEventSourceProvider
    {
        private List<Customer> _customers = new List<Customer>();
        private List<Order> _orders = new List<Order>();

        public Customer CreateCustomer(string name)
        {
            var loadCpuSeconds = 5;

            DatabaseManagerEtwProvider.Log.LoadCpu(loadCpuSeconds);

            LoadCpu(loadCpuSeconds);

            var customer = new Customer { Id = Guid.NewGuid().ToString(), Name = name };
            _customers.Add(customer);           

            return customer;
        }

        public Order CreateOrder(string number, string description)
        {
            var order = new Order { Id = Guid.NewGuid().ToString(), Number = number, Description = description };
            _orders.Add(order);
            return order;
        }

        public Customer AttachOrderToCustomer(Customer customer, Order order)
        {
            customer.Orders.Add(order);

            GetEventSource().FireEventVerbose("Attach order to customer",
                string.Format("Customer='{0}' OrdersCount='{1}'", customer.Name, customer.Orders.Count));            

            return customer;
        }

        public List<Customer> GetCustomers(out int count)
        {
            count = _customers.Count;
            return _customers;
        }

        public void GenerateException()
        {
            throw new Exception("DatabaseManagerException");
        }

        private void LoadCpu(int seconds)
        {
            ManualResetEvent e = new ManualResetEvent(false);
            DateTime dt = DateTime.Now;
            while (!e.WaitOne(10))
            {
                if ((DateTime.Now - dt).TotalSeconds > seconds)
                    e.Set();
            }
        }

        [EtwTrace(AttributeExclude = true)]
        public IStaticEventSource GetEventSource()
        {
            return DatabaseManagerEtwProvider.Log;
        }
    }
}
