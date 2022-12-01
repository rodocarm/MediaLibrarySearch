using System;
using System.IO;
using NLog.Web;
using System.Linq;

namespace MediaLibrarySearch
{
    public static class FileScrubber
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        public static string ScrubMovies(string readFile)
        {
            try
            {
                string ext = readFile.Split('.').Last();
                string writeFile = readFile.Replace(ext, $"scrubbed.{ext}");
                if (File.Exists(writeFile))
                {
                    logger.Info("File already scrubbed");
                }
                else
                {
                    logger.Info("File scrub started");
                    StreamWriter sw = new StreamWriter(writeFile);
                    StreamReader sr = new StreamReader(readFile);
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        Movie movie = new Movie();
                        string line = sr.ReadLine();
                        int idx = line.IndexOf('"');
                        string genres = "";
                        if (idx == -1)
                        {
                            string[] movieDetails = line.Split(',');
                            movie.mediaId = UInt64.Parse(movieDetails[0]);
                            movie.title = movieDetails[1];
                            genres = movieDetails[2];
                            movie.director = movieDetails.Length > 3 ? movieDetails[3] : "unassigned";
                            movie.runningTime = movieDetails.Length > 4 ? TimeSpan.Parse(movieDetails[4]) : new TimeSpan(0);
                        }
                        else
                        {
                            movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                            line = line.Substring(idx);
                            idx = line.LastIndexOf('"');
                            movie.title = line.Substring(0, idx + 1);
                            line = line.Substring(idx + 2);
                            string[] details = line.Split(',');
                            genres = details[0];
                            movie.director = details.Length > 1 ? details[1] : "unassigned";
                            movie.runningTime = details.Length > 2 ? TimeSpan.Parse(details[2]) : new TimeSpan(0);
                        }
                        sw.WriteLine($"{movie.mediaId},{movie.title},{genres},{movie.director},{movie.runningTime}");
                    }
                    sw.Close();
                    sr.Close();
                    logger.Info("File scrub ended");
                }
                return writeFile;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return "";
        }
    }
}