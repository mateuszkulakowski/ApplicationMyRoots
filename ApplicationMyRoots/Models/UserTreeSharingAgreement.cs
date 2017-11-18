using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeSharingAgreement
    {
        public UserTreeSharingAgreement() {}

        [Key]
        public int UserTreeSharingAgreementID { get; set; }

        public int UserSendingID { get; set; }

        [Required, ForeignKey("UserSendingID")]
        public virtual User UserSending { get; set; }

        public int UserRecivingID { get; set; }

        [Required, ForeignKey("UserRecivingID")]
        public virtual User UserReceiving { get; set; }

        public bool? Accpeted { get; set; } //null - wysłane jeszcze brak odpowiedzi, false-true - odpowiedź receivera

        public DateTime Date { get; set; }

        public bool? Visible { get; set; }

        public bool IsReceivedUserRead { get; set; }

        public bool IsSendedUserRead { get; set; }
    }
}