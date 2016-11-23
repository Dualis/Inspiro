using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responder
{
    public class Responder
    {
        private string greetingStr = "Hello, I am a generic bot";
        private string farewellStr = "Have a nice day";
        private string positiveStr = "This is good";
        private string neutralStr = "This is okay";
        private string negativeStr = "This is bad";
        private string unknownStr = "Sorry, I don't understand";
        private string questionStr = "Please, ";
        private string affirmativeStr = "Affirmative";
        public string greeting()
        {
            return greetingStr;
        }

        public string farewell()
        {
            return farewellStr;
        }

        public string positive()
        {
            return positiveStr;
        }

        public string neutral()
        {
            return neutralStr;
        }

        public string negative()
        {
            return negativeStr;
        }

        public string unknown()
        {
            return unknownStr;
        }

        public string question()
        {
            return questionStr;
        }

        public string affirmative()
        {
            return affirmativeStr;
        }
        public static Responder getResponder(string name)
        {
            return new Responder();
        }
    }
}