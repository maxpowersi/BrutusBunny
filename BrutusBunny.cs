using System.IO;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BrutusBunny
{
    public class BrutusBunny
    {
        private string quack = "Q ";
        private string mask = "{{REPLACE}}";

        public BrutusBunny()
        {
            Strategy = new BrutusBunnyHeaderStrategy();
        }

        public BrutusBunny(BrutusBunny original)
        {
            Header = original.Header;
            Template = original.Template;
            Footer = original.Footer;
            PasswordIterator = original.PasswordIterator;
            Strategy = original.Strategy;
        }

        #region Internal Properties

        private string Header { get; set; }

        private string Template { get; set; }

        private IBashBrutusStrategy Strategy { get; set; }

        private string Footer { get; set; }

        private Func<IEnumerable<string>> PasswordIterator { get; set; }

        #endregion

        #region Actions

        #region Interface 

        private void AddToHeader(string command)
        {
            Header += command;
        }

        private void AddToFooter(string command)
        {
            Footer += command;
        }

        private void AddToTemplate(string command)
        {
            Template += command;
        }

        #endregion

        private void AddCommand(string command)
        {
            Strategy.AddCommand(command + "\n", this);
        }

        public BrutusBunny Comment(string text)
        {
            AddCommand("#  " + text);
            return new BrutusBunny(this);
        }

        public BrutusBunny AttackMode(string mode)
        {
            AddCommand("ATTACKMODE " + mode);
            return new BrutusBunny(this);
        }

        public BrutusBunny StartAttack(Func<IEnumerable<string>> passwordIterator)
        {
            PasswordIterator = passwordIterator;
            Strategy = new BrutusBunnyTemplateStrategy();
            return new BrutusBunny(this);
        }

        public BrutusBunny Led(string color, string state)
        {
            AddCommand("LED "  + color + " " + state);
            return new BrutusBunny(this);
        }

        public BrutusBunny Enter()
        {
            AddCommand(quack + "ENTER");
            return new BrutusBunny(this);
        }

        public BrutusBunny Down()
        {
            AddCommand(quack + "DOWN");
            return new BrutusBunny(this);
        }

        public BrutusBunny Up()
        {
            AddCommand(quack + "UP"); 
            return new BrutusBunny(this);
        }

        public BrutusBunny Left()
        {
            AddCommand(quack + "LEFT");
            return new BrutusBunny(this);
        }

        public BrutusBunny Right()
        {
            AddCommand(quack + "RIGHT");
            return new BrutusBunny(this);
        }

        public BrutusBunny BackSpace()
        {
            AddCommand(quack + "BACKSPACE");
            return new BrutusBunny(this);
        }

        public BrutusBunny Writte(string text)
        {
            AddCommand(quack + "STRING " + text);
            return new BrutusBunny(this);
        }

        public BrutusBunny Writte()
        {
            AddCommand(quack + "STRING " + mask);
            return new BrutusBunny(this);
        }

        public BrutusBunny Delay(int time)
        {
            AddCommand(quack + "DELAY " + time.ToString());
            return new BrutusBunny(this);
        }

        public BrutusBunny EndAttack()
        {
            Strategy = new BrutusBunnyFooterStrategy();
            return new BrutusBunny(this);
        }

        #endregion

        #region Output

        public void Save(string outputPath)
        {

            using (var file = File.CreateText(Path.Combine(outputPath, "payload.txt")))
            {
                file.Write(this.GetContent());
                file.Flush();
                file.Close();
            }
        }

        public void Save()
        {

            Save(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }

        public string GetContent()
        {
            var content = Header;

            foreach(var e in PasswordIterator.Invoke())
                content += Template.Replace(mask, e);
           
            content += Footer;
            return content;
        }

        #endregion

        #region internal class

        public static class AttackModes
        {
            public static string HID { get { return "HID"; } }

            public static string STORAGE { get { return "STORAGE"; } }

            public static string SERIAL { get { return "SERIAL"; } }

            public static string RNDIS_ETHERNET { get { return "RNDIS_ETHERNET"; } }

            public static string ECM_ETHERNET { get { return "ECM_ETHERNET"; } }
        }

        public static class LedColor
        {
            public static string Red { get { return "R"; } }

            public static string Green { get { return "G"; } }

            public static string Blue { get { return "B"; } }

            public static string Yellow { get { return "Y"; } }

            public static string Cyan { get { return "C"; } }

            public static string Magenta { get { return "M"; } }

            public static string White { get { return "W"; } }
        }

        public static class LedStatus
        {
            public static string Solid { get { return "SOLID"; } }

            public static string BlinkFast { get { return "FAST"; } }

            public static string BlinkVeryFast { get { return "VERYFAST"; } }

            public static string BlinkSlow { get { return "SLOW"; } }
        }

        private interface IBashBrutusStrategy
        {
            void AddCommand(string command, BrutusBunny bb);
        }

        private class BrutusBunnyHeaderStrategy : IBashBrutusStrategy
        {
            public void AddCommand(string command, BrutusBunny bb)
            {
                bb.AddToHeader(command);
            }
        }

        private class BrutusBunnyTemplateStrategy : IBashBrutusStrategy
        {
            public void AddCommand(string command, BrutusBunny bb)
            {
                bb.AddToTemplate(command);
            }
        }

        private class BrutusBunnyFooterStrategy : IBashBrutusStrategy
        {
            public void AddCommand(string command, BrutusBunny bb)
            {
                bb.AddToFooter(command);
            }
        }

        #endregion

        #region DiccionaryHelper

        public static IEnumerable<string> CreateDictionaryPinFromStartToEnd(int start, int end)
        {
            var format = new String('0', end.ToString().Count());

            for (var i = start; i <= end; i++)
                yield return String.Format("{0:" + format + "}", i);
        }

        public static IEnumerable<string> CreateDictionaryPin(int start, int lenght)
        {
            var format = new String('0', lenght);
            var max = new String('9', lenght); ;

            for (var i = start; i <= int.Parse(max); i++)
                yield return String.Format("{0:"+ format+ "}", i);
        }
        public static IEnumerable<string> CreateDictionaryPin(int lenght)
        {
            return CreateDictionaryPin(0, lenght);
        }

        public static IEnumerable<string> CreateDictionaryFromFile(string path)
        {
            return File.ReadLines(path);
        }

        public static IEnumerable<string> CreateDictionaryFromInput(params string[] words)
        {
            return words;
        }

        #endregion
    }
}