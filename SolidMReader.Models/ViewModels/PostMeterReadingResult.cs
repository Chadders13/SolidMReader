namespace SolidMReader.Models.ViewModels;

public class PostMeterReadingResult
{
    public PostMeterReadingResult()
    {
        FailedReading = new List<string>();
    }

    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<string> FailedReading { get; set; }
}