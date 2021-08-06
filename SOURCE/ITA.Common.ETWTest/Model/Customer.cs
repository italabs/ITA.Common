using System.Collections.Generic;
using ITA.Common.ETW;

namespace ITA.Common.ETWTest.Model
{
    public class Customer : IEtwInformation
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public List<Order> Orders { get; set; }

        public string GetEtwInformation()
        {
            return string.Format("Name: '{0}' OrdersCount: {1}", Name, Orders.Count);
        }
    }
}
