using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Localization.Exceptions
{

    public class NoInstance : Exception
    {

    }

    public class NoKeyFound : Exception
    {
        public NoKeyFound(string key) : base("Missing key: " + key)
        {

        }

    }

    public class LanguageFileMissing : Exception
    {
        public LanguageFileMissing(string tag) : base("Missing language: " + tag)
        {

        }
    }


}
