using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Shozam
{
    public class RegistryLicenseStorage : LicenseStorage
    {
        private const string registryKey = @"HKEY_CURRENT_USER\Software\Shozam\Licenses";

        protected override void Save()
        {
            Registry.SetValue(
                registryKey,
                "License", 
                this._licenseString,
                RegistryValueKind.String);
        }

        protected override void Retrieve()
        {
            object val = Registry.GetValue(registryKey, "License", null);
            if (val != null) this._licenseString = (String)val;
        }


    }
}
