namespace ITA.Common.ETWTest.Model
{
    public class Order
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return Number;
        }
    }
}
