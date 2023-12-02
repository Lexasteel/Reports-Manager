using System.Collections.Generic;

namespace Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
    public static class Dictionares
    {
        public static List<Item> DestinationType { get; set; } = new List<Item>() {
            //new Item { Id = 1, Value = "Окно" },
            //new Item { Id = 2, Value = "Printer" },
            new Item { Id = 3, Value = "Файл" },
            //new Item { Id = 4, Value = "Email" },
            //new Item { Id = 5, Value = "Exchange" },
            //new Item { Id = 6, Value = "File at Historian" },
        };
        public static List<Item> ReportType { get; set; } = new List<Item>() {
            new Item { Id = 1, Value = "Простой" },
            new Item { Id = 2, Value = "Дейст.опер." },
            new Item { Id = 3, Value = "Алармы" },
           // new Item { Id = 3, Value = "Point-ExpressCalc" },
            new Item { Id = 4, Value = "Необраб." },
           // new Item { Id = 6, Value = "SOE" },
           // new Item { Id = 7, Value = "Text" },
           // new Item { Id = 8, Value = "Unknown" },
        };
        public static List<Item> TimeType { get; set; } = new List<Item>() {
            new Item { Id = 1, Value = "Час:Мин:Сек" },
            new Item { Id = 2, Value = "День" },
            new Item { Id = 3, Value = "Месяц" },
            //new Item { Id = 5, Value = "No. of Intervals" }
            new Item { Id = 6, Value = "Часть секунды" }
        };
        public static List<Item> Filters { get; set; } = new List<Item>() {
               new Item { Id = 0, Value = "Actual" },
               new Item { Id = 1, Value = "Actual Bit" },
               new Item { Id = 2, Value ="Time Average" },
               new Item { Id = 3, Value ="Maximum Value" },
               new Item { Id = 4, Value ="Minimum Value" },
               new Item { Id = 5, Value ="Time of Maximum" },
               new Item { Id = 6, Value ="Time of Minimum" },
               new Item { Id = 7, Value ="Integration" },
               new Item { Id = 8, Value ="Toggle" },
               new Item { Id = 9, Value ="Toggle Set" },
               new Item { Id = 10, Value ="Toggle Reset" },
               new Item { Id = 11, Value ="Time Set" },
               new Item { Id = 12, Value ="Time Reset" },
                //{ 13, "" },
               new Item { Id = 14, Value ="Total" },
               new Item { Id = 15, Value ="Average" },
               new Item { Id = 16, Value ="Count" },
               new Item { Id = 17, Value ="Standard Deviation" },
               new Item { Id = 18, Value ="End Value" },
                //{ 19, "" },
                //{ 20, "" },
                //{ 21, "" },
               new Item { Id = 22, Value ="Variance" },
               new Item { Id = 23, Value ="Range" },
               new Item { Id = 24, Value ="Duration Good" },
               new Item { Id = 25, Value ="Duration Bad" },
               new Item { Id = 26, Value ="Percent Good" },
               new Item { Id = 27, Value ="Percent Bad" },
                //{ 28, "" },
               new Item { Id = 29, Value ="Delta" },
               new Item { Id = 30, Value ="Start Value" },
               new Item { Id = 31, Value ="Average Rate Of Change" }
        };
        public static List<Item> Headers { get; set; } = new List<Item>() {
            new Item { Value = "1", Description = "KKS" },
            new Item { Value = "2", Description = "Description" }, 
            new Item {Value = "3", Description  = "Eng.Units" },
        };
    }
}
