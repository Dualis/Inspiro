using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responders
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

        private string positiveImage = "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png";
        private string neutralImage = "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png";
        private string negativeImage = "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png";

        private static BoringResponder instance;

        public override string getName()
        {
            return "Anonymous";
        }

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