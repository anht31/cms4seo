using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace cms4seo.Model.Entities
{
    public class Supporter
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        public string Name { get; set; }

        // relation - photo =======================================
        // img-1|img-2 |img-3 (with first is default)
        public string Photos { get; set; }

        public string Role { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public string Phone { get; set; }

        public string Phone2 { get; set; }

        public string Email { get; set; }


        //============= Social ===============

        [DisplayName("Nick Skype")]
        public string SkypeId { get; set; }

        [DisplayName("Nick Yahoo")]
        public string YahooId { get; set; }

        public string YahooStatusPhoto => "http://opi.yahoo.com/online?u=" + YahooId + "&m=g&t=2";

        public string YahooSendIM => "ymsgr:SendIM?" + YahooId;

        public string SkypeSendIM => "skype:" + SkypeId + "?chat";
    }
}