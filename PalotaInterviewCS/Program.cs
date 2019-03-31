using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
namespace PalotaInterviewCS
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string countriesEndpoint = "https://restcountries.eu/rest/v2/all";

        static void Main(string[] args)
        {
            Country[] countries = GetCountries(countriesEndpoint).GetAwaiter().GetResult();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Palota Interview: Country Facts");
            Console.WriteLine();
            Console.ResetColor();

            Random rnd = new Random(); // random to populate fake answer - you can remove this once you use real values

            // List<Country> _countries = new List<Country>(countries.);
            //TODO use data operations and data structures to optimally find the correct value (N.B. be aware of null values)

            /*
             * HINT: Sort the list in descending order to find South Africa's place in terms of gini coefficients
             * `Country.Gini` is the relevant field to use here           
             */

            int southAfricanGiniPlace = countries.OrderByDescending(o => o.Gini).ToList().FindIndex(r => r.Name == "South Africa") + 1;

            Console.WriteLine($"1. South Africa's Gini coefficient is the {GetOrdinal(southAfricanGiniPlace)}  highest");

            /*
             * HINT: Sort the list in ascending order or just find the Country with the minimum gini coeficient          
             * use `Country.Gini` for the ordering then return the relevant country's name `Country.Name`
             */
            string lowestGiniCountry = countries.OrderBy(o => o.Gini).ToList().Where(o => o.Gini != null).First().Name;
            Console.WriteLine($"2. {lowestGiniCountry} has the lowest Gini Coefficient");

            /*
             * HINT: Group by regions (`Country.Region`), then count the number of unique timezones that the countries in each region span
             * Once you have done the grouping, find the group `Region` with the most timezones and return it's name and the number of unique timezones found          
             */

            var RegionsWithZones = countries.GroupBy(r => r.Region)
                                            .Select(s =>
                                                   new
                                                   {
                                                       Region = s.Key,
                                                       TotalTimezonesPerRegion = s.Select(o => o.Timezones).Distinct().Count()
                                                   });

            string regionWithMostTimezones = RegionsWithZones.OrderBy(o => o.TotalTimezonesPerRegion).Last().Region.ToString();
            int amountOfTimezonesInRegion = RegionsWithZones.OrderBy(o => o.TotalTimezonesPerRegion).Last().TotalTimezonesPerRegion; // Use correct value

            Console.WriteLine($"3. {regionWithMostTimezones} is the region that spans most timezones at {amountOfTimezonesInRegion} timezones");

            /*
             * HINT: Count occurances of each currency in all countries (check `Country.Currencies`)
             * Find the name of the currency with most occurances and return it's name (`Currency.Name`) also return the number of occurances found for that currency          
             */

            List<Currency> CurrencyList = new List<Currency>();

            countries.ToList().ForEach(o =>
            {
                CurrencyList.AddRange(o.Currencies);
            });

            var CurrencyAll = CurrencyList.GroupBy(r => r.Name)
                                           .Select(s =>
                                                 new
                                                 {
                                                       MostPopularCurrency = s.Key,
                                                       NumCountriesUsedByCurrency = s.Count()
                                               }).ToList();

            string mostPopularCurrency = CurrencyAll.OrderBy(o => o.NumCountriesUsedByCurrency).Select(o => o.MostPopularCurrency).Last();
            int numCountriesUsedByCurrency = CurrencyAll.OrderBy(o => o.NumCountriesUsedByCurrency).Select(o => o.NumCountriesUsedByCurrency).Last();

            Console.WriteLine($"4. {mostPopularCurrency} is the most popular currency and is used in {numCountriesUsedByCurrency} countries");

            /*
             * HINT: Count the number of occurances of each language (`Country.Languages`) and sort then in descending occurances count (i.e. most populat first)
             * Once done return the names of the top three languages (`Language.Name`)
             */

            List<Language> Languages = new List<Language>();

            countries.ToList().ForEach(o =>
            {
                Languages.AddRange(o.Languages);
            });

            string[] mostPopularLanguages = Languages.GroupBy(o => o.Name)
                                                     .Select(s => new
                                                        {
                                                            Language = s.Key,
                                                            MostPopularLanguages = s.Count()
                                                        }).OrderByDescending(o => o.MostPopularLanguages)
                                                        .Select(o => o.Language).ToArray();


            Console.WriteLine($"5. The top three popular languages are {mostPopularLanguages[0]}, {mostPopularLanguages[1]} and {mostPopularLanguages[2]}");

            /*
             * HINT: Each country has an array of Bordering countries `Country.Borders`, The array has a list of alpha3 codes of each bordering country `Country.alpha3Code`
             * Sum up the population of each country (`Country.Population`) along with all of its bordering countries's population. Sort this list according to the combined population descending
             * Find the country with the highest combined (with bordering countries) population the return that country's name (`Country.Name`), the number of it's Bordering countries (`Country.Borders.length`) and the combined population
             * Be wary of null values           
             */

            Dictionary<string, long> countryPopution = new Dictionary<string, long>();

            countries.ToList().ForEach(o =>
            {
                var sumOfCombinedPopulation = countries.Where(a => o.Borders.Contains(a.Alpha3Code)).Sum(s => s.Population);
                countryPopution.Add(o.Name, sumOfCombinedPopulation);
            });

            var countryPoputionDSorted = countryPopution.OrderByDescending(o => o.Value).Select(o => o);

            string countryWithBorderingCountries = countryPoputionDSorted.Select(o => o.Key).First(); // Use correct value
            int numberOfBorderingCountries = countries.Where(o => o.Name == countryWithBorderingCountries).Select(o => o.Borders).FirstOrDefault().Count(); // Use correct value

            long combinedPopulation = countryPoputionDSorted.Select(o => o.Value).First(); // Use correct value

            Console.WriteLine($"6. {countryWithBorderingCountries} and it's {numberOfBorderingCountries} bordering countries has the highest combined population of {combinedPopulation}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the lowest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of null values when doing calculations           
             */


            Dictionary<string, double> CountryPopulationDensity = new Dictionary<string, double>();

            countries.ToList().ForEach(o =>
            {
                if (o.Area != null)
                {
                    var PopulationDensity = o.Population / o.Area.Value;
                    CountryPopulationDensity.Add(o.Name, PopulationDensity);
                }
            });

            var CountryPopulationDensitySorted = CountryPopulationDensity.OrderBy(o => o.Value).Select(o => o);
            
            string lowPopDensityName = CountryPopulationDensitySorted.Select(o => o.Key).First(); // Use correct value
            double lowPopDensity = CountryPopulationDensitySorted.Select(o => o.Value).First(); // Use correct value

            Console.WriteLine($"7. {lowPopDensityName} has the lowest population density of {lowPopDensity}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the highest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of any null values when doing calculations. Consider reusing work from above related question           
             */
            string highPopDensityName = CountryPopulationDensitySorted.Select(o => o.Key).Last(); // Use correct value
            double highPopDensity = CountryPopulationDensitySorted.Select(o => o.Value).Last(); // Use correct value
            Console.WriteLine($"8. {highPopDensityName} has the highest population density of {highPopDensity}");

            /*
             * HINT: Group by subregion `Country.Subregion` and sum up the area (`Country.Area`) of all countries per subregion
             * Sort the subregions by the combined area to find the maximum (or just find the maximum)
             * Return the name of the subregion
             * Be wary of any null values when summing up the area           
             */

            var subregion = countries.GroupBy(o => o.Subregion)
                                       .Select(s => new
                                       {
                                           subregion = s.Key,
                                           area = s.Sum(o => o.Area ?? 0)
                                       }).OrderByDescending(o => o.area).FirstOrDefault();

            string largestAreaSubregion = subregion.subregion; // Use correct value
            Console.WriteLine($"9. {largestAreaSubregion} is the subregion that covers the most area");

            /*
             * HINT: Group by regional blocks (`Country.RegionalBlocks`). For each regional block, average out the gini coefficient (`Country.Gini`) of all member countries
             * Sort the regional blocks by the average country gini coefficient to find the lowest (or find the lowest without sorting)
             * Return the name of the regional block (`RegionalBloc.Name`) along with the calculated average gini coefficient
             */
            var RegionalBlocks = countries.Where(o => o.RegionalBlocs.Count() != 0)
                                          .GroupBy(o => o.RegionalBlocs.Select(s => s.Name).FirstOrDefault())
                                          .Select(s => new
                                          {
                                              subregion = s.Key,
                                              avgGini = s.Average(o => o.Gini ?? 0)
                                          }).OrderBy(o => o.avgGini).ToList();

            string mostEqualRegionalBlock = RegionalBlocks.First().subregion; // Use correct value
            double lowestRegionalBlockGini = RegionalBlocks.First().avgGini; // Use correct value

            Console.WriteLine($"10. {mostEqualRegionalBlock} is the regional block with the lowest average Gini coefficient of {lowestRegionalBlockGini}");
        }

        /// <summary>
        /// Gets the countries from a specified endpiny
        /// </summary>
        /// <returns>The countries.</returns>
        /// <param name="path">Path endpoint for the API.</param>
        static async Task<Country[]> GetCountries(string path)
        {
            Country[] countries = null;
            //TODO get data from endpoint and convert it to a typed array using Country.FromJson
            HttpResponseMessage response = await client.GetAsync(path);

            var results = response.Content.ReadAsStringAsync().Result;

            countries = Country.FromJson(results);

            return countries;
        }

        /// <summary>
        /// Gets the ordinal value of a number (e.g. 1 to 1st)
        /// </summary>
        /// <returns>The ordinal.</returns>
        /// <param name="num">Number.</param>
        public static string GetOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }

        }
    }
}
