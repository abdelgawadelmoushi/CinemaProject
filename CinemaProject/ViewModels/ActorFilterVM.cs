namespace CinemaProject.ViewModels
{
    public record ActorFilterVM(string? ActorName , int? ActorId, string Skills, string CinemaName, int age, string Img , int? CinemaId, int? CategoryId, int page = 1 )
    {
    }
}
