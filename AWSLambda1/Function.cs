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
using Alexa.NET.Response.Ssml;
using System.Xml.Linq;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda1
{
    public class Function
    {
        // IMAGINE THIS:
        // WE BUILD AN AUDIO ENGINE THAT CAN CONVERT XML INTO A GAME
        // WE CAN THEN OBTAIN A SCRIPT FROM A SERVER
        // AND THEN PLAY THE CONVERTER XML GAME
        // CAN CREATE WRAPPER SOFTWARE TO HELP THE GIRLS TO WRITE STUFF

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            // Tokens cannot be the same otherwise things will not work
            context.Logger.LogLine("Request Type: " + input.GetRequestType().Name);

            if (input.Request is IntentRequest && 
                (input.Request as IntentRequest).Intent != null)
            {
                IntentRequest request = input.Request as IntentRequest;
                context.Logger.LogLine("Request Intent: " + request.Intent.Name);

                switch ((input.Request as IntentRequest).Intent.Name)
                {
                    case "AnswerIntent":
                    {
                        string answer = request.Intent.Slots.ContainsKey("answer") ? request.Intent.Slots["answer"].Value.ToLower() : "";
                        context.Logger.LogLine("Answer " + answer);

                        Speech speech = new Speech();
                        speech.Elements.Add(new Audio(answer == "a" ? GetAnswerRightUrl("FlowerQuestion") : GetAnswerWrongUrl("FlowerQuestion")));

                        context.Logger.LogLine(speech.ToXml());
                        return ResponseBuilder.Tell(speech);
                    }

                    default:
                    {
                        return ResponseBuilder.Empty();
                    }
                }
            }
            else if (input.Request is LaunchRequest)
            {
                context.Logger.LogLine("Playing intro");
                return PlayIntroduction(context.Logger);
            }
            
            else if (input.Request is SessionEndedRequest)
            {
                context.Logger.LogLine("Session ended");
            }

            return ResponseBuilder.Empty();
        }

        private SkillResponse PlayIntroduction(ILambdaLogger logger)
        {
            // build the speech response 
            Speech speech = new Speech();
            speech.Elements.Add(new Sentence("Launching Word Play"));
            speech.Elements.Add(new Audio(GetUrl("Introduction")));
            speech.Elements.Add(new Audio(GetUrl("FlowerQuestion")));

            logger.LogLine(speech.ToXml());

            // create the response using the ResponseBuilder
            SkillResponse response = ResponseBuilder.Tell(speech);
            response.Response.ShouldEndSession = false;

            return response;
        }

        private XElement GetAudioFilesElement(string elementName)
        {
            XDocument document = XDocument.Load(File.OpenText("AudioData.xml"));
            return document.Element("AudioFiles").Element(elementName);
        }

        private string GetUrl(string elementName)
        {
            return GetAudioFilesElement(elementName).Attribute("source").Value;
        }

        private string GetAnswerWrongUrl(string questionElementName)
        {
            return GetAudioFilesElement(questionElementName).Element("AnswerWrong").Attribute("source").Value;
        }

        private string GetAnswerRightUrl(string questionElementName)
        {
            return GetAudioFilesElement(questionElementName).Element("AnswerRight").Attribute("source").Value;
        }
    }
}
