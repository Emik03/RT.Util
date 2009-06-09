﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RT.Util.Collections;
using RT.Util.ExtensionMethods;
using RT.Util.Xml;

namespace RT.Util.Lingo
{
    /// <summary>
    /// Static class with helper methods to support multi-language applications.
    /// </summary>
    public static class Lingo
    {
        /// <summary>
        /// Attempts to load the translation for the specified module and language. The translation must exist
        /// in the application executable directory under a subdirectory called "Translations". See remarks for
        /// more info.
        /// </summary>
        /// <remarks>
        /// If the translation can be loaded successfully, this function will return true and will store the translation
        /// in the specified variable. Otherwise, the variable will be unmodified. In DEBUG mode, any exception when
        /// loading the translation will be propagated, but in release mode the function will simply behave as if the
        /// file didn't exist.
        /// </remarks>
        /// <typeparam name="TTranslation">The type of the translation class to load the translation into.</typeparam>
        /// <param name="module">The name of the module whose translation is being loaded.</param>
        /// <param name="language">The language code of the language to be loaded.</param>
        /// <param name="translation">Upon success, the translation will be stored here. On failure, this will not be modified.</param>
        /// <returns>True if the translation has been loaded and stored in "translation"; false otherwise.</returns>
        public static bool TryLoadTranslation<TTranslation>(string module, string language, ref TTranslation translation) where TTranslation : new()
        {
            string path = PathUtil.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Translations", module + "." + language + ".xml");
            if (language == null || !File.Exists(path))
                return false;
            try
            {
                translation = XmlClassify.LoadObjectFromXmlFile<TTranslation>(path);
            }
#if DEBUG
            catch (Exception e)
            {
                throw new RTException(@"Could not load translation for module ""{0}"", language ""{1}"", from file ""{2}""".Fmt(module, language, path), e);
            }
#else
            catch
            {
                string dummy = "{0}".Fmt(""); // Crappy solution for the IDE thinking that .Fmt is not used when in Release mode.
                return false;
            }
#endif
            return true;
        }

        /// <summary>
        /// Generates a list of <see cref="MenuItem"/>s for the user to select a language from. The list is generated from the set of available XML files in the application's directory.
        /// </summary>
        /// <typeparam name="TTranslation">The type in which translations are stored.</typeparam>
        /// <param name="programName">The name of the program. XML files considered valid translation files are those that match programName+".&lt;languagecode&gt;.xml".</param>
        /// <param name="setLanguage">A callback function to call when the user clicks on a menu item. The first parameter to the callback function is the <typeparamref name="TTranslation"/>
        /// object for the selected language. The second parameter is the string identifying the language, or null for the application's native language.</param>
        /// <param name="curLanguage">The string that identifies the currently-selected language. (The relevant menu item is automatically checked.)</param>
        /// <returns>A <see cref="MenuItem"/>[] containing the generated menu items.</returns>
        public static MenuItem[] LanguageMenuItems<TTranslation>(string programName, Action<TTranslation, string> setLanguage, string curLanguage) where TTranslation : TranslationBase, new()
        {
            MenuItem selected = null;
            var arr = languageMenuItems<TTranslation>(programName)
                .OrderBy(tup => tup.E1.Language.GetNativeName())
                .Select(tup => new MenuItem(tup.E1.Language.GetNativeName(), new EventHandler((snd, ev) =>
                {
                    if (selected != null) selected.Checked = false;
                    selected = (MenuItem) snd;
                    selected.Checked = true;
                    var t = (Tuple<TTranslation, string>) ((MenuItem) snd).Tag;
                    setLanguage(t.E1, t.E2);
                })) { Tag = tup, Checked = tup.E2 == curLanguage }).ToArray();
            selected = arr.FirstOrDefault(m => ((Tuple<TTranslation, string>) m.Tag).E2 == curLanguage);
            return arr;
        }

