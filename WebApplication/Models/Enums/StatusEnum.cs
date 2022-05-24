namespace WebApplication.Models.Enums
{
    public enum StatusEnum
    {
        NotDelivered = 0,
    }

    public static class StatusEnumExtensions
    {
        public static string GetDescruption(StatusEnum statusEnum)
        {
            switch (statusEnum)
            {
                case StatusEnum.NotDelivered:
                    return "Не отправлено";
                default: return "UNKNOWN";
            }
        }
    }
}