namespace Einsatzplanung.Util.Services;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

internal class YmlService {

	public string[] Keys => bufferedKeyValuePairs.Keys.ToArray();

	public string this[string index] {
		set => SetValue(index, value);
		get => GetValue(index);
	}

	private Dictionary<string, string> bufferedKeyValuePairs = [];

	public unsafe void ReadFromFile(string filePath) {
        //Console.WriteLine($"reading from file at {filePath}");
        byte* file = FileService.LoadFileUnsafe(filePath, out int fileSize);
		char* text = FileService.DecodeBufferAnsi(file, fileSize, out int charCount);
		NativeMemory.Free(file);
		bufferedKeyValuePairs = [];
		char* bufferStartPos = text;
		char* bufferEndPos = text+charCount;
		char* currentSubStringStart = bufferStartPos;
		char* currentchar = bufferStartPos;
		while (currentchar < bufferEndPos) {
			if (*currentchar == '\r' | *currentchar == '\n') {
				currentchar++;
                currentSubStringStart = currentchar;
                continue;
			}
            int keyCharCount = 0;
			while (*currentchar != ':') {
				currentchar++;
				keyCharCount++;
				if (currentchar >= bufferEndPos)
					break;
			}
            if (currentchar >= bufferEndPos)
                break;
            currentchar--;
            string key = new(currentSubStringStart, 0, keyCharCount);
            int valueCharCount = 0;
			currentchar += 2;
			currentSubStringStart = currentchar;
            while (*currentchar != '\r' && *currentchar != '\n') {
                currentchar++;
                valueCharCount++;
                if (currentchar >= bufferEndPos) {
                    break;
				}
            }
            if (currentchar > bufferEndPos) {
                break;
			}
            string val = new(currentSubStringStart, 0, valueCharCount);
			currentSubStringStart = currentchar;
			bufferedKeyValuePairs[key] = val;
        }
		NativeMemory.Free(text);
	}

	public void WriteTofile(string filePath) {
		List<string> pairs = [];
		foreach (var key in bufferedKeyValuePairs.Keys) {
			pairs.Add($"{key}:{bufferedKeyValuePairs[key]}");
		}
		string text = string.Join('\n', pairs);
		File.WriteAllText(filePath, text);
	}

	private string GetValue(string index) {
		if (string.IsNullOrEmpty(index))
			return "";
		if(bufferedKeyValuePairs.TryGetValue(index, out string? bufferedRes))
			return bufferedRes;
		return "";
	}

	private void SetValue(string index, string value) {
		if (string.IsNullOrEmpty(index))
			return;
		if (string.IsNullOrEmpty(value))
			return;
		bufferedKeyValuePairs[index] = value;
	}
}
