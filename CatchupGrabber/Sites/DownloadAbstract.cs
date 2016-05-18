using System.Collections.Generic;

namespace CatchupGrabber
{
    abstract public class DownloadAbstract
    {
        public bool RequestedSiteData = false; //true after FillShowsList() is called
        /// <summary>
        /// Handles requesting data for a show and creating a list containing the show's episodes 
        /// </summary>
        /// <param name="selectedIndex">The index of the show wlist</param>
        /// <returns>The selected show name</returns>
        abstract public void ClickDisplayedShow(int selectedIndex);
        /// <summary>
        /// Initializes the show list
        /// </summary>
        abstract public void FillShowsList();
        /// <summary>
        /// Gets the selected name of the episode
        /// </summary>
        /// <param name="selectedIndex">The indexin the episode list</param>
        /// <returns>The selected name</returns>
        abstract public string GetSelectedEpisodeName(int selectedIndex);
        /// <summary>
        /// Gets a donwload object containg information of where to download the episode, subtitles and type 
        /// </summary>
        /// <param name="selectedIndex">The index in the episode list</param>
        /// <returns>The DownloadObject for the episode</returns>
        abstract public DownloadObject GetDownloadObject(int selectedIndex);
        /// <summary>
        /// Removes all data from the episode list
        /// </summary>
        abstract public void CleanEpisodes();
        /// <summary>
        /// Gets the subtitle for the episode
        /// </summary>
        /// <returns>A url to the subtitle, an epty string if unavalible</returns>
        abstract public string GetSubtitles();
        /// <summary>
        /// Gets a list of shows used to display in a list box
        /// </summary>
        /// <returns>A list of shows</returns>
        abstract public List<object> GetShowsList();
        /// <summary>
        /// Gets a list of episode used to display in a list box
        /// </summary>
        /// <returns>A list of episodes for a show</returns>
        abstract public List<object> GetEpisodesList();
        /// <summary>
        /// Gets the description of a show
        /// </summary>
        /// <returns>If a description is avalible a description, otherwise null</returns>
        abstract public string GetDescriptionShow(int selectedIndex);
        /// <summary>
        /// Gets the description of an episode
        /// </summary>
        /// <param name="selectedIndex">The index of the selected episode in the episode list</param>
        /// <returns>If a description is avalible a description, otherwise the show's description if avalible, otherwise null</returns>
        abstract public string GetDescriptionEpisode(int selectedIndex);
        /// <summary>
        /// Gets the name of the selected show
        /// </summary>
        /// <param name="selectedIndex">The index in the show list</param>
        /// <returns>THe name of the show</returns>
        abstract public string GetSelectedShowName(int selectedIndex);
    }
}
