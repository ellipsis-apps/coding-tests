namespace DaysOfWeekLINQ;

internal class Program
{
	static void Main(string[] args)
	{
		string[] dayArr = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
		string[] weekEndDays = { "Sunday", "Saturday" };
		var days = from day in dayArr
				   where !weekEndDays.Contains(day)
				   select day;
		foreach (var day in days)
		{
			Console.WriteLine($"Day: {day}");
		}
		Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
		Console.ReadKey();
	}
}
