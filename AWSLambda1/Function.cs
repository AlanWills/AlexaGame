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
using Newtonsoft.Json;
using System.Net;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda1
{
    public class Function
    {
        // Move the game name, current question and score to a static object for ease of use

        /// <summary>
        /// Download all of our games and process them for use.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            List<RegisteredGameInfo> gameInfoList = await GetRegisteredGames(context.Logger);

            // Tokens cannot be the same otherwise things will not work
            context.Logger.LogLine("Request Type: " + input.GetRequestType().Name);

            if (input.Request is IntentRequest && 
                (input.Request as IntentRequest).Intent != null)
            {
                IntentRequest request = input.Request as IntentRequest;
                context.Logger.LogLine("Request Intent: " + request.Intent.Name);

                switch ((input.Request as IntentRequest).Intent.Name)
                {
                    case "PlayGameIntent":
                    {
                        string requestedGame = request.Intent.Slots.ContainsKey("game") ? request.Intent.Slots["game"].Value.ToLower() : "";
                        context.Logger.LogLine("Request Game " + requestedGame);

                        Game game = await gameInfoList.Find(x => x.Name.ToLower() == requestedGame).LoadGame(context.Logger);
                        SkillResponse response = game.StartGame(context.Logger);
                        response.SessionAttributes = response.SessionAttributes ?? new Dictionary<string, object>();
                        response.SessionAttributes.Add("GameName", requestedGame);
                        response.SessionAttributes.Add("CurrentQuestion", (long)0);
                        response.SessionAttributes.Add("Score", (long)0);

                        return response;
                    }

                    case "AnswerIntent":
                    {
                        string answer = request.Intent.Slots.ContainsKey("answer") ? request.Intent.Slots["answer"].Value.ToLower() : "";
                        context.Logger.LogLine("Answer " + answer);

                        string gameName = input.Session.Attributes["GameName"] as string;

                        Game game = await gameInfoList.Find(x => x.Name.ToLower() == gameName).LoadGame(context.Logger);
                        Tuple<bool, SkillResponse> correctAnswer = game.AnswerQuestion(0, answer, context.Logger);

                        // Make this building up of session attributes much safer - add attributes if not present
                        // This currently relies on the previous response doing it correctly
                        correctAnswer.Item2.SessionAttributes = input.Session.Attributes;
                        long q = (long)input.Session.Attributes["CurrentQuestion"];
                        correctAnswer.Item2.SessionAttributes["CurrentQuestion"] = ++q;

                        context.Logger.LogLine("Current Question " + q);

                        if (correctAnswer.Item1)
                        {
                            long score = (long)input.Session.Attributes["Score"];
                            correctAnswer.Item2.SessionAttributes["Score"] = ++score;

                            context.Logger.LogLine("Current Score " + score);
                        }

                        return correctAnswer.Item2;
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
                return SayRegisteredGames(gameInfoList);
            }
            
            else if (input.Request is SessionEndedRequest)
            {
                context.Logger.LogLine("Session ended");
            }

            return ResponseBuilder.Empty();
        }

        private async Task<List<RegisteredGameInfo>> GetRegisteredGames(ILambdaLogger logger)
        {
            List<RegisteredGameInfo> gameInfoList = new List<RegisteredGameInfo>();

            // https://s3-eu-west-1.amazonaws.com/word-play-games/RegisteredGames.json
            HttpWebRequest downloadFilesRequest = (HttpWebRequest)WebRequest.Create("https://s3-eu-west-1.amazonaws.com/word-play-games/RegisteredGames.json");

            // Execute the request
            using (HttpWebResponse response = (HttpWebResponse)(await downloadFilesRequest.GetResponseAsync()))
            {
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string bufAsString = reader.ReadToEnd();
                        gameInfoList = JsonConvert.DeserializeObject<List<RegisteredGameInfo>>(bufAsString);

                        logger.LogLine("Discovered games:");
                        foreach (RegisteredGameInfo gameInfo in gameInfoList)
                        {
                            logger.LogLine(gameInfo.Name);
                        }
                    }
                }
            }

            return gameInfoList;
        }

        private SkillResponse SayRegisteredGames(List<RegisteredGameInfo> gameInfoList)
        {
            // build the speech response 
            Speech speech = new Speech();
            speech.Elements.Add(new Sentence("Welcome to Word Play."));
            speech.Elements.Add(new Sentence("The available games to play are: "));

            foreach (RegisteredGameInfo info in gameInfoList)
            {
                speech.Elements.Add(new Sentence(info.Name));
            }

            // create the response using the ResponseBuilder
            SkillResponse response = ResponseBuilder.Tell(speech);
            response.Response.ShouldEndSession = false;

            return response;
        }
    }
}
