using System;

namespace CalendarEvents.Models
{
    public class EventModel : IGenericEntity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string URL { get; set; }
        public string ImagePath {get;set;}
        public Guid Id { get; set; }        
        public int Audience { get; set; }
        public string Base64Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OwnerId { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string Currency { get; set; }
    }

    public class EventModelDTO 
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string URL { get; set; }
        public string ImagePath { get; set; }
        public Guid Id { get; set; }
        public AudienceType Audience { get; set; }
        public string Base64Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OwnerId { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public override bool Equals(object obj)
        {
            var other = obj as EventModelDTO;
            if (other == null)
            {
                return false;
            }

            return Base64Id == other.Base64Id &&
                CreateDate == other.CreateDate &&
                Description == other.Description &&
                Details == other.Details &&
                End == other.End &&
                Id == other.Id &&
                ImagePath == other.ImagePath &&
                IsAllDay == other.IsAllDay &&
                OwnerId == other.OwnerId &&
                Start == other.Start &&
                Title == other.Title &&
                UpdateDate == other.UpdateDate &&
                URL == other.URL &&
                Audience == other.Audience;
        }
        public override int GetHashCode()
        {
            return this.Base64Id.GetHashCode() +
                this.CreateDate.GetHashCode() +
                this.Description.GetHashCode() +
                this.Details.GetHashCode() +
                this.End.GetHashCode() +
                this.Id.GetHashCode() +
                this.ImagePath.GetHashCode() +
                this.IsAllDay.GetHashCode() +
                this.OwnerId.GetHashCode() +
                this.Start.GetHashCode() +
                this.Title.GetHashCode() +
                this.UpdateDate.GetHashCode() +
                this.URL.GetHashCode() +
                this.Audience.GetHashCode();
        }

    }
}
