﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RT.Util.Xml;
using RT.Util.Serialization;

namespace RT.Util.Lingo
{
    /// <summary>Abstract base class to represent translations of a piece of software.</summary>
    public abstract class TranslationBase
    {
        /// <summary>Language of this translation.</summary>
        [LingoIgnore, ClassifyIgnore]
        public Language Language;

        /// <summary>Constructor.</summary>
        public TranslationBase(Language language) { Language = language; }
    }
}
