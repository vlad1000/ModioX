﻿using ModioX.Constants;
using ModioX.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ModioX.Database
{
    public class DropboxData
    {
        /// <summary>
        /// Initialization of the class.
        /// </summary>
        /// <returns>instance of the class.</returns>
        public static async Task<DropboxData> Initialize()
        {
            var data = new DropboxData
            {
                CategoriesData = await GetCategories(),
                Mods = await GetMods()
            };

            data.CategoriesData.Categories = data.CategoriesData.Categories.OrderBy(o => o.Title).ToList();
            return data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private DropboxData() { }

        /// <summary>
        /// Contains the categories from the database.
        /// </summary>
        public CategoriesData CategoriesData { get; private set; }

        /// <summary>
        /// Contains the mods from database.
        /// </summary>
        public ModsData Mods { get; private set; }

        /// <summary>
        /// Download and return the data for categories and games.
        /// </summary>
        /// <returns>CategoriesData</returns>
        /// <exception cref="HttpException">Thrown when getting a Bad Response</exception>
        /// <exception cref="JsonSerializationException">Thrown when failing to deserialize the json data.</exception>
        private static async Task<CategoriesData> GetCategories()
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(Urls.CategoriesData);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpException($"Bad response {response.StatusCode}");

            var responseData = response.Content.ReadAsStringAsync().Result;

            if (!IsValidJson(responseData))
                throw new JsonException("Failed to process the data for the Categories.");

            return JsonConvert.DeserializeObject<CategoriesData>(responseData);
        }

        /// <summary>
        /// Download and return the data for mods.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<ModsData> GetMods()
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(Urls.ModsDataPS3);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Bad response {response.StatusCode}");

            var responseData = response.Content.ReadAsStringAsync().Result;

            if (!IsValidJson(responseData))
                throw new JsonSerializationException("Failed to process the data for the Mods.");

            return JsonConvert.DeserializeObject<ModsData>(responseData);
        }

        /// <summary>
        /// Determines a valid json response
        /// </summary>
        /// <param name="data">Json data to validate</param>
        /// <returns>Whether text is valid json format</returns>
        private static bool IsValidJson(string data)
        {
            try
            {
                var unused = JToken.Parse(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}