        /// <summary>
        /// Generates a list of <see cref="ToolStripMenuItem"/>s for the user to select a language from. The list is generated from the set of available XML files in the application's directory.
        /// </summary>
        /// <typeparam name="TTranslation">The type in which translations are stored.</typeparam>
        /// <param name="programName">The name of the program. XML files considered valid translation files are those that match programName+".&lt;languagecode&gt;.xml".</param>
        /// <param name="setLanguage">A callback function to call when the user clicks on a menu item. The first parameter to the callback function is the <typeparamref name="TTranslation"/>
        /// object for the selected language. The second parameter is the string identifying the language, or null for the application's native language.</param>
        /// <param name="curLanguage">The string that identifies the currently-selected language. (The relevant menu item is automatically checked.)</param>
        /// <returns>A <see cref="ToolStripMenuItem"/>[] containing the generated menu items.</returns>
        public static ToolStripMenuItem[] LanguageToolStripMenuItems<TTranslation>(string programName, Action<TTranslation, string> setLanguage, string curLanguage) where TTranslation : TranslationBase, new()
        {
            ToolStripMenuItem selected = null;
            var arr = languageMenuItems<TTranslation>(programName)
                .OrderBy(tup => tup.E1.Language.GetNativeName())
                .Select(tup => new ToolStripMenuItem(tup.E1.Language.GetNativeName(), null, new EventHandler((snd, ev) =>
                {
                    if (selected != null) selected.Checked = false;
                    selected = (ToolStripMenuItem) snd;
                    selected.Checked = true;
                    var t = (Tuple<TTranslation, string>) ((ToolStripMenuItem) snd).Tag;
                    setLanguage(t.E1, t.E2);
                })) { Tag = tup, Checked = tup.E2 == curLanguage }).ToArray();
            selected = arr.FirstOrDefault(m => ((Tuple<TTranslation, string>) m.Tag).E2 == curLanguage);
            return arr;
        }

        private static IEnumerable<Tuple<TTranslation, string>> languageMenuItems<TTranslation>(string programName) where TTranslation : TranslationBase, new()
        {
            yield return new Tuple<TTranslation, string>(new TTranslation(), null);
            var path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Translations");
            if (!Directory.Exists(path))
                yield break;
            foreach (var file in new DirectoryInfo(path).GetFiles(programName + ".*.xml"))
            {
                Match match = Regex.Match(file.Name, "^" + programName + @"\.(.*)\.xml$");
                if (!match.Success) continue;
                TTranslation transl;
                try { transl = XmlClassify.LoadObjectFromXmlFile<TTranslation>(file.FullName); }
                catch { continue; }
                yield return new Tuple<TTranslation, string>(transl, match.Groups[1].Value);
            }
        }

        /// <summary>
        /// Translates the text of the specified control and all its sub-controls using the specified translation object.
        /// </summary>
        /// <param name="control">Control whose text is to be translated.</param>
        /// <param name="translation">Object containing the translations. Use [TranslationDebug] attribute on the class you use for this.</param>
        public static void TranslateControl(Control control, object translation)
        {
            translateControl(control, translation);
        }

        private static string translate(string key, object translation, object control)
        {
            var translationType = translation.GetType();

            FieldInfo field = translationType.GetField(key);
            if (field != null)
                return field.GetValue(translation).ToString();

            PropertyInfo property = translationType.GetProperty(key);
            if (property != null)
                return property.GetValue(translation, null).ToString();

            MethodInfo method = translationType.GetMethod(key, new Type[] { typeof(Control) });
            if (method != null)
                return method.Invoke(translation, new object[] { control }).ToString();

            return null;
        }

        private static void translateControl(Control control, object translation)
        {
            if (control == null)
                return;

            if (!string.IsNullOrEmpty(control.Name))
            {
                if (!string.IsNullOrEmpty(control.Text) && (!(control.Tag is string) || ((string) control.Tag != "notranslate")))
                {
                    string translated = translate(control.Name, translation, control);
                    if (translated != null)
                        control.Text = translated;
#if DEBUG
                    else
                        setMissingTranslation(translation, control.Name, control.Text);
#endif
                }
            }

            if (control is ToolStrip)
                foreach (ToolStripItem tsi in ((ToolStrip) control).Items)
                    translateToolStripItem(tsi, translation);
            foreach (Control subcontrol in control.Controls)
                translateControl(subcontrol, translation);
        }

