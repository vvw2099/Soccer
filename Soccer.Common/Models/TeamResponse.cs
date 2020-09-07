namespace Soccer.Common.Models
{
    public class TeamResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LogoPath { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
            ? "http://www.soccer.somee.com/images/noimage.png"
            : $"http://www.soccer.somee.com{LogoPath.Replace('~', '/')}";
    }
}
