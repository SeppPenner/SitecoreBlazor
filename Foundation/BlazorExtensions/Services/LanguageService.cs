﻿using SitecoreBlazorHosted.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.BlazorExtensions.Services
{
    public class LanguageService
    {
        public Language GetDefaultLanguage()
        {
            //Get is from sitecore
            return new Language()
            {
                Name = "English",
                NativeName = "English",
                TwoLetterCode = "en"
            };
        }

        public IEnumerable<SitecoreBlazorHosted.Shared.Models.Language>
            GetLanguages()
        {
            yield return new SitecoreBlazorHosted.Shared.Models.Language()
            {
                Name = "en",
                NativeName = "English",
                TwoLetterCode = "en"
            };

            yield return new SitecoreBlazorHosted.Shared.Models.Language()
            {
                Name = "sv",
                NativeName = "Svenska",
                TwoLetterCode = "sv"
            };


        }

        public Language Get(string twoLetterCode)
        {

            return GetLanguages().FirstOrDefault(l => l.TwoLetterCode == twoLetterCode);
        }

        public bool IsValidLanguage(string language)
        {
            string[] validLanguages = new string[] { "sv", "en" };
            return validLanguages.Any(vl => vl == language);
        }


        public bool HasValidLanguageInUrl(string baseUrl, string relativeUrl)
        {

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return false;

            Uri uri = new Uri(new Uri(baseUrl), relativeUrl);

            string segment =  uri?.Segments?.ElementAt(1);

            if(string.IsNullOrWhiteSpace(segment))
                return false;

            string twoLetterCode = segment.Replace("/","");

            twoLetterCode = twoLetterCode.Replace("?", "");

            return IsValidLanguage(twoLetterCode);


        }

        public Language GetLanguageFromUrl(string baseUrl, string relativeUrl)
        {

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return GetDefaultLanguage();

            Uri uri = new Uri(new Uri(baseUrl), relativeUrl);

            string segment =  uri?.Segments?.ElementAt(1);

            if(string.IsNullOrWhiteSpace(segment))
                return GetDefaultLanguage();

            string twoLetterCode = segment.Replace("/","");

            twoLetterCode = twoLetterCode.Replace("?", "");

            return IsValidLanguage(twoLetterCode) ? Get(twoLetterCode) : GetDefaultLanguage();


        }
    }
}
