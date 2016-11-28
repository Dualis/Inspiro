using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responder
{
    public class BoringResponder:Responder
    {
        
        private string greetingStr = "Hello, I am a generic bot";
        private string farewellStr = "Have a nice day";
        private string positiveStr = "This is good";
        private string neutralStr = "This is okay";
        private string negativeStr = "This is bad";
        private string unknownStr = "Sorry, I don't understand";
        private string questionStr = "Please, ";
        private string affirmativeStr = "Affirmative";
        private static BoringResponder instance;

        public static Responder getInstance()
        {
            if (instance != null) return instance;

            instance = new BoringResponder();
            return instance;
        }

        public override string greeting()
        {
            return greetingStr;
        }

        public override string farewell()
        {
            return farewellStr;
        }

        public override string positive()
        {
            return positiveStr;
        }

        public override string neutral()
        {
            return neutralStr;
        }

        public override string negative()
        {
            return negativeStr;
        }

        public override string unknown()
        {
            return unknownStr;
        }

        public override string question()
        {
            return questionStr;
        }

        public override string affirmative()
        {
            return affirmativeStr;
        }

    }
}