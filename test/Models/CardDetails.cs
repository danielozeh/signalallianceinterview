namespace test.Models
{
    public class CardDetails
    {
        public Number number { get; set; }
        public string scheme { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public bool prepaid { get; set; }
        public Country country { get; set; }
        public Bank bank { get; set; }
    }
}
