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
        private const string Question = "https://s3-eu-west-1.amazonaws.com/flowerquestion/FLOWER+Q.mp3";
        private const string YesResponse = "https://s3-eu-west-1.amazonaws.com/flowerquestion/WELL_DONE_FLOWER.mp3";
        private const string NoResponse = "https://s3-eu-west-1.amazonaws.com/flowerquestion/NOPE+WRONG+FLOWER.mp3";

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            context.Logger.LogLine((input.GetRequestType().Name));
            context.Logger.LogLine((input.Request.GetType().Name));

            if (input.Request is IntentRequest && 
                (input.Request as IntentRequest).Intent != null)
            {
                IntentRequest request = input.Request as IntentRequest;
                context.Logger.LogLine(request.Intent.Name);

                switch ((input.Request as IntentRequest).Intent.Name)
                {
                    case "AnswerIntent":
                    {
                        string answer = request.Intent.Slots.ContainsKey("answer") ? request.Intent.Slots["answer"].Value.ToLower() : "";

                        context.Logger.LogLine("Answer " + answer);
                        return ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, answer == "a" ? YesResponse : NoResponse, "");
                    }
                    case "LaunchIntent":
                    {
                        context.Logger.LogLine("Playing question");
                        SkillResponse response = ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, Question, "");
                        return response;
                    }
                    case BuiltInIntent.Cancel:
                    case BuiltInIntent.Stop:
                    case BuiltInIntent.Pause:
                    {
                        context.Logger.LogLine("Stopping audio");
                        return ResponseBuilder.AudioPlayerClearQueue(ClearBehavior.ClearAll);
                    }
                }
            }
            else if (input.Request is LaunchRequest)
            {
                context.Logger.LogLine("Playing question");
                SkillResponse response = ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, Question, "");
                return response;
            }

            return ResponseBuilder.Empty();
        }
    }
}
