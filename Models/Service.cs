namespace Training_tunisie_telecome.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
