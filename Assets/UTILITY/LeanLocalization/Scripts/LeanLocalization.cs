using UnityEngine;
using System.Collections.Generic;

namespace Lean.Localization
{
	// This component stores a list of phrases, their translations, as well as manage a global list of translations for easy access
	[ExecuteInEditMode]
	public class LeanLocalization : MonoBehaviour
	{
		public enum DetectType
		{
			None,
			SystemLanguage,
			CurrentCulture,
			CurrentUICulture
		}

		[Tooltip("If the application is started and no language has been loaded or auto detected, this language will be used")]
		[LeanLanguageName]
		public string DefaultLanguage;

		[Tooltip("How should the cultures be used to detect the user's device language?")]
		public DetectType DetectLanguage = DetectType.SystemLanguage;

		[Tooltip("Automatically save/load the CurrentLanguage selection to PlayerPrefs? (can be cleared with ClearSave context menu option)")]
		public bool SaveLanguage = true;

		// The list of languages defined by this localization
		public List<string> Languages;

		// The list of cultures defined by this localization
		public List<LeanCulture> Cultures;

		// The list of phrases defined by this localization
		public List<LeanPhrase> Phrases;

		// Called when the language or translations change
		public static System.Action OnLocalizationChanged;

		// All active and enabled LeanLocalization components
		public static List<LeanLocalization> AllLocalizations = new List<LeanLocalization>();

		// The currently set language
		private static string currentLanguage;

		// The list of currently supported languages
		private static List<string> currentLanguages = new List<string>();

		// The list of currently supported phrases
		private static List<string> currentPhrases = new List<string>();

		// Dictionary of all the phrase names mapped to their current translations
		private static Dictionary<string, LeanTranslation> currentTranslations = new Dictionary<string, LeanTranslation>();

		// The list of languages that you can currently switch between
		public static List<string> CurrentLanguages
		{
			get
			{
				return currentLanguages;
			}
		}

		// The list of languages that you can currently switch between
		public static List<string> CurrentPhrases
		{
			get
			{
				return currentPhrases;
			}
		}

