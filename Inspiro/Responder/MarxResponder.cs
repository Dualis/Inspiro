using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responders
{
    public class MarxResponder:Responder
    {
        private string greetingStr = "I am nothing but I must be everything";
        private string farewellStr = "The last capitalist we hang shall be the one who sold us the rope";
        private string positiveStr = "The less you are, the more you have";
        private string neutralStr = "The true law of Economics is chance";
        private string negativeStr = "Rise up, and seize the means of production";
        private string unknownStr = "Reason has always existed, but not always in a reasonable form";
        private string questionStr = "Comrade, ";
        private string affirmativeStr = "I have, of course, so worded my proposition as to be right either way";
        private string positiveImage = "https://upload.wikimedia.org/wikipedia/commons/5/50/Marx_color2.jpg";
        private string neutralImage = "https://upload.wikimedia.org/wikipedia/commons/5/50/Marx_color2.jpg";
        private string negativeImage = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Tesserapcd21.jpg";

        public override string getName()
        {
            return "Karl Marx";
        }

        private static MarxResponder instance;
       

        public static Responder getInstance()
        {
            if (instance != null) return instance;

            instance = new MarxResponder();
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

        public override string positiveImageUrl()
        {
            return neutralImage;
        }

        public override string neutralImageUrl()
        {
            return neutralImage;
        }

        public override string negativeImageUrl()
        {
            return negativeImage;
        }
    }
}