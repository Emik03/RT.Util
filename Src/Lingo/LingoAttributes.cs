﻿using System;

namespace RT.Util.Lingo
{
    /// <summary>Specifies notes to the translator, detailing the purpose, context, or format of a translatable string.</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LingoNotesAttribute : Attribute
    {
        private readonly string _notes;

        /// <summary>Constructor for a <see cref="LingoNotesAttribute"/> attribute.</summary>
        /// <param name="notes">Specifies notes to the translator, detailing the purpose, context, or format of a translatable string.</param>
        public LingoNotesAttribute(string notes) { _notes = notes; }

        /// <summary>Gets the associated notes to the translator, detailing the purpose, context, or format of a translatable string.</summary>
        public string Notes { get { return _notes; } }
    }

    /// <summary>Specifies that a translatable string is in a particular group.</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class LingoInGroupAttribute : Attribute
    {
        private readonly object _group;
        /// <summary>Constructor for a <see cref="LingoInGroupAttribute"/> attribute.</summary>
        /// <param name="group">Specifies that a translatable string is in a particular group.</param>
        public LingoInGroupAttribute(object group) { _group = group; }

        /// <summary>Gets the group a translatable string is in.</summary>
        public object Group { get { return _group; } }
    }

    /// <summary>Specifies that a class is a class containing translatable strings.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LingoStringClassAttribute : Attribute { }

    /// <summary>Specifies information about a group of translatable strings. Use this on a field in an enum type which enumerates the available groups of strings.</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LingoGroupAttribute : Attribute
    {
        private readonly string _name;
        private readonly string _description;

        /// <summary>Constructor.</summary>
        /// <param name="name">Specifies a short name for the group, to be used in the listbox in the translation window.</param>
        /// <param name="description">Specifies a long description for the group, to be displayed above the list of string codes while the group is selected.</param>
        public LingoGroupAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        /// <summary>Specifies a short name for the group, to be used in the listbox in the translation window.</summary>
        public string Name { get { return _name; } }
        /// <summary>Specifies a long description for the group, to be displayed above the list of string codes while the group is selected.</summary>
        public string Description { get { return _description; } }
    }

#if DEBUG
    /// <summary>
    /// Use this attribute on a type that contains translations for a form. <see cref="Lingo.TranslateControl"/> will automatically add missing fields to the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LingoDebugAttribute : Attribute
    {
        private readonly string _relativePath;

        /// <summary>Constructor.</summary>
        /// <param name="relativePath">Specifies the relative path from the compiled assembly to the source file of the translation type.</param>
        public LingoDebugAttribute(string relativePath) { _relativePath = relativePath; }

        /// <summary>Specifies the relative path from the compiled assembly to the source file of the translation type.</summary>
        public string RelativePath { get { return _relativePath; } }
    }
#endif

    /// <summary>Provides information about a language. Used on the values in the <see cref="Language"/> enum.</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LanguageInfoAttribute : Attribute
    {
        private readonly string _languageCode;
        private readonly string _englishName;
        private readonly string _nativeName;
        private readonly NumberSystem _numberSystem;

        /// <summary>Constructor.</summary>
        /// <param name="languageCode">Specifies the ISO-639 language code of the language.</param>
        /// <param name="englishName">Specifies the English name of the language.</param>
        /// <param name="nativeName">Specifies the native name of the language.</param>
        /// <param name="numberSystem">Specifies the number system of the language.</param>
        public LanguageInfoAttribute(string languageCode, string englishName, string nativeName, Type numberSystem)
        {
            _languageCode = languageCode;
            _englishName = englishName;
            _nativeName = nativeName;
            if (numberSystem == typeof(NumberSystem))
                _numberSystem = null;
            else if (typeof(NumberSystem).IsAssignableFrom(numberSystem))
                _numberSystem = (NumberSystem) numberSystem.GetConstructor(Type.EmptyTypes).Invoke(null);
            else
                _numberSystem = null;
        }

        /// <summary>Specifies the ISO-639 language code of the language.</summary>
        public string LanguageCode { get { return _languageCode; } }
        /// <summary>Specifies the English name of the language.</summary>
        public string EnglishName { get { return _englishName; } }
        /// <summary>Specifies the native name of the language.</summary>
        public string NativeName { get { return _nativeName; } }
        /// <summary>Specifies the number system of the language.</summary>
        public NumberSystem NumberSystem { get { return _numberSystem; } }
    }
}
