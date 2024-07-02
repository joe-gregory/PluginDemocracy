using PluginDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Read { get; set; } = false;
        public DateTime Date { get; set; }
        public NotificationDTO() { }
        public NotificationDTO(Notification notification)
        {
            Id = notification.Id;
            Title = notification.Title;
            Message = notification.Message;
            Read = notification.Read;
            Date = notification.Date;

        }
    }
}
