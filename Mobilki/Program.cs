namespace Mobilki
{
    public class Kalkulator
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HI!");
        }

        public static int Dodawanie(int a, int b)
        {
            return a + b;
        }

        public static int Odejmowanie(int a, int b)
        {
            return a - b;
        }

        public static int Mnozenie(int a, int b)
        {
            return a * b;
        }

        public static int Dzielenie(int a, int b)
        {
            if (b == 0)
            {
                throw new ArgumentException("NIE DZIEL PRZEZ 0!");
            }
            return a / b;
        }
    }
}