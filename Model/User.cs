namespace api_rest.Model
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
