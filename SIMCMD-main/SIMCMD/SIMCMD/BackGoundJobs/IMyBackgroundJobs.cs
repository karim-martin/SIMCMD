using System.Threading.Tasks;

namespace SIMCMD.BackGroundJobs
{
    public interface IMyBackgroundJobs
    {
        public void DownloadFiles();

        public Task ConvertFilesAsync();

        public void ImportFiles();
    }
}