using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class MessageDTO
    {
        public int Id { get; init; }
        public UserDTO? Sender { get; init; }
        public DateTime Date { get; init; }
        /// <summary>
        /// The actual message content.
        /// </summary>
        public string Content { get; init; }
        public MessageDTO()
        {
            Content = string.Empty;
        }
        public MessageDTO(UserDTO author, string content, DateTime date)
        {
            Sender = author;
            Date = date;
            Content = content;
        }
    }
}
