

namespace ContentServiceManagementAPI.Helpers
{
    public static class NotificationTemplates
    {
        public const string DeclineSPMessage = "Hello {ServiceProviderName}, <br><br>  " +
            "Your application as a Service Provider for the Do Not Disturb Platform has been declined for the reason listed below. <br> <br>" +
            "<b> {message} <br> <br></b>" +
            " <br> Thank You."
            ;

        public const string ApproveSPMessage = "Hello {ServiceProviderName}, <br><br> <b> " +
            "Your application as a Service Provider for the Do Not Disturb Platform has been approved. <br> <br>" +
            " <br> Thank You. <br> <br> " +
            "<a href='http://192.168.1.85:8007/auth/login' target='_blank'>Click here to login</a>."
            ;
    }
}
