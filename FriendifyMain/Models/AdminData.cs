namespace FriendifyMain.Models
{
    public class AdminData
    {
        public int TotalInteractions { get; set; }
        public int TotalAccounts { get; set; }

        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int TotalAccountsInTimespan { get; set; }
        public double AverageInteractions { get; set; }
        public List<DateTime> RegistrationDates { get; set; }
    }
}
