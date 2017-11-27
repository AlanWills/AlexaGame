using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda1
{
    public class RegisteredGameInfo
    {
        #region Properties and Fields

        /// <summary>
        /// The name of the game which we will use to match an intent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL to the game file that our engine will download to reconstruct the game.
        /// </summary>
        public string GameFileURL { get; set; }

        #endregion

        /// <summary>
        /// Load the game file from the URL and create a game instance using it's information.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<Game> LoadGame(ILambdaLogger logger)
        {
            Game game = null;
            HttpWebRequest downloadFilesRequest = (HttpWebRequest)WebRequest.Create(GameFileURL);

            // Execute the request
            using (HttpWebResponse response = (HttpWebResponse)(await downloadFilesRequest.GetResponseAsync()))
            {
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream))
                    {
                        string bufAsString = reader.ReadToEnd();
                        game = JsonConvert.DeserializeObject<Game>(bufAsString);

                        logger.LogLine("Loaded Game");
                    }
                }
            }

            return game;
        }
    }
}
