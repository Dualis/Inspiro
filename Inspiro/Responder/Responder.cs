using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Responders
{
    public abstract class Responder
    {

        public abstract string greeting();

        public abstract string farewell();

        public abstract string positive();

        public abstract string neutral();

        public abstract string negative();

        public abstract string positiveImageUrl();

        public abstract string neutralImageUrl();

        public abstract string negativeImageUrl();

        public abstract string unknown();

        public abstract string question();

        public abstract string affirmative();

        public abstract string getName();

        public static Responder GetResponder(string name)
        {
            if(name == null) return BoringResponder.getInstance();
            if      (name.ToLower().StartsWith("arn")) return ArnieResponder.getInstance();
            else if (name.ToLower().StartsWith("marx") || name.ToLower().StartsWith("karl")) return MarxResponder.getInstance();
            else return BoringResponder.getInstance();
        }
    }
}