        private static void translateToolStripItem(ToolStripItem tsi, object translation)
        {
            if (!string.IsNullOrEmpty(tsi.Name))
            {
                if (!string.IsNullOrEmpty(tsi.Text) && (!(tsi.Tag is string) || ((string) tsi.Tag != "notranslate")))
                {
                    string translated = translate(tsi.Name, translation, tsi);
                    if (translated != null)
                        tsi.Text = translated;
#if DEBUG
                    else
                        setMissingTranslation(translation, tsi.Name, tsi.Text);
#endif
                }
            }
            if (tsi is ToolStripDropDownItem)
            {
                foreach (ToolStripItem subitem in ((ToolStripDropDownItem) tsi).DropDownItems)
                    translateToolStripItem(subitem, translation);
            }
        }

#if DEBUG
        private static void setMissingTranslation(object translation, string key, string origText)
        {
            var translationType = translation.GetType();
            var attributes = translationType.GetCustomAttributes(typeof(LingoDebugAttribute), false);
            if (!attributes.Any())
                throw new Exception("Your translation type must have a [LingoDebug(...)] attribute which specifies the relative path from the compiled assembly to the source of that translation type.");

            var translationDebugAttribute = (LingoDebugAttribute) attributes.First();
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), translationDebugAttribute.RelativePath);
            string source = File.ReadAllText(path);
            var match = Regex.Match(source, @"^\s*#region " + translationType.Name + @"\s*$", RegexOptions.Multiline);
            if (!match.Success)
                return;
            string beforeRegion = source.Substring(0, match.Index);
            var afterRegion = source.Substring(match.Index);
            match = Regex.Match(afterRegion, @"^(\s*)#endregion", RegexOptions.Multiline);
            if (!match.Success)
                return;
            var newSource = beforeRegion + afterRegion.Substring(0, match.Index) + match.Groups[1].Value + "public TrString " + key + " = \"" + origText + "\";\n" + afterRegion.Substring(match.Index);
            File.WriteAllText(path, newSource);
        }
#endif

        /// <summary>Gets the number system associated with the specified language.</summary>
        public static NumberSystem GetNumberSystem(this Language language)
        {
            var t = typeof(Language);
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                if ((Language) f.GetValue(null) == language)
                    foreach (var a in f.GetCustomAttributes(typeof(LanguageInfoAttribute), false))
                        return ((LanguageInfoAttribute) a).NumberSystem;
            return null;
        }

        /// <summary>Gets the native name of the specified language.</summary>
        public static string GetNativeName(this Language language)
        {
            var t = typeof(Language);
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                if ((Language) f.GetValue(null) == language)
                    foreach (var a in f.GetCustomAttributes(typeof(LanguageInfoAttribute), false))
                        return ((LanguageInfoAttribute) a).NativeName;
            return null;
        }

        /// <summary>Gets the English name of the specified language.</summary>
        public static string GetEnglishName(this Language language)
        {
            var t = typeof(Language);
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                if ((Language) f.GetValue(null) == language)
                    foreach (var a in f.GetCustomAttributes(typeof(LanguageInfoAttribute), false))
                        return ((LanguageInfoAttribute) a).EnglishName;
            return null;
        }

        /// <summary>Gets the ISO language code of the specified language.</summary>
        public static string GetIsoLanguageCode(this Language language)
        {
            var t = typeof(Language);
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                if ((Language) f.GetValue(null) == language)
                    foreach (var a in f.GetCustomAttributes(typeof(LanguageInfoAttribute), false))
                        return ((LanguageInfoAttribute) a).LanguageCode;
            return null;
        }
    }
}
