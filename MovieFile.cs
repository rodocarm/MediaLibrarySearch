using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaLibrarySearch
{
    public class MovieFile
    {
        public string filePath { get; set; }
        public List<Movie> Movies { get; set; }
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        public MovieFile(string movieFilePath)
        {
            filePath = movieFilePath;
            Movies = new List<Movie>();

            try
            {
                StreamReader sr = new StreamReader(filePath);
                while (!sr.EndOfStream)
                {
                    Movie movie = new Movie();
                    string line = sr.ReadLine();
                    int idx = line.IndexOf('"');
                    if (idx == -1)
                    {
                        string[] movieDetails = line.Split(',');
                        movie.mediaId = UInt64.Parse(movieDetails[0]);
                        movie.title = movieDetails[1];
                        movie.genres = movieDetails[2].Split('|').ToList();
                        movie.director = movieDetails[3];
                        movie.runningTime = TimeSpan.Parse(movieDetails[4]);
                    }
                    else
                    {
                        movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        line = line.Substring(idx);
                        idx = line.LastIndexOf('"');
                        movie.title = line.Substring(0, idx + 1);
                        line = line.Substring(idx + 2);
                        string[] details = line.Split(',');
                        movie.genres = details[0].Split('|').ToList();
                        movie.director = details[1];
                        movie.runningTime = TimeSpan.Parse(details[2]);
                    }
                    Movies.Add(movie);
                }
                sr.Close();
                logger.Info("Movies in file {Count}", Movies.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public bool isUniqueTitle(string title)
        {
            if (Movies.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate movie title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddMovie(Movie movie)
        {
            try
            {
                movie.mediaId = Movies.Max(m => m.mediaId) + 1;
                string title = movie.title.IndexOf(',') != -1 || movie.title.IndexOf('"') != -1 ? $"\"{movie.title}\"" : movie.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine($"{movie.mediaId},{title},{string.Join("|", movie.genres)},{movie.director},{movie.runningTime}");
                sw.Close();
                Movies.Add(movie);
                logger.Info("Media id {Id} added", movie.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}