using ApplicationMyRoots.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class User
    {
        public User() {
            this.SendingAgreements = new List<UserTreeSharingAgreement>();
            this.ReceivingAgreements = new List<UserTreeSharingAgreement>();
        }

        public User(String Login, String Password, String Name, String Surname)
        {
            this.Login = Login;
            this.Password = Password;
            this.Name = Name;
            this.Surname = Surname;

            this.SendingAgreements = new List<UserTreeSharingAgreement>();
            this.ReceivingAgreements = new List<UserTreeSharingAgreement>();
        }

        [Key]
        public int UserID { get; set; }

        public String Login { get; set; }

        public String Password { get; set; }

        public byte[] PasswordEncoded { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        [NotMapped]
        public String NameSurname { get { return Name + " " + Surname; } }

        public DateTime? DateBorn { get; set; }

        public DateTime? DateSign { get; set; }

        public String City { get; set; }

        [ForeignKey("Language")]
        public int LanguageID { get; set; }

        public Language Language { get; set; }

        [ForeignKey("UserTreeSharingStatus")]
        public int UserTreeSharingStatusID { get; set; }

        public UserTreeSharingStatus UserTreeSharingStatus { get; set; }

        public byte[] Image { get; set; }

        [NotMapped]
        public string ImageString
        {
            get {
                DbContext db = new DbContext();
                string img = "";
                User user = db.Users.Where(u => u.UserID == this.UserID).First();
                byte[] imgByteData = user.Image;
                if (imgByteData != null)
                {
                    string imageBase64Data = Convert.ToBase64String(imgByteData);
                    img = string.Format("data:image/png;base64,{0}", imageBase64Data);
                }
                return img;
            }
        }

        public virtual List<UserTreeSharingAgreement> SendingAgreements { get; set; }

        public virtual List<UserTreeSharingAgreement> ReceivingAgreements { get; set; }

    }
}