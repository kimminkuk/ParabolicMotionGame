using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;


namespace ParbolicMotionGame.Data
{
    public class ParabolicDBManager
    {
        static readonly string BaseAddress = "http://39.124.28.208:1234/api/ParabolicDB4s";
        static readonly string Url = $"{BaseAddress}";

        private async Task<HttpClient> GetClient()
        {
            HttpClient client = new HttpClient();
            return client;
        }
        public async Task<IEnumerable<ParabolicDB>> GetAll()
        {
            // TODO: use GET to retrieve books
            HttpClient client = await GetClient();
            string result = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<ParabolicDB>>(result);
        }

        public async Task<IEnumerable<ParabolicDB>> GetAll4()
        {
            // TODO: use GET to retrieve books
            HttpClient client = await GetClient();
            string result = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<ParabolicDB>>(result);
        }

        public async Task<List<ParabolicDB>> Add(int LevelPrabolic_, int ScoreParabolic_, string NameParabolic_)
        {
            ////// TODO: use POST to add a book
            //(1) eventid 추가
            //(2) Date 아 스트링이네 ㅅㅂ
            ParabolicDB parabolicDB = new ParabolicDB()
            {
                LevelPrabolic_class = LevelPrabolic_,
                ScoreParabolic_class = ScoreParabolic_,
                NameParabolic_class = NameParabolic_,
                DateParabolic_class = DateTime.Now,
            };


            string BaseAddressAdd = "http://39.124.28.208:1234/api/ParabolicDB4";
            string UrlAdd = $"{BaseAddressAdd}";
            HttpClient client = await GetClient();
            var reponse = await client.PostAsync(UrlAdd,
                new StringContent
                (
                    JsonConvert.SerializeObject(parabolicDB),
                    Encoding.UTF8, "application/json")
                );
            return JsonConvert.DeserializeObject<List<ParabolicDB>>
                (
                  await reponse.Content.ReadAsStringAsync()
                );
        }

    }
}
