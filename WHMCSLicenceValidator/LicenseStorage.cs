using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shozam
{
    /// <summary>
    /// Local Licence Storage Provider
    /// </summary>
    abstract public class LicenseStorage
    {
        /// <summary>
        /// Serialized and Base64 encoded value
        /// </summary>
        protected License _licese;
        protected string _licenseString;


        private string SerializeBase64( object o)
        {
            // Serialize to a base 64 string
            byte[] bytes;
            long length = 0;
            MemoryStream ws = new MemoryStream();
            BinaryFormatter sf = new BinaryFormatter();
            sf.Serialize(ws, o);
            length = ws.Length;
            bytes = ws.GetBuffer();
            string encodedData = bytes.Length 
                + ":" + Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
            return encodedData;
        }

        private object DeserializeBase64(string s)
        {
            // We need to know the exact length of the string - Base64 can sometimes pad us by a byte or two
            int p = s.IndexOf(':');
            int length = Convert.ToInt32(s.Substring(0, p));

            // Extract data from the base 64 string!
            byte[] memorydata = Convert.FromBase64String(s.Substring(p + 1));
            MemoryStream rs = new MemoryStream(memorydata, 0, length);
            BinaryFormatter sf = new BinaryFormatter();
            object o = sf.Deserialize(rs);
            return o;
        }

        /// <summary>
        /// Saves the licence
        /// </summary>
        protected virtual void Save() {}

        protected virtual void Retrieve() {}

        /// <summary>
        /// Alias for Save()
        /// </summary>
        protected void Store() { this.Save(); }
        /// <summary>
        /// Alias for Restrieve()
        /// </summary>
        protected void Restore() { this.Retrieve(); }

        /// <summary>
        /// Stores and retrieves the local licence
        /// </summary>
        public License License
        {
            get {
                if (_licese == null)
                {
                    this.Retrieve();
                    if (!String.IsNullOrEmpty(this._licenseString))
                        _licese = (License)this.DeserializeBase64(this._licenseString);
                }
                return this._licese;
            }
            set { 
                this._licese = value;
                this._licenseString = this.SerializeBase64(this._licese);
                this.Save();
            }
        }

    }
}
