using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responders
{
    public class ArnieResponder:Responder
    {
        private string greetingStr = "None of my rules of success will work unless you do";
        private string farewellStr = "I'll be back!";
        private string positiveStr = "Take one dollar and turn it into two.";
        private string neutralStr = "Well, you know, I'm the forever optimist";
        private string negativeStr = "Nobody in Hollywood wins all the time. At some point, you're bound to get a beating.";
        private string unknownStr = "I never listen that you can’t";
        private string questionStr = "Hey you, ";
        private string affirmativeStr = "No Problemo";

        private string positiveImage = "http://www.ew.com/sites/default/files/i/2015/03/02/arnold-schwarzenegger_0.jpg";
        private string neutralImage = "http://media.schwarzenegger.ca/arnold/Schwarzenegger.jpg";
        private string negativeImage = "https://upload.wikimedia.org/wikipedia/commons/b/be/Arnold_Schwarzenegger_2,_2012.jpg";

        public override string getName()
        {
            return "Arnold Schwarzenegger";
        }
        private static ArnieResponder instance;

        public static Responder getInstance()
        {
            if (instance != null) return instance;

            instance = new ArnieResponder();
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
            return positiveImage;
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