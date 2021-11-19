using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    public class HitCounter
    {
        [Key]
        [ScaffoldColumn(false)]
        public int CounterId { get; set; }

        // session ===================================

        [DisplayName("User GUID")]
        public string UserGuid { get; set; }

        [DisplayName("SessionId")]
        public string AspNetSessionId { get; set; }

        [DisplayName("OpenTime")]
        public DateTime OpenTime { get; set; }

        [DisplayName("AccessTime")]
        public DateTime? AccessTime { get; set; }

        [DisplayName("CloseTime")]
        public DateTime? CloseTime { get; set; }

        [DisplayName("Interval (minute)")]
        public double Interval { get; set; }

        //[DisplayName("Disconnected")]
        //public bool IsDisconnected { get; set; }

        [DisplayName("Online")]
        public bool IsOnline { get; set; }

        public bool IsClient { get; set; }

        public string UserAgent { get; set; }
        public bool IsCrawlerMedium { get; set; }
        public bool IsCrawlerParanoid { get; set; }

        // info ======================================

        [DisplayName("Ip Address")]
        public string IpAddress { get; set; }

        [DisplayName("Country")]
        public string CountryName { get; set; }
    }
}