using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

   

namespace Shozam
{


    public class WHMCSLicense
    {
        private const string defaultLicenseSecKey = "Rinku@Mommy";
        private const string WHMCSUrl = "http://komotion.com/secure/modules/servers/licensing/verify.php";
        /// <summary>
        /// How long the local key is valid for in between remote verification
        /// </summary>
        private const int localKeyDays = 15;
        /// <summary>
        /// How many days to allow after local key expiry before blocking access if conncetion cannot be made
        /// </summary>
        private const int allowCheckFailDays = 5;

        private string _licenseSecKey;
        private LicenseStorage _storage = null;
        private License _license = null;

        public WHMCSLicense(
            LicenseStorage storage,
            string name,
            string email,
            string licenseKey)
        {
            this._storage = storage;
            this.License.Name = name;
            this.License.Email = email;
            this.LicenseKey = licenseKey;
        }

        public WHMCSLicense(
            string name,
            string email,
            string licenseKey)
        {
            this.License.Name = name;
            this.License.Email = email;
            this.LicenseKey = licenseKey;
        }


        public WHMCSLicense(LicenseStorage storage)
        {
            this._storage = storage;
        }
       
        public WHMCSLicense(string LicenseSecKey)
        {
            this._licenseSecKey = LicenseSecKey;
            this._storage = this.getDefaultLicenseKeyStorage();
        }

        public WHMCSLicense()
        {
            this._storage = this.getDefaultLicenseKeyStorage();
        }


        /// <summary>
        /// License key validation.
        /// If the key is invalid the funcition will rais an exception
        /// </summary>
        /// <param name="storage">License Key storage provider</param>
        /// <param name="licenseSecKey">Licence Security Key (the provide to the client)</param>
        /// <returns>Success or failure (invalid key)</returns>
        public bool IsValid
        {
            get { return this.License.IsValid; }
            set { this.License.IsValid = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public License License {
            get {
                if (this._license == null)
                {
                    if (this._storage != null) this._license = this._storage.License;
                    
                }
                if (this._license == null) this._license = new License();
                return this._license;
            }
        }

        /// <summary>
        /// Supplies the default license key storage provider
        /// </summary>
        /// <returns>A license key storage provider</returns>
        private LicenseStorage getDefaultLicenseKeyStorage()
        {
            return new FileLicenseStorage();
        }

        private void SaveLocalLicense()
        {
            if (this._storage != null && this._license != null)
                this._storage.License = this._license;
        }

        public string ValidateRemotely()
        {
            this.SaveLocalLicense();
            HttpPostSubmitter post = new HttpPostSubmitter();
            post.Url = WHMCSUrl;
            post.PostItems.Add( "licensekey", this.LicenseKey );
            post.PostItems.Add( "domain", this.License.Name  );
            post.PostItems.Add( "ip", this.Token );
            post.PostItems.Add( "dir", this.License.Email);
            post.Type = HttpPostSubmitter.PostTypeEnum.Post;
            string result = post.Post();
            return result;
        }

        /// <summary>
        /// License Secret Key
        /// </summary>
        public string LicenseSecretKey
        {
            get
            {
                if (this._licenseSecKey == null)
                {
                    this._licenseSecKey = defaultLicenseSecKey;
                }
                return this._licenseSecKey;
            }
            set
            {
                this._licenseSecKey = value;
            }
        }

        /// <summary>
        /// Date when the key was verified against the licence server
        /// </summary>
        public DateTime? OriginalCheckDate {
            get { return this.License.OriginalCheckDate; } 
            set { this.License.OriginalCheckDate = value; }
        }

        /// <summary>
        /// License Key - shorcut for accessing License propery 
        /// </summary>
        public String LicenseKey
        {
            get { return this.License.LicenseKey; }
            set { this.License.LicenseKey = value; }
        }

        /// <summary>
        /// Name - shorcut for accessing License propery 
        /// </summary>
        public String Name
        {
            get { return this.License.Name; }
            set { this.License.Name = value; }
        }

        /// <summary>
        /// Email - shorcut for accessing License propery 
        /// </summary>
        public String Email
        {
            get { return this.License.Email; }
            set { this.License.Email = value; }
        }

        /// <summary>
        /// Unique value related to the hardware of user's computer,e.g., BIOS number, host name, IP address....
        /// </summary>
        public String Token
        {
            get {
                return this.License.Token;
            }
        }

        /// <summary>
        /// License Status - shorcut for accessing License propery 
        /// </summary>
        public LicenseStatus Status
        {
            get
            {
                return this.License.Status;
            }
        }

        /// <summary>
        /// Calculate MD5 Hash string - similar interface to PHP md5 function
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Calculated MD5 hash value</returns>
        public string MD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string Validate()
        {
            License l = this.License;
            bool isLocalValid = false;
            if (l.Status == LicenseStatus.Active
                /* && l.MD5Hash == this.MD5Hash(this.LicenseSecretKey) */)
            {
                DateTime localExpiryDate = DateTime.Now.AddDays(-localKeyDays);
                if (l.OriginalCheckDate != null && l.OriginalCheckDate > localExpiryDate)
                {
                    isLocalValid = (l.Token == System.Environment.MachineName);

                }
            }
            if (isLocalValid)
            {
                this.IsValid = true;
                return "VALID:\n" + this.License.ToString();
            }
            else
            {
                string data = this.ValidateRemotely();
                if (String.IsNullOrEmpty(data))
                {
                    DateTime localExpiryDate = DateTime.Now.AddDays(  -localKeyDays - allowCheckFailDays);
                    if (l.OriginalCheckDate == null)
                    {
                        l.OriginalCheckDate = DateTime.Now;
                        l.IsValid = true;
                        this.SaveLocalLicense();
                    }
                    else if (l.OriginalCheckDate < localExpiryDate)
                    {
                        l.IsValid = false;
                        l.IsTrial = true;
                        l.Status = LicenseStatus.Trial;
                    }
                }
                else
                {
                    Dictionary<string,string> result = new Dictionary<string,string>();
                    Regex rex = new Regex(@"<(.+)>([^<]+)<\/.*>");
                    MatchCollection matches = rex.Matches(data);
                    foreach (Match m in matches)
                    {
                        result[m.Groups[1].Value] = m.Groups[2].Value;
                    }
                    if (result.ContainsKey("status") && result["status"] == "Active")
                    {
                        l.Status = LicenseStatus.Active;
                        l.LastCheckDate = DateTime.Now;
                        l.IsValid = true;
                        if (l.OriginalCheckDate == null) l.OriginalCheckDate = DateTime.Now;
                        this.SaveLocalLicense();
                        return "VALID:\n" + data;
                    }
                    else
                    {
                        if ( result.ContainsKey("status") )
                        {
                            switch ( result["status"] )
                            {
                                case "Expired": l.Status = LicenseStatus.Expired; break;
                                case "Reissued": l.Status = LicenseStatus.Reissued; break;
                                case "Suspended": l.Status = LicenseStatus.Suspended; break;
                            }
                        }
                        l.IsValid = false;
                        this.SaveLocalLicense();
                        return "INVALID:\n" + data;
                    }

                }

            }

            return "INVALID";
        }


    }
}
