namespace StringTests;

internal class Program
{
	static void Main(string[] args)
	{
        Console.Write("\nLINQ : Display the characters and frequency of character from giving string : "); 
        Console.Write("\n----------------------------------------------------------------------------\n");	
		Console.WriteLine("Enter a string of characters: ");
		string input = Console.ReadLine() ?? string.Empty;
		Console.WriteLine($"The string length is: {input.Length}");
		var frequency = from x in input
						group x by x into c
						select new { Character = c.Key, Count = c.Count() };
		var calcLen = 0;
		foreach (var item in frequency)
		{
			calcLen += item.Count;
			Console.WriteLine($"Character: {item.Character}:: Count: {item.Count}");
		}
		Console.WriteLine($"The calculated length is: {calcLen}");
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
