namespace Sahaj_Yatri.Update
{
    public class Updates
    {
        public const string StatusPending = "Pending"; // Booking requested but not confirmed yet
        public const string StatusConfirmed = "Confirmed"; // Booking confirmed by the hotel
        public const string StatusInProgress = "In Progress"; // Guest checked in and stay in progress
        public const string StatusCompleted = "Completed"; // Guest checked out, stay completed successfully
        public const string StatusCancelled = "Cancelled"; // Booking cancelled by the guest or hotel

    }
}
