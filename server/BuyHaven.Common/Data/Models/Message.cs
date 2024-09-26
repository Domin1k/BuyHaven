namespace BuyHaven.Common.Data.Models
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        private string serializedData; // Note: this is a field!

        public Message(object data)
        {
            Data = data;
        }

        private Message() { }

        public int Id { get; private set; }

        public Type Type { get; private set; }

        public bool Published { get; private set; }

        public Message MarkAsPublished()
        {
            Published = true;
            return this;
        }

        [NotMapped]
        public object Data
        {
            get => JsonConvert.DeserializeObject(serializedData, Type, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            set
            {
                Type = value?.GetType();
                serializedData = JsonConvert.SerializeObject(value, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
        }
    }
}
