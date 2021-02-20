namespace ContentServiceManagementAPI
{
    public class ScheduledTasksSettings
    {
        public string AnqConnectionString { get; set; }
        public int BackgroundTaskInterval { get; set; }
        public int CommandTimeoutSeconds { get; set; }
        public int TransactionTimeoutSeconds { get; set; }

        public string RabbitMqNotificationHost { get; set; }
        public string RabbitMqHost { get; set; }
        public string RabbitMqBaseHost { get; set; }
    }
}
