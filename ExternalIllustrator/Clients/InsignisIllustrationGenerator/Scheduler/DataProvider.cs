using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Scheduler
{
    public class DataProvider
    {
        private const string apiUrl ="https://test.insigniscash.com/Admin/API/Illustrator/Query.aspx?method=Catalogue&format=JSON&apikey=d6446736-2f17-4a16-8d8f-13226169f68a";

        private readonly HttpClient _httpClient;
        

        public DataProvider()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Insignis.Asset.Management.Illustrator.Interface.Bank>> UpdateString()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadAsStringAsync();
                    BankProducts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Insignis.Asset.Management.Illustrator.Interface.Bank>>(products);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return BankProducts;
        }

        public List<Insignis.Asset.Management.Illustrator.Interface.Bank> BankProducts { get; set; } 

    }

}
