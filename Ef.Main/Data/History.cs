using System;

namespace Ef.Main.Data
{
    public class History
    {
        public DateTime EventTime { get; set; }
        public int Id { get; set; }
        public string Event { get; set; }
    }
}