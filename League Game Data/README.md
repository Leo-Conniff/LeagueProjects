# League of Legends Game Scraper
This utility is designed to scrape the LoL ESports site for games prior to the 2016 season to gather professional game statistics.  This is accomplished by finding a link to the official match history at matchhistory.na.leagueoflegends.com and scaping the gaming statistics from the stats page.  With the start of the 2016 season, a new website format was created and the methodology used to enumerate the game links will no longer work.  However, the match history page remains the same and scraping from that page still works.  A new method to find the links to games for 2016+ will need to be created.

# Install
This utility was written in C# using Visual Studio 2015.  The easiest way to build and compile the application is to download the Community edition of Visual Studio 2015, open the solution file and build it using Visual Studio.

In addition to the utility, you will need a database setup to store the information that is scraped.  A setup script for SQL Server to create tables for a db called League_Staging is included in the Scripts folder.  You will need to update the connString variable in the app.config file with the connection string to your database.  Support for updating a SQLite database has been added with the intention of including a database file in the repository at some point in time.

# Usage
Currently this application has been used from the debugger, so you will need to update the arguments to the createGames method in maind for the series of games that would like to scrape and recompile.  The intention is to update how to select which games to scrape via command line args at some point.

To date, this utility has only been used to pull in games from the NA LCS, but this will also work for any games so long as there is a match history available and you can pass the URLs.

# Things that would make this better
- Support for getting 2016 match history urls
- Make the match history scraping methods independent of the methods to acquire match history urls
- Create a SQLite database with the proper schema and add a copy to the project.  This way everyone can share what they have already scraped within the project.
- Identify a better way to assign bans to a team
- Add more stats like Baron/Dragon times, player builds and other data which is on the match history page that is not currently getting scraped.

# External Libraries
Currently the project uses Selenium for browser automation and includes the dlls for interacting with SQLite.

# Contributing
Anyone is free to submit pull requests or fork the repository and take it in their own direction.  If there are users that take an interest in the project and want to get commit access, then it will be assessed on a case by case basis.

# License
Any code written for this project is released under the MIT License.  A copy of which is included in the license file.  Any 3rd party libraries or utilities are subject to their own license terms.