/*MIT License

Copyright (c) 2021 NustyFrozen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BeatSaverSharp
{
    #region beatsaver structs & enums
    [Serializable]
    public struct userDiffStats
    {
        int total;
        int easy;
        int normal;
        int hard;
        int expert;
        int expertPlus;
    }
    [Serializable]
    public struct userData
    {
        public int id;
        public string name;
        public string hash;
        public string avatar;
        public userDiffStats diffStats;
    }
    [Serializable]
    public struct Score
    {
        public long playerId;
        public string name;
        public int rank;
        public long score;
        public double pp;
        public List<string> mods;
    }
    [Serializable]
    public struct LeaderBoard
    {
        public bool ranked;
        public int uid;
        public List<Score> scores;
        public bool mods;
        public bool valid;
    }
    [Serializable]
    public struct userDataStats
    {
        public int totalUpvotes;
        public int totalDownvotes;
        public int totalMaps;
        public int rankedMaps;
        public double avgBpm;
        public double avgScore;
        public double avgDuration;
        public string firstUpload;
        public string lastUpload;

    }
    [Serializable]
    public struct customData
    {
        public string syncURL;
    }
    [Serializable]
    public struct song
    {
        public string key;
        public string hash;
        public string songName;
    }
    [Serializable]
    public struct playList
    {
        public string playlistTitle;
        public string playlistAuthor;
        public string playlistDescription;
        public string image; //base64
        public customData customData;
        public List<song> songs;

    }
    [Serializable]
    public struct Beatmap
    {
        public string id;
        public string name;
        public string description;
        public Uploader uploader;
        public Stats stats;
        public string uploaded;
        public bool automapper;
        public bool ranked;
        public bool qualified;
        public List<metadata> versions;

    }
    [Serializable]
    public enum Difficulty
    {
        expertPlus = 9,
        expert = 7,
        hard = 5,
        normal = 3,
        easy = 1
    }
    [Serializable]
    public struct beatsaverDateFormat
    {
        private string year;
        private string month;
        private string day;
        public string getDate()
        {
            return year + "-" + month + "-" + day;
        }
        public beatsaverDateFormat(int day, int month, int year)
        {
            this.month = month.ToString();
            this.day = day.ToString();
            this.year = year.ToString();
            if (this.month.Length != 2)
                this.month = "0" + this.month;
            if (this.day.Length != 2)
                this.day = "0" + this.day;
        }
    }
    [Serializable]
    public struct Uploader
    {
        public int id;
        public string name;
        public string hash;
        public string avatar;
    }
    [Serializable]
    public struct Stats
    {
        public int plays;
        public int downloads;
        public int upvotes;
        public int downvotes;
        int score;
    }
    [Serializable]
    public enum filters
    {
        automapper = 0,
        chroma = 1,
        noodle = 2,
        cinema = 3,
        ranked = 4,
        fullspread = 5
    }
    [Serializable]
    public struct metadata
    {
        public string hash;
        public string state;
        public string createdAt;
        public int sageScore;
        public diff[] diffs;
        public string downloadURL;
        public string coverURL;
        public string previewURL;
    }
    [Serializable]
    public struct diff
    {
        public double njs;
        public double offset;
        public int notes;
        public int bombs;
        public int obstacles;
        public double nps;
        public double length;
        public string characteristic;
        public string difficulty;
        public int events;
        public bool chroma;
        public bool me;
        public bool ne;
        public bool cinema;
        public double seconds;

    }
    [Serializable]
    public struct parity
    {
        public int errors;
        public int warns;
        public int resets;
    }
    [Serializable]
    public struct sortOrder
    {
        public static string Relevance = "Relevance";
        public static string Latest = "Latest";
        public static string Rating = "Rating";
    }
    [Serializable]
    public struct searchResults
    {
        public Beatmap[] docs;
    }
    #endregion
    /// <summary>
    /// this is where the good functions are :),
    /// everything is async besides getting a map's scores
    /// </summary>
    public class BeatSaverAPI
    {
        #region siteEndPoints
        string ROOT = "beatsaver.com/";
        string APIRoot = "api/";
        string SearchRoot = "search/";
        string textRoot = "text/";
        string MapsRoot = "maps/";
        string idRoot = "id/";
        string usersROOT = "users/";
        string uploaderROOT = "uploader/";
        string playlistROOT = "playlist";
        string hashROOT = "hash/";
        string scoresROOT = "scores/";
        #endregion


        #region class init
        string applicationname, version;



        /// <summary>
        /// creates a reference in the memory to use the api
        /// </summary>
        /// <param name="applicationName">the application/plugin which is using this class</param>
        /// <param name="version">the version of the plugin</param>
        public BeatSaverAPI(string applicationName, string version)
        {
            this.applicationname = applicationName;

            this.version = version;
        }
        #endregion

        #region helping functions
        /// <summary>
        /// supports search options by creating endpoints by filters
        /// </summary>
        /// <param name="filters">the filters which are being used</param>
        /// <returns></returns>
        private string createFiltersRoot(filters[] filters)
        {
            string results = "&";
            List<filters> temp = new List<filters>();
            temp.AddRange(filters);

            if (temp.Contains(BeatSaverSharp.filters.automapper))
                results += "automapper=true&";

            if (temp.Contains(BeatSaverSharp.filters.chroma))
                results += "chroma=true&";

            if (temp.Contains(BeatSaverSharp.filters.noodle))
                results += "noodle=true&";

            if (temp.Contains(BeatSaverSharp.filters.cinema))
                results += "cinema=true&";

            if (temp.Contains(BeatSaverSharp.filters.ranked))
                results += "ranked=true&";

            return results;
        }



        /// <summary>
        /// a function to wrap a connection with the api instead of copy pasting the same stuff over and over
        /// </summary>
        /// <param name="qwery">the qwery to send to the server</param>
        /// <returns></returns>
        private async Task<string> apicall(string qwery)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + qwery);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "[BeatSaverSharp] " + applicationname + " , " + version;

            WebResponse response = await request.GetResponseAsync();

            string respondStream = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();

            return respondStream;
        }
        #endregion


        #region searching methods (overloading)



        /// <summary>
        /// receiving a list of maps by a searched string
        /// </summary>
        /// <param name="text">what do you want to search for example: RCTTS</param>
        /// <param name="page">first page starts at 0</param>
        /// <returns></returns>
        public async Task<Beatmap[]> searchAsync(string text, int page)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + SearchRoot
                + textRoot
                + page
                + "?sortOrder=" + sortOrder.Relevance
                + "&q="
                + text
                );

            return Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs;
        }



        /// <summary>
        /// receiving a list of maps by a searched string
        /// </summary>
        /// <param name="text">what do you want to search for example: RCTTS</param>
        /// <param name="page">first page starts at 0</param>
        /// <param name="sortOrder">the sorting order, you can get it from the sortOrder struct</param>
        /// <returns></returns>
        public async Task<Beatmap[]> searchAsync(string text, int page, string sortOrder)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + SearchRoot
                + textRoot
                + page
                + "?sortOrder=" + sortOrder
                + "&q="
                + text
                );

            return Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs;
        }



        /// <summary>
        /// receiving a list of maps by a searched string
        /// </summary>
        /// <param name="text">what do you want to search for example: RCTTS</param>
        /// <param name="page">first page starts at 0</param>
        /// <param name="sortOrder">the sorting order</param>
        /// <returns></returns>
        public async Task<Beatmap[]> searchAsync(string text, int page, string sortOrder, filters[] searchFilters)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + SearchRoot
                + textRoot
                + page
                + "?sortOrder=" + sortOrder
                + createFiltersRoot(searchFilters)
                + "q="
                + text
                );
            return Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs;
        }



        /// <summary>
        /// receiving a list of maps by a searched string
        /// </summary>
        /// <param name="text">what do you want to search for example: RCTTS</param>
        /// <param name="page">first page starts at 0</param>
        /// <param name="sortOrder">the sorting order</param>
        /// <returns></returns>
        public async Task<Beatmap[]> searchAsync(string text, int page, string sortOrder, beatsaverDateFormat from, beatsaverDateFormat to)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + SearchRoot
                + textRoot
                + page
                + "?sortOrder=" + sortOrder
                + "&q="
                + text
                + "&from="
                + from.getDate()
                + "&to="
                + to.getDate()
                );
            return Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs;
        }

        #endregion

        #region instant map detals functions



        /// <summary>
        /// a function to get instantly the map by the map's key, in the case of map not found the function will return default values
        /// </summary>
        /// <param name="key">the key of the map</param>
        /// <returns></returns>
        public async Task<Beatmap> getMapbyKeyAsync(string key)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + MapsRoot
                + idRoot
                + key
                );
            if (respondInfo == "Not Found")
                return new Beatmap();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Beatmap>(respondInfo);
        }



        /// <summary>
        /// a function to get instantly the map by the map's hash, in the case of map not found the function will return default values
        /// </summary>
        /// <param name="hash">base64 hash of the map</param>
        /// <returns></returns>
        public async Task<Beatmap> getMapbyHashAsync(string hash)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + MapsRoot
                + hashROOT
                + hash
                );
            if (respondInfo == "Not Found")
                return new Beatmap();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Beatmap>(respondInfo);
        }

        #endregion



        #region beatsaver be like middle man scoresaber



        /// <summary>
        /// returns the leaderboard of the specific map, will return an empty array of scores in case of out of pages
        /// </summary>
        /// <param name="hash">the map's hash</param>
        /// <param name="page">leaderboard page</param>
        /// <param name="difficulty">difficulty of the map</param>
        /// <param name="gamemode">gamemode, type 0 for soloStandard</param>
        /// <returns></returns>
        public LeaderBoard getSongLeaderboard(string hash, int page, Difficulty difficulty, int gamemode)
        {
            string respondInfo = apicall(
               ROOT
               + APIRoot
               + scoresROOT
               + hash
               + @"/"
               + page
               + "?"
               + "difficulty="
               + (int)difficulty
               + "&gameMode="
               + gamemode
               ).Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderBoard>(respondInfo);
        }

        #endregion


        #region functions that are related to the specified mapper



        /// <summary>
        /// get mapper userData by his specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<userData> getMapperDetailsAsync(int id)
        {
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + usersROOT
                + idRoot
                + id
                );
            if (respondInfo == "Internal Server Error")
                return new userData();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<userData>(respondInfo);
        }



        /// <summary>
        /// returns all the maps by the specific mapper in a specific page, 
        /// in case of "out of pages" the function will return null array
        /// </summary>
        /// <param name="mapperID">the mapper's ID</param>
        /// <param name="page">the page you would like, put page as -1 if you would like all the maps</param>
        /// <returns></returns>
        public async Task<Beatmap[]> getMapsByMappersAsync(int mapperID, int page)
        {
            List<Beatmap> temp = new List<Beatmap>();
            string respondInfo = await apicall(
                ROOT
                + APIRoot
                + MapsRoot
                + uploaderROOT
                + mapperID
                + @"/"
                + page
                );
            int counter = 1;
            while (page == -1)
            {
                List<Beatmap> temp2 = new List<Beatmap>();

                respondInfo = await apicall(
                ROOT
                + APIRoot
                + MapsRoot
                + uploaderROOT
                + mapperID
                + @"/"
                + counter++);

                temp2.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs);

                if (temp2.Count == 0)
                    break;

                temp.AddRange(temp2);
            }
            if (temp.Count > 0)
                return temp.ToArray();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<searchResults>(respondInfo).docs;
        }



        /// <summary>
        /// this function will return a playlist of all the maps of the specified mapper
        /// </summary>
        /// <param name="mapperID">the ID of the mapper</param>
        /// <returns></returns>
        public async Task<playList> getPlaylistByMapper(int mapperID)
        {
            string respondInfo = await apicall(
              ROOT
              + APIRoot
              + usersROOT
              + idRoot
              + mapperID
              + @"/"
              + playlistROOT
              );
            return Newtonsoft.Json.JsonConvert.DeserializeObject<playList>(respondInfo);
        }
        #endregion
    }
}
