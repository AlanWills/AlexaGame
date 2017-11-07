using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response.Directive;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda1
{
    public class Function
    {
        //public string FunctionHandler(JSONObject input, ILambdaContext context)
        //{
        //    return input.StarSign + "Blob";
        //}

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            //string starSign = (input.Request as IntentRequest).Intent.Slots["starsign"].Value.ToLower();
            //string text = "";
            //if (starSign == "cancer")
            //{
            //    text = "Good things";
            //}
            //else if (starSign == "aquarius")
            //{
            //    text = "Bad things";
            //}
            //else if (starSign == "leo")
            //{
            //    text = "Bad things";
            //}

            // Now try with google drive file - won't work though, as they're not audio files
            // Maybe we should host on our own server?
            // Or on github?
            return ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, "https://opengameart.org/sites/default/files/Mountain%20_0.mp3", "audio-token");
        }
    }
}
