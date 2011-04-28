using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shozam
{

    public enum LicenseStatus
    {
        Reissued = 1,
        Active = 2,
        Suspended = 4,
        Expired = 8,
        Trial = 16
    }


    /// <summary>
    /// Application License
    /// </summary>
    [Serializable()]
    public class License
    {
        private string _token = null;
        public string Name { get; set; }
        public string Email { get; set; }
        public string LicenseKey { get; set; }
        /// <summary>
        /// Unique value related to the hardware of user's computer,e.g., BIOS number, host name, IP address....
        /// </summary>
        public string Token {
            get {
                if (String.IsNullOrEmpty(this._token))
                    this._token = System.Environment.MachineName;
                return this._token;
            }
        }
        /// <summary>
        /// Time the license was activated
        /// </summary>
        public DateTime? OriginalCheckDate { get; set; }
        /// <summary>
        /// Last time the license was checked;
        /// </summary>
        public DateTime? LastCheckDate { get; set; }
        public bool IsTrial { get; set; }

        public string MD5Hash { get; set; }

        public LicenseStatus Status { get; set; }

        public bool IsValid { get; set; }



    }
}
