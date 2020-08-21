using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext dataContext, IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckTeamsAsync();
            await CheckTournamentsAsync();
            await CheckUserAsync("1010", "Enrique", "Gudiel", "vvw2099@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("1010", "Enrique", "Gudiel", "vvw2099@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            
            await CheckPreditionsAsync();
        }

        private async Task<UserEntity> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    Team = _dataContext.Teams.FirstOrDefault(),
                    UserType=userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTeamsAsync()
        {
            if (!_dataContext.Teams.Any())
            {
                AddTeamAsync("Ajax");
                AddTeamAsync("America");
                AddTeamAsync("Argentina");
                AddTeamAsync("Barcelona");
                AddTeamAsync("Bayer Leverkusen");
                AddTeamAsync("Bolivia");
                AddTeamAsync("Borussia Dortmund");
                AddTeamAsync("Brasil");
                AddTeamAsync("Bucaramanga");
                AddTeamAsync("Canada");
                AddTeamAsync("Chelsea");
                AddTeamAsync("Chile");
                AddTeamAsync("Colombia");
                AddTeamAsync("Costa Rica");
                AddTeamAsync("Ecuador");
                AddTeamAsync("Honduras");
                AddTeamAsync("Inter Milan");
                AddTeamAsync("Junior");
                AddTeamAsync("Juventus");
                AddTeamAsync("Liverpool");
                AddTeamAsync("Medellin");
                AddTeamAsync("Mexico");
                AddTeamAsync("Millonarios");
                AddTeamAsync("Nacional");
                AddTeamAsync("Once Caldas");
                AddTeamAsync("Panama");
                AddTeamAsync("Paraguay");
                AddTeamAsync("Peru");
                AddTeamAsync("PSG");
                AddTeamAsync("Real Madrid");
                AddTeamAsync("Santa Fe");
                AddTeamAsync("Uruguay");
                AddTeamAsync("USA");
                AddTeamAsync("Venezuela");
                await _dataContext.SaveChangesAsync();
            }

            
        }
        private void AddTeamAsync(string name)
        {
            //string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Teams", $"{name}.jpg");
            //string imageId = path;    //await _blobHelper.UploadBlobAsync(path, "teams");
                 _dataContext.Teams.Add(new TeamEntity { Name = name, LogoPath = $"~/images/Teams/{name}.jpg" });
        }

        private async Task CheckTournamentsAsync()
        {
            if (!_dataContext.Tournaments.Any())
            {
                DateTime startDate = DateTime.Today.AddMonths(2).ToUniversalTime();
                DateTime endDate = DateTime.Today.AddMonths(3).ToUniversalTime();

                string imageIdCopaAmerica = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Tournaments", "Copa America 2020.png.jpg");
                string imageIdLigaAguila = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Tournaments", "Liga Aguila 2020-I.png");
                string imageIdChampions = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Tournaments", "Champions 2020.png");

                _dataContext.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = imageIdCopaAmerica,
                    Name = "Copa America 2020",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                             Name = "A",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Colombia") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ecuador") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Panama") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Canada") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Colombia"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ecuador")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Panama"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Canada")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(4).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Colombia"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Panama")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(4).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ecuador"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Canada")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(9).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Canada"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Colombia")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(9).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ecuador"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Panama")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "B",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Argentina") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Paraguay") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Mexico") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chile") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Argentina"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Paraguay")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Mexico"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chile")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(5).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Argentina"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Mexico")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(5).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Paraguay"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chile")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(10).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chile"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Argentina")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(10).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Paraguay"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Mexico")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "C",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Brasil") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Venezuela") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "USA") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Peru") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(2).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Brasil"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Venezuela")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(2).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "USA"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Peru")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(6).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Brasil"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "USA")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(6).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Venezuela"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Peru")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(11).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Peru"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Brasil")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(11).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Venezuela"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "USA")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "D",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Uruguay") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bolivia") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Costa Rica") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Honduras") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(3).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Uruguay"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bolivia")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(3).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Costa Rica"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Honduras")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(7).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Uruguay"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Costa Rica")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(7).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bolivia"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Honduras")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(12).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Honduras"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Uruguay")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(12).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bolivia"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Costa Rica")
                                 }
                             }
                        }
                    }
                });

                startDate = DateTime.Today.AddMonths(1).ToUniversalTime();
                endDate = DateTime.Today.AddMonths(4).ToUniversalTime();

                _dataContext.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = imageIdLigaAguila,
                    Name = "Liga Aguila 2020-I",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                             Name = "A",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "America") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "America"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(4).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "America"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(4).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(9).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "America")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(9).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(15).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "America")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(15).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(19).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "America")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(19).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(19).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "America"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Medellin")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(19).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bucaramanga")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "B",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(5).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(5).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(10).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(10).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(16).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(16).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(20).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(20).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(35).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Santa Fe")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(35).AddHours(16),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Nacional")
                                 }
                             }
                        }
                    }
                });

                startDate = DateTime.Today.AddMonths(1).ToUniversalTime();
                endDate = DateTime.Today.AddMonths(2).ToUniversalTime();

                _dataContext.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = imageIdChampions,
                    Name = "Champions 2020",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                             Name = "A",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ajax") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Barcelona") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ajax"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Barcelona")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Barcelona"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Ajax")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "B",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bayer Leverkusen") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chelsea") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bayer Leverkusen"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chelsea")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Chelsea"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Bayer Leverkusen")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "C",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Borussia Dortmund") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Inter Milan") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Borussia Dortmund"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Inter Milan")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Inter Milan"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Borussia Dortmund")
                                 }
                             }
                        },
                        new GroupEntity
                        {
                             Name = "D",
                             GroupDetails = new List<GroupDetailEntity>
                             {
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "PSG") },
                                 new GroupDetailEntity { Team = _dataContext.Teams.FirstOrDefault(t => t.Name == "Real Madrid") }
                             },
                             Matches = new List<MatchEntity>
                             {
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(14),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "PSG"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "Real Madrid")
                                 },
                                 new MatchEntity
                                 {
                                     Date = startDate.AddDays(1).AddHours(17),
                                     Local = _dataContext.Teams.FirstOrDefault(t => t.Name == "Real Madrid"),
                                     Visitor = _dataContext.Teams.FirstOrDefault(t => t.Name == "PSG")
                                 }
                             }
                        }
                    }
                });

                await _dataContext.SaveChangesAsync();
            }
        }
        private async Task CheckPreditionsAsync()
        {
            if (!_dataContext.Predictions.Any())
            {
                foreach (UserEntity user in _dataContext.Users)
                {
                    if (user.UserType == UserType.User)
                    {
                        AddPrediction(user);
                    }
                }

                await _dataContext.SaveChangesAsync();
            }
        }
        private void AddPrediction(UserEntity user)
        {
            foreach(MatchEntity match in _dataContext.Matches)
            {
                _dataContext.Predictions.Add(new PredictionEntity
                {
                    GoalsLocal = _random.Next(0, 5),
                    GoalsVisitor=_random.Next(0,5),
                    Match=match,
                    User=user
                });
            }
        }
    }
}
