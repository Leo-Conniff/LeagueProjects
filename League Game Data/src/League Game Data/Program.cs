using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
/*
    This program is designed to scrape the LoL Esports site to get access to professional
    game statisics and store them in a database for easy consumption.  This particular
    implementation works for games prior to 2016.  Separate scaping logic is required for 2016
    onwards as the site format changed and url naming conventions are different now.  The match history
    page is still the same and scraping for that method still works.

    This data will be placed into the staging database and then processed into the reporting database where 
    player and team data will be de-duplicated and proccessed for better consumption.

    Please make sure you either have the SQLlite database or have setup a sql server using the 
    schema in the Database project.  This was originally written using SQL Server, but is utilizing 
    SQLite so others can contribute to gathering data and simply commit the DB back to GitHub for now.
*/
namespace League_Game_Data
{
    class Program
    {
        public static FirefoxDriver firefox;
        
        //TODO: change how game urls are acquired and add date range/Season as command line args
        static void Main(string[] args)
        {
            firefox = new FirefoxDriver();
            //prior to 2016 season all games had a url format that use this base url
            //and have a game number that would redirect to the main game landing page
            //This utilizes that format to iterate through a series of games without scraping links

            var baseUrl = "http://2015.na.lolesports.com/tourney/match/";
            
            createGames(baseUrl, 4681, 4681);

            firefox.Quit();
        }
        
        //TODO: Create new version of this method for 2016+ season
        public static void createGames(string baseUrl, int startMatch, int endMatch)
        {
            for (int i = startMatch; i <= endMatch; ++i)
            {
                var currentUrl = String.Format("{0}{1}", baseUrl, i);
                firefox.Navigate().GoToUrl(currentUrl);

                var teamString = firefox.Title.Split(new string[] { "|" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                var teams = teamString.Split(new string[] { "vs" }, 2, StringSplitOptions.RemoveEmptyEntries);
                //find the button with the link to match history
                var matchHistory = firefox.FindElementByCssSelector(".btn-secondary").GetAttribute("href");
                //go to the stats page of match history
                firefox.Navigate().GoToUrl(String.Format("{0}&tab=stats", matchHistory));
                createGame(teams, matchHistory);
            }
        }

        //This method should continue to work for all seasons as match history page remains the same
        //Method assumes that firefox browser has already made it to the match history page
        public static void createGame(string[] teams, string matchHistory )
        {
            firefox.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            var wait = new WebDriverWait(firefox, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.CssSelector(".map-header-date .binding")));

            var date = firefox.FindElementByCssSelector(".map-header-date .binding").Text;
            var duration = convertDuration(firefox.FindElementByCssSelector(".map-header-duration .binding").Text);
            var gameData = new Tuple<string[], string, string, string>(teams, matchHistory, date, duration);

            var game = new LeagueGame(scrapeStats(), gameData);

            //TODO: find reliable way to assign bans to a team.
            //Can't rely on there always being 6 bans and no tagging on site
            //that would let us tell a particular ban belongs to one side or the other
            var bans = new List<string>();
            var bansList = firefox.FindElementsByCssSelector(".bans img");
            foreach (var ban in bansList)
            {
                var parent = ban.FindElement(By.XPath(".."));
                bans.Add(parent.GetAttribute("data-rg-id"));
            }

            game.processBans(bans);
            game.processStats();
        }
        //Uses firefox driver that has already navigated to the match history page
        //sometimes the previous elements have loaded before the grid has loaded
        //another wait was added to make sure the grid elements are available

        /*Data Array:
        0-3 - KDA, Largest Killing Spree (LKS), Largest Multi Kill (LMK), First Blood
        4-12 - Damage Dealt statistics, 4-7 to Champs, 8-11 total, 12 Largest Critical Strike (LCS)
        13-17 - Healing/Damage Taken
        18-21 - Wards
        22-27 - Gold Earned, Gold Spent, Minions Killed, Neutral Minions Killed (NMK), Neutral Minions killed in team/enemy jungle
        28-29 - Champion Key later mapped to Champion ID from LoL API, player Name later mapped to player Id
        */
        public static string[][] scrapeStats()
        {
            
            var wait = new WebDriverWait(firefox, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.CssSelector(".team-100 .binding span")));

            //The following were the easiest css elements to use to locate the proper grids
            var teamNames = firefox.FindElementsByCssSelector(".binding span"); 
            var teamStats = firefox.FindElementsByCssSelector(".grid-cell");
            //It was easier to locate the champion images and get the parent container then 
            //try and locate the champions played by themselves
            var champImages = firefox.FindElementsByCssSelector("#stats-body img");

            var result = new string[10][];

            //Generate a blank 2d Array to store player stats
            for (int i = 0; i < result.GetLength(0); ++i)
            {
                result[i] = new string[30];
            }
            
            for (int i = 0; i < 10; ++i)
            {
                //From champ images we can go to parent div and get champ name from data id
                var parent = champImages.ElementAt(i).FindElement(By.XPath(".."));
                result[i][28] = parent.GetAttribute("data-rg-id");
                result[i][29] = teamNames.ElementAt(i).Text;
                
                //10 columns for 10 players and 28 rows for different stats
                for (int j = 0; j < 28; ++j)
                {
                    var data = teamStats.ElementAt(j * 10 + i).Text;
                    //Numbers are in format 14.5k, convert to 14500
                    data = data.Replace(".", "");
                    data = data.Replace("k", "00");
                    //- used when 0/no data
                    data = data.Replace("-", "0");
                    result[i][j] = data;
                }
                Console.WriteLine(String.Format("{0} has been processed.", result[i][28]));
            }

            return result;
        }

        public static string convertDuration(string duration)
        {
            //add 0 hours if time is in minutes to convert properly
            int count = 0;
            foreach (var letter in duration)
            {
                if (letter == ':')
                {
                    count++;
                }
            }

            if (count == 1)
            {
                duration = String.Format("00:{0}", duration);
            }

            return duration;
        }
    }
}
