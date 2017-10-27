using UnityEngine;

namespace Lean.Localization
{
	// This component will load localizations from a file
	// They must be in the format:
	// Phrase Name Here = Translation Here // Optional Comment Here
	public class LeanLocalizationLoader : MonoBehaviour
	{
		[Tooltip("The LeanLocalization target the translations will be loaded into. If this is empty, the first in the scene will automatically be used. If there are none in the scene, one will be created")]
		public LeanLocalization Target;

		[Tooltip("The file with all the translations in")]
		public TextAsset Source;

		[Tooltip("The language of the translations in the source file")]
		[LeanLanguageName]
		public string SourceLanguage;

		[Tooltip("The string separating the phrase name from the translation")]
		public string Separator = " = ";

		[Tooltip("The string denoting a new line within a translation")]
		public string NewLine = "\\n";

		[Tooltip("The string separating the translation from the comment (empty = no comments)")]
		public string Comment = " // ";

		// The characters used to detect a newline
		private static readonly char[] newlineCharacters = new char[] { '\r', '\n' };

		protected virtual void Start()
		{
			LoadFromSource();
		}

		[ContextMenu("Load From Source")]
		public void LoadFromSource()
		{
			if (Source != null && string.IsNullOrEmpty(SourceLanguage) == false)
			{
				var target = Target;

				// Automatically get first LeanLocalization?
				if (target == null && LeanLocalization.AllLocalizations.Count > 0)
				{
					target = LeanLocalization.AllLocalizations[0];
				}

				// Create a new LeanLocalization?
				if (target == null)
				{
					target = new GameObject("LeanLocalization").AddComponent<LeanLocalization>();
				}

				// Split file by line and loop through them all
				var lines = Source.text.Split(newlineCharacters, System.StringSplitOptions.RemoveEmptyEntries);

				for (var i = 0; i < lines.Length; i++)
				{
					var line        = lines[i];
					var equalsIndex = line.IndexOf(Separator);

					if (equalsIndex != -1)
					{
						var phraseName        = line.Substring(0, equalsIndex).Trim();
						var phraseTranslation = line.Substring(equalsIndex + Separator.Length).Trim();

						// Replace newline markers with actual newlines
						if (string.IsNullOrEmpty(NewLine) == false)
						{
							phraseTranslation = phraseTranslation.Replace(NewLine, System.Environment.NewLine);
						}

						// Does this entry have a comment?
						if (string.IsNullOrEmpty(Comment) == false)
						{
							var commentIndex = phraseTranslation.LastIndexOf(Comment);

							if (commentIndex != -1)
							{
								phraseTranslation = phraseTranslation.Substring(0, commentIndex).Trim();
							}
						}

						// Find or add the translation for this phrase
						var translation = target.AddTranslation(SourceLanguage, phraseName);

						// Set the translation text for this phrase
						translation.Text = phraseTranslation;
					}
				}

				// Update translations?
				if (LeanLocalization.CurrentLanguage == SourceLanguage)
				{
					LeanLocalization.UpdateTranslations();
				}
			}
		}

		// NOTE: Saving text files like this doesn't work with webplayer build set
		// If this doesn't work for other platforms then add them to the list
#if UNITY_EDITOR && !UNITY_WEBPLAYER
		[ContextMenu("Export Text Asset")]
		public void ExportTextAsset()
		{
			var localization = GetComponent<LeanLocalization>();

			if (string.IsNullOrEmpty(SourceLanguage) == false)
			{
				if (localization.Languages.Contains(SourceLanguage) == true)
				{
					// Find where we want to save the file
					var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Export Text Asset", SourceLanguage, "txt", "");

					// Make sure we didn't cancel the panel
					if (string.IsNullOrEmpty(path) == false)
					{
						var data = "";

						// Add all phrase names and existing translations to lines
						for (var i = localization.Phrases.Count - 1; i >= 0; i--)
						{
							var phrase      = localization.Phrases[i];
							var translation = phrase.FindTranslation(SourceLanguage);

							data += phrase.Name + Separator;

							if (translation != null)
							{
								var text = translation.Text;

								// Replace all new line permutations with the new line token
								text = text.Replace("\r\n", NewLine);
								text = text.Replace("\n\r", NewLine);
								text = text.Replace("\n", NewLine);
								text = text.Replace("\r", NewLine);

								data += text;
							}

							if (i > 0)
							{
								data += System.Environment.NewLine; //data += "\r\n";
							}
						}

						// Write text to file
						using (var file = System.IO.File.OpenWrite(path))
						{
							var encoding = new System.Text.UTF8Encoding();
							var bytes    = encoding.GetBytes(data);

							file.Write(bytes, 0, bytes.Length);
						}

						// Import asset into project
						UnityEditor.AssetDatabase.ImportAsset(path);

						// Replace Soure with new Text Asset?
						var textAsset = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));

						if (textAsset != null)
						{
							Source = textAsset;

							UnityEditor.EditorGUIUtility.PingObject(textAsset);

							UnityEditor.EditorUtility.SetDirty(this);
						}
					}
				}
			}
			else
			{
				Debug.LogError("Can't export Text Asset for language that doesn't exist in the current localization");
			}
		}
#endif
	}
}