		// Does at least one localization have 'SaveLanguage' set?
		public static bool CurrentSaveLanguage
		{
			get
			{
				for (var i = AllLocalizations.Count - 1; i >= 0; i--)
				{
					if (AllLocalizations[i].SaveLanguage == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		// Change the current language of this instance?
		public static string CurrentLanguage
		{
			set
			{
				if (CurrentLanguage != value)
				{
					currentLanguage = value;
					
					UpdateTranslations();

					if (CurrentSaveLanguage == true)
					{
						PlayerPrefs.SetString("LeanLocalization.CurrentLanguage", value);
					}
				}
			}

			get
			{
				return currentLanguage;
			}
		}

		[ContextMenu("Clear Save")]
		public void ClearSave()
		{
			PlayerPrefs.DeleteKey("LeanLocalization.CurrentLanguage");
		}

		// Sets the 'CurrentLanguage' property, this is useful for GUI events
		public void SetCurrentLanguage(string newLanguage)
		{
			CurrentLanguage = newLanguage;
		}

		// Get the current translation for this phrase, or return null
		public static LeanTranslation GetTranslation(string phraseName)
		{
			var translation = default(LeanTranslation);

			if (currentTranslations != null && phraseName != null)
			{
				currentTranslations.TryGetValue(phraseName, out translation);
			}

			return translation;
		}
		
		// Get the current text for this phrase, or return null
		public static string GetTranslationText(string phraseName)
		{
			var translation = GetTranslation(phraseName);

			if (translation != null)
			{
				return translation.Text;
			}

			return null;
		}

		// Get the current Object for this phrase, or return null
		public static Object GetTranslationObject(string phraseName)
		{
			var translation = GetTranslation(phraseName);

			if (translation != null)
			{
				return translation.Object;
			}

			return null;
		}

		// Add a new language to this localization
		public void AddLanguage(string language, string[] aliases = null)
		{
			if (Languages == null) Languages = new List<string>();

			// Add language to languages list?
			if (Languages.Contains(language) == false)
			{
				Languages.Add(language);
			}

			// Add cultures to language cultures list?
			if (aliases != null && aliases.Length > 0)
			{
				if (Cultures == null) Cultures = new List<LeanCulture>();

				for (var i = 0; i < aliases.Length; i++)
				{
					var alias = aliases[i];

					if (Cultures.Exists(c => c.Language == language && c.Alias == alias) == false)
					{
						var newCulture = new LeanCulture();

						newCulture.Language = language;
						newCulture.Alias    = alias;

						Cultures.Add(newCulture);
					}
				}
			}
		}

		// Add a new culture
		public LeanCulture AddCulture(string language, string alias)
		{
			if (Cultures == null) Cultures = new List<LeanCulture>();

			var culture = Cultures.Find(c => c.Language == language && c.Alias == alias);

			if (culture == null)
			{
				culture = new LeanCulture();

				culture.Language = language;
				culture.Alias    = alias;

				Cultures.Add(culture);
			}

			return culture;
		}

		// Add a new phrase to this localization, or return the current one
		public LeanPhrase AddPhrase(string phraseName)
		{
			if (Phrases == null) Phrases = new List<LeanPhrase>();

			var phrase = Phrases.Find(p => p.Name == phraseName);

			if (phrase == null)
			{
				phrase = new LeanPhrase();

				phrase.Name = phraseName;

				Phrases.Add(phrase);
			}

			return phrase;
		}
		
		// Add a new translation to this localization, or return the current one
		public LeanTranslation AddTranslation(string language, string phraseName)
		{
			AddLanguage(language);

			return AddPhrase(phraseName).AddTranslation(language);
		}

		// This rebuilds the dictionary used to quickly map phrase names to translations for the current language
		public static void UpdateTranslations()
		{
			currentTranslations.Clear();
			currentLanguages.Clear();
			currentPhrases.Clear();

			// Go through all enabled localizations
			for (var i = 0; i < AllLocalizations.Count; i++)
			{
				var localization = AllLocalizations[i];

				// Add all phrases to currentTranslations
				if (localization.Phrases != null)
				{
					for (var j = localization.Phrases.Count - 1; j >= 0; j--)
					{
						var phrase     = localization.Phrases[j];
						var phraseName = phrase.Name;

						if (currentPhrases.Contains(phraseName) == false)
						{
							currentPhrases.Add(phraseName);
						}

						// Make sure this phrase hasn't already been added
						if (currentTranslations.ContainsKey(phraseName) == false)
						{
							// Find the translation for this phrase
							var translation = phrase.FindTranslation(currentLanguage);

							// If it exists, add it
							if (translation != null)
							{
								currentTranslations.Add(phraseName, translation);
							}
						}
					}
				}

				// Add all languages to currentLanguages
				if (localization.Languages != null)
				{
					for (var j = localization.Languages.Count - 1; j >= 0; j--)
					{
						var language = localization.Languages[j];

						if (currentLanguages.Contains(language) == false)
						{
							currentLanguages.Add(language);
						}
					}
				}
			}

			// Notify changes?
			if (OnLocalizationChanged != null)
			{
				OnLocalizationChanged();
			}
		}

		// Set the instance, merge old instance, and update translations
		protected virtual void OnEnable()
		{
			AllLocalizations.Add(this);

			// Load saved language?
			if (string.IsNullOrEmpty(currentLanguage) == true)
			{
				if (SaveLanguage == true)
				{
					currentLanguage = PlayerPrefs.GetString("LeanLocalization.CurrentLanguage");
				}
			}

			// Find language by culture?
			if (string.IsNullOrEmpty(currentLanguage) == true)
			{
				if (Cultures != null)
				{
					switch (DetectLanguage)
					{
						case DetectType.SystemLanguage:
						{
							currentLanguage = FindLanguageName(Application.systemLanguage.ToString());
						}
						break;

						case DetectType.CurrentCulture:
						{
							var culture = System.Globalization.CultureInfo.CurrentCulture;

							if (culture != null)
							{
								currentLanguage = FindLanguageName(culture.Name);
							}
						}
						break;

						case DetectType.CurrentUICulture:
						{
							var culture = System.Globalization.CultureInfo.CurrentUICulture;

							if (culture != null)
							{
								currentLanguage = FindLanguageName(culture.Name);
							}
						}
						break;
					}
				}
			}

			// Use default language?
			if (string.IsNullOrEmpty(currentLanguage) == true)
			{
				currentLanguage = DefaultLanguage;
			}

			UpdateTranslations();
		}

		// Unset instance?
		protected virtual void OnDisable()
		{
			AllLocalizations.Remove(this);

			UpdateTranslations();
		}
#if UNITY_EDITOR
		// Inspector modified?
		protected virtual void OnValidate()
		{
			UpdateTranslations();
		}
#endif
		private string FindLanguageName(string alias)
		{
			for (var i = Languages.Count - 1; i >= 0; i--)
			{
				var language = Languages[i];

				if (language == alias)
				{
					return language;
				}
			}

			for (var i = Cultures.Count - 1; i >= 0; i--)
			{
				var culture = Cultures[i];

				if (culture != null && culture.Alias == alias)
				{
					return culture.Language;
				}
			}

			return null;
		}
	}
}