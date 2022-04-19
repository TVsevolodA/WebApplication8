namespace WebApplication8.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }    // описание
        public int Price { get; set; }
        public int Number { get; set; }         // количество
    }
}