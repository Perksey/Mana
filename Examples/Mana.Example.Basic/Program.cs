namespace Mana.Example.Basic
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var window = new ManaWindow())
            {
                window.Run(new ExampleGame());                
            }
        }
    }
}
