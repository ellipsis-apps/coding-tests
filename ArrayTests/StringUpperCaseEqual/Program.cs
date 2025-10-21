namespace StringUpperCaseEqual;

internal class Program
{
	static void Main(string[] args)
	{
		Console.Write("\nLINQ : Find the uppercase words in a string : ");
		Console.Write("\n----------------------------------------------\n");

		string strNew; // Declare a string variable named strNew

		// Ask the user to input a string
		Console.Write("Input the string using spaces to separate words: ");
		strNew = Console.ReadLine();

		if (string.IsNullOrEmpty(strNew))
		{
			Console.WriteLine("No input provided. Exiting the program.");
			Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
			Console.ReadKey();
			return; // Exit the program if no input is provided
		}

		// Call the WordFilt method to filter uppercase words in the input string
		var ucWord = WordFilt(strNew);

		// Display the uppercase words found in the input string
		Console.WriteLine("The UPPER CASE words are:");
		foreach (string strRet in ucWord)
		{
			Console.WriteLine($"{strRet}");
		}

		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}

	// Define a method named WordFilt that takes a string parameter and returns IEnumerable<string>
	static IEnumerable<string> WordFilt(string mystr)
	{
		// Split the input string into words, filter and retrieve uppercase words
		var upWord = mystr.Split(' ')
					.Where(x => string.Equals(x, x.ToUpper(),
					StringComparison.Ordinal));

		return upWord; // Return the uppercase words
	}
}

