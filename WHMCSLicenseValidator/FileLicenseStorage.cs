using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shozam
{
    public class FileLicenseStorage : LicenseStorage
    {
        /// <summary>
        /// File name where the local license data is stored
        /// </summary>
        private const string licenseFileName = "license.bin";

        /// <summary>
        /// Stores the local licence in the file system
        /// </summary>
        protected override void Save()
        {
            if (!String.IsNullOrEmpty(this._licenseString) )
                File.WriteAllText( licenseFileName, this._licenseString);
        }

        protected override void Retrieve()
        {
            if (File.Exists(licenseFileName))
                this._licenseString = File.ReadAllText(licenseFileName);
        }
    }